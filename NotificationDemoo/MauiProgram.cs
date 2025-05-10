using Microsoft.Extensions.Logging;
using Microsoft.Maui.LifecycleEvents;
using Plugin.Firebase.Auth;
using Plugin.Firebase.Bundled.Shared;
using Plugin.Firebase.Crashlytics;
using Plugin.Firebase.CloudMessaging;
using System.Diagnostics;




#if IOS
using Plugin.Firebase.Bundled.Platforms.iOS;
#else
using Plugin.Firebase.Bundled.Platforms.Android;
#endif

namespace NotificationDemoo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            builder.RegisterFirebaseServices();
            return builder.Build();
        }


        private static MauiAppBuilder RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddAndroid(android => android.OnCreate((activity, _) =>
                {
                    CrossFirebase.Initialize(activity, CreateCrossFirebaseSettings());

                    CrossFirebaseCloudMessaging.Current.NotificationTapped += async (sender, e) =>
                    {
                        Debug.WriteLine("Notification Tapped");

                        await MainThread.InvokeOnMainThreadAsync(async () =>
                        {
                            string msg = "Hello from Firebase";
                            string encoded = Uri.EscapeDataString(msg);
                            await Shell.Current.GoToAsync($"///DetailsPage?Message={encoded}");
                        });
                    };
                }));
            });

            builder.Services.AddSingleton(_ => CrossFirebaseAuth.Current);
            return builder;
        }

        private static CrossFirebaseSettings CreateCrossFirebaseSettings()
        {
            return new CrossFirebaseSettings(isAuthEnabled: true,
                isCloudMessagingEnabled: true, isAnalyticsEnabled: true);
        }
    }
}
