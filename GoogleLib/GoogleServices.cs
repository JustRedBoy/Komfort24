using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;

namespace GoogleLib
{
    public static class GoogleServices
    {
        private static readonly string AppName = "komfort24";
        public static SheetsService GetSheetsService()
        {
            return new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GetUserCredential(SheetsService.Scope.Spreadsheets),
                ApplicationName = AppName,
            });
        }

        public static SheetsService GetSheetsServiceWithoutLogin()
        {
            using var stream = new FileStream("google-credentials.json", FileMode.Open, FileAccess.Read);
            var serviceInitializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromStream(stream)
                    .CreateScoped(SheetsService.Scope.SpreadsheetsReadonly)
            };
            return new SheetsService(serviceInitializer);
        }

        public static DriveService GetDriveService()
        {
            return new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GetUserCredential(DriveService.Scope.Drive, SheetsService.Scope.Spreadsheets),
                ApplicationName = AppName,
            });
        }

        private static UserCredential GetUserCredential(params string[] scopes)
        {
            using var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read);
            return GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.Load(stream).Secrets,
                scopes,
                "user",
                CancellationToken.None,
                new FileDataStore("auth", true)).Result;
        }
    }
}
