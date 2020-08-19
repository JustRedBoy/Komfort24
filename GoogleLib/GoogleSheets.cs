using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Tools;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using GoogleLib.Exceptions;

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
            try
            {
                var response = await _service.Spreadsheets.Values.
                    Get(spreadSheetId, readRange).ExecuteAsync();
                return response?.Values;
            }
            catch (Exception e)
            {
                throw AccessDeniedException.CreateException(e);
            }
        }

        public async Task WriteInfoAsync(IList<IList<object>> info, string spreadSheetId, string writeRange)
        {
            try
            {
                var valueRange = new ValueRange { Values = info };
                var update = _service.Spreadsheets.Values.Update(valueRange, spreadSheetId, writeRange);
                update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
                await update.ExecuteAsync();
            }
            catch (Exception e)
            {
                throw AccessDeniedException.CreateException(e);
            }
        }

        public async Task<IList<IList<object>>> GetHouseInfoAsync(string houseNumber)
        {
            return await ReadInfoAsync(Sheets.ServiceSpreadSheetId, $"{houseNumber}!A1:AH97");
        }

        public IList<IList<object>> GetHouseInfo(string houseNumber)
        {
            return GetHouseInfoAsync(houseNumber).Result;
        }

        public async Task<IList<object>> GetRatesAsync(string houseNumber)
        {
            var response = await ReadInfoAsync(Sheets.ServiceSpreadSheetId, $"Rates!B2:H7");
            IList<object> rates = houseNumber switch
            {
                "20/1" => response[0],
                "20/2" => response[1],
                "22/2" => response[2],
                "24/2" => response[3],
                "26/1" => response[4],
                "26/2" => response[5],
                _ => throw new ArgumentException("Недопустимый номер дома"),
            };
            string month = rates[6].ToString().ToLower();
            rates[6] = month;
            int numMonth = Date.GetNumMonth(month);
            rates.Add(Date.GetShortNumMonth(numMonth));
            rates.Add(numMonth == 12 ? "01" : Date.GetShortNumMonth(numMonth + 1));

            return rates;
        }

        public IList<object> GetRates(string houseNumber)
        {
            return GetRatesAsync(houseNumber).Result;
        }

        public async Task<IList<IList<object>>> GetPaymentsAsync()
        {
            return await ReadInfoAsync(Sheets.PaymentsSpreadSheetId, "Список платежей!A2:H100000");
        }
    }
}
