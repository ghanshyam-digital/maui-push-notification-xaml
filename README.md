
### 1. Firebase Project Setup

1. Go to the [Firebase Console](https://console.firebase.google.com/).
2. Create a new project (or use an existing one).
3. Add a new Android app to the project (use your MAUI app's package name).
4. Download the `google-services.json` file.
5. Add it to your project root directory.
6. Set its **Build Action** to `GoogleServicesJson` (Right-click → Properties).

### 2. Add Firebase Admin SDK

- Download `firebase-adminsdk.json` from Firebase (Service Account Key).
- Add it to the project as an **Embedded Resource**.
- Ensure its **Build Action** is set to `EmbeddedResource`.

## 📦 NuGet Dependencies

Install the following NuGet packages:

| Package Name                         | Version  |
|-------------------------------------|----------|
| `Google.Apis.Auth`                  | `1.69.0` |
| `Plugin.Firebase`                   | `3.1.4`  |
| `Plugin.Firebase.CloudMessaging`    | `3.1.2`  |
| `Plugin.FirebaseCrashlytics`        | `4.1.0`  |
