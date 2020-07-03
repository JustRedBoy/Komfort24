using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GenerateFlyers
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

        public async Task<IList<IList<object>>> GetHouseInfoAsync(string houseNumber)
        {
            return (await _valuesResource.Get(ServiceSpreadsheetId, 
                $"{houseNumber.Replace('_', '/')}!A1:AH97").ExecuteAsync()).Values;
        }

        public async Task<IList<object>> GetRatesAsync(string houseNumber)
        {
            var response = await _valuesResource.Get(ServiceSpreadsheetId,$"Rates!B2:H7").ExecuteAsync();
            IList<object> rates = null;
            switch (houseNumber)
            {
                case "20_1":
                    rates = response.Values[0];
                    break;
                case "20_2":
                    rates = response.Values[1];
                    break;
                case "22_2":
                    rates = response.Values[2];
                    break;
                case "24_2":
                    rates = response.Values[3];
                    break;
                case "26_1":
                    rates = response.Values[4];
                    break;
                case "26_2":
                    rates = response.Values[5];
                    break;
            }

            string month = rates[6].ToString().ToLower();
            rates[6] = month;
            int numMonth = GetNumMonth(month);
            rates.Add(numMonth);
            rates.Add(numMonth + 1);

            return rates;
        }

        private int GetNumMonth(string month) => month switch
        {
            "январь" => 1,
            "ферваль" => 2,
            "март" => 3,
            "апрель" => 4,
            "май" => 5,
            "июнь" => 6,
            "июль" => 7,
            "август" => 8,
            "сентябрь" => 9,
            "октябрь" => 10,
            "ноябрь" => 11,
            "декабрь" => 12,
            _ => throw new ArgumentException("Недопустимый месяц")
        };
    }
}
