using Google.Apis.Auth.OAuth2;
using Plugin.Firebase.CloudMessaging;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace NotificationDemoo;

public partial class MainPage : ContentPage
{
    private string deviceToken;
    private string projectId;

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        try
        {
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            deviceToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();

            Console.WriteLine($"FCM Token: {deviceToken}");

            await SendPushNotification(deviceToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    private async Task<string> GetAccessToken()
    {
        var assembly = typeof(MainPage).Assembly;
        var resourceName = "NotificationDemoo.firebase-adminsdk.json";

        using Stream originalStream = assembly.GetManifestResourceStream(resourceName);
        if (originalStream == null)
            throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");

        using var memoryStream = new MemoryStream();
        await originalStream.CopyToAsync(memoryStream);
        byte[] streamBytes = memoryStream.ToArray();

        using (var jsonDoc = JsonDocument.Parse(streamBytes))
        {
            if (jsonDoc.RootElement.TryGetProperty("project_id", out var projectIdElement))
            {
                projectId = projectIdElement.GetString();
            }
            else
            {
                throw new Exception("project_id not found in firebase-adminsdk.json");
            }
        }

        using var credentialStream = new MemoryStream(streamBytes);

        var credential = GoogleCredential
            .FromStream(credentialStream)
            .CreateScoped("https://www.googleapis.com/auth/firebase.messaging");

        var accessToken = await credential.UnderlyingCredential.GetAccessTokenForRequestAsync();
        return accessToken;
    }

    private async Task SendPushNotification(string token)
    {
        var accessToken = await GetAccessToken();

        var message = new
        {
            message = new
            {
                token = token,
                notification = new
                {
                    title = "Hello from MAUI",
                    body = "This is a test notification."
                }
            }
        };

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var content = new StringContent(JsonSerializer.Serialize(message), Encoding.UTF8, "application/json");

        var response = await client.PostAsync(
            $"https://fcm.googleapis.com/v1/projects/{projectId}/messages:send",
            content
        );

        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Push Result: {response.StatusCode} - {responseBody}");
    }
}
