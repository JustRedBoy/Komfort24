using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Web.Services
{
    public class GoogleSheets
    {
        private static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private const string ServiceSpreadsheetId = "1E0bdRIXxwgSyxw_OFWLjYaIoh1qv38IAUZS0xoy84IY";
        private const string GoogleCredentialsFileName = "google-credentials.json";

        private readonly SpreadsheetsResource.ValuesResource _valuesResource;

        public GoogleSheets()
        {
            _valuesResource = GetSheetsService().Spreadsheets.Values;
        }

        private SheetsService GetSheetsService()
        {
            using var stream = new FileStream(GoogleCredentialsFileName, FileMode.Open, FileAccess.Read);
            var serviceInitializer = new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleCredential.FromStream(stream).CreateScoped(Scopes)
            };
            return new SheetsService(serviceInitializer);
        }

        public async Task<Account> GetAccountAsync(string accountId)
        {
            (Houses.House houseId, int numRow) = GetAccountInfo(accountId);
            var response = await _valuesResource.Get(ServiceSpreadsheetId, 
                $"{Houses.GetHouseInfo(houseId).fullHouseNumber}!A{numRow}:AH{numRow}").ExecuteAsync();
            return new Account(response.Values[0]);
        }

        public async Task<Rates> GetRatesAsync(Houses.House houseId)
        {
            int numRow = (int)houseId + 2;
            var response = await _valuesResource.Get(ServiceSpreadsheetId, $"Rates!B{numRow}:F{numRow}").ExecuteAsync();
            return new Rates(houseId, 
                double.Parse(response.Values[0][4].ToString()),
                double.Parse(response.Values[0][3].ToString()),
                double.Parse(response.Values[0][0].ToString()),
                double.Parse(response.Values[0][2].ToString()),
                double.Parse(response.Values[0][1].ToString())); 
        }

        public async Task<IEnumerable<Rates>> GetRatesAsync()
        {
            List<Rates> rates = new List<Rates>();
            var response = await _valuesResource.Get(ServiceSpreadsheetId, $"Rates!B2:F7").ExecuteAsync();

            for (int i = 0; i < 6; i++)
            {
                rates.Add(new Rates((Houses.House)i,
                    double.Parse(response.Values[i][4].ToString()),
                    double.Parse(response.Values[i][3].ToString()),
                    double.Parse(response.Values[i][0].ToString()),
                    double.Parse(response.Values[i][2].ToString()),
                    double.Parse(response.Values[i][1].ToString())));
            }
            return rates;
        }

        private (Houses.House houseId, int numRow) GetAccountInfo(string accountId)
        {
            Houses.House houseId = Houses.GetHouse(accountId);
            return (houseId, Houses.GetNumRow(houseId, accountId));
        }
    }
}
