using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GoogleLib.Tools;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleLib
{
    public class GoogleSheets
    {
        private readonly SheetsService _service;

        public GoogleSheets()
        {
            _service = GoogleServices.GetSheetsService();
        }

        public async Task<IList<IList<object>>> ReadInfoAsync(string spreadSheetId, string readRange)
        {
            var response = await _service.Spreadsheets.Values.
                Get(spreadSheetId, readRange).ExecuteAsync();
            return response.Values;
        }

        public async Task WriteInfoAsync(IList<IList<object>> info, string spreadSheetId, string writeRange)
        {
            var valueRange = new ValueRange { Values = info };
            var update = _service.Spreadsheets.Values.Update(valueRange, spreadSheetId, writeRange);
            update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            await update.ExecuteAsync();
        }

        public async Task<IList<IList<object>>> GetHouseInfoAsync(string houseNumber)
        {
            return await ReadInfoAsync(Sheets.ServiceSpreadSheetId, 
                $"{houseNumber.Replace('_', '/')}!A1:AH97");
        }

        public IList<IList<object>> GetHouseInfo(string houseNumber)
        {
            lock (this)
            {
                return GetHouseInfoAsync(houseNumber).Result;
            }
        }

        public async Task<IList<object>> GetRatesAsync(string houseNumber)
        {
            var response = await ReadInfoAsync(Sheets.ServiceSpreadSheetId, $"Rates!B2:H7");
            IList<object> rates = null;
            switch (houseNumber)
            {
                case "20_1":
                    rates = response[0];
                    break;
                case "20_2":
                    rates = response[1];
                    break;
                case "22_2":
                    rates = response[2];
                    break;
                case "24_2":
                    rates = response[3];
                    break;
                case "26_1":
                    rates = response[4];
                    break;
                case "26_2":
                    rates = response[5];
                    break;
            }

            string month = rates[6].ToString().ToLower();
            rates[6] = month;
            int numMonth = Date.GetNumMonth(month);
            rates.Add(Date.GetShortNumMonth(numMonth));
            rates.Add(numMonth == 12 ? "01" : Date.GetShortNumMonth(numMonth + 1));

            return rates;
        }

        public IList<object> GetRates(string houseNumber)
        {
            lock (this)
            {
                return GetRatesAsync(houseNumber).Result;
            }
        }

        public async Task<IList<IList<object>>> GetPaymentsAsync()
        {
            return await ReadInfoAsync(Sheets.PaymentsSpreadSheetId, "Список платежей!A2:H100000");
        }
    }
}
