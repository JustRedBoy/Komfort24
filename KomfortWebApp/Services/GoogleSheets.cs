using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using KomfortWebApp.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KomfortWebApp.Services
{
    public class GoogleSheets
    {
        private static readonly string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        private const string WerSpreadsheetId = "1LQ0CifcavN-T9RKV0MkAzgSA3KWNwuswkxGbGpl8VuE";
        private const string HeatingSpreadsheetId = "1yFpPyeM2DlBRNYEPu_Zo0kkyuLay-u56Y-IeU_M9RfY";
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
            string fullHouseNumber = Houses.GetHouseInfo(houseId).fullHouseNumber;
            var response = await _valuesResource.Get(WerSpreadsheetId, $"{fullHouseNumber}!A{numRow}:S{numRow}").ExecuteAsync();
            var values = response.Values;

            var response2 = await _valuesResource.Get(HeatingSpreadsheetId, $"{fullHouseNumber}!D{numRow}:R{numRow}").ExecuteAsync();
            values.Add(response2.Values[0]);
            return new Account(values);
        }

        public async Task<Rates> GetRatesAsync(Houses.House houseId)
        {
            string fullHouseNumber = Houses.GetHouseInfo(houseId).fullHouseNumber;
            var response = await _valuesResource.Get(WerSpreadsheetId, $"{fullHouseNumber}!G3:K6").ExecuteAsync();
            var values1 = response.Values;

            var response2 = await _valuesResource.Get(HeatingSpreadsheetId, $"{fullHouseNumber}!K3:K6").ExecuteAsync();
            var values2 = response2.Values;

            return new Rates(houseId, 
                double.Parse(values1[0][0].ToString()),
                double.Parse(values1[3][0].ToString()),
                double.Parse(values1[3][4].ToString()),
                double.Parse(values2[0][0].ToString()),
                double.Parse(values2[3][0].ToString())); 
        }

        public async Task<IEnumerable<Rates>> GetRatesAsync()
        {
            List<Rates> rates = new List<Rates>();
            for (int i = 1; i < 7; i++)
            {
                Houses.House house = (Houses.House)i;
                string fullHouseNumber = Houses.GetHouseInfo(house).fullHouseNumber;
                if(house == Houses.House.House_20_2)
                {
                    var res = await _valuesResource.Get(HeatingSpreadsheetId, $"{fullHouseNumber}!K3:K6").ExecuteAsync();
                    var val = res.Values;
                    rates.Add(new Rates(house,
                        0.0,
                        0.0,
                        0.0,
                        double.Parse(val[0][0].ToString()),
                        double.Parse(val[3][0].ToString())));
                    continue;
                }
                var response = await _valuesResource.Get(WerSpreadsheetId, $"{fullHouseNumber}!G3:K6").ExecuteAsync();
                var values1 = response.Values;

                var response2 = await _valuesResource.Get(HeatingSpreadsheetId, $"{fullHouseNumber}!K3:K6").ExecuteAsync();
                var values2 = response2.Values;
                rates.Add(new Rates(house,
                    double.Parse(values1[0][0].ToString()),
                    double.Parse(values1[3][0].ToString()),
                    double.Parse(values1[3][4].ToString()),
                    double.Parse(values2[0][0].ToString()),
                    double.Parse(values2[3][0].ToString())));
            }
            return rates;
        }

        private (Houses.House houseId, int numRow) GetAccountInfo(string accountId)
        {
            int numRow = int.Parse(accountId) - 7637 + 9;
            if (accountId.Contains("/2"))
            {
                numRow++;
            }
            return (Houses.GetHouse(accountId), numRow);
        }
    }
}
