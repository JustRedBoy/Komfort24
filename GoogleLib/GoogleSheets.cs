using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using GoogleLib.Tools;
using System;
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

        public async Task AddNewPayments()
        {
            IList<IList<object>> payments = new List<IList<object>>();
            for (int i = 0; i < Houses.NumberHouses; i++)
            {
                var info = await ReadInfoAsync(Sheets.ServiceSpreadSheetId,
                    $"{Houses.GetHouseInfo(i).fullHouseNumber}!A1:AH97");
                int countNum = Houses.GetHouseInfo(i).fullHouseNumber == "24/2" ? 97 : 96;
                for (int j = 0; j < countNum; j++)
                {
                    double heatingPayment = GetNumber(info[j][13]) + GetNumber(info[j][14]);
                    double werPayment = GetNumber(info[j][29]) + GetNumber(info[j][30]);
                    if (heatingPayment != 0 || werPayment != 0) 
                    {
                        double forWater = GetNumber(info[j][25]);
                        double forWer = Math.Round(werPayment - forWater, 2);
                        payments.Add(new List<object>() 
                        {
                            info[j][1], 
                            info[j][2],
                            forWer,
                            forWater,
                            heatingPayment,
                            forWer + forWater + heatingPayment,
                            Date.GetNamePrevMonth(),
                            DateTime.Now.Year
                        });
                    }
                }
            }
            var oldPayments = await ReadInfoAsync(Sheets.PaymentsSpreadSheetId, "Список платежей!A2:H100000"); // max
            foreach (var item in oldPayments)
            {
                payments.Add(new List<object>()
                {
                    item[0],
                    item[1],
                    GetNumber(item[2]),
                    GetNumber(item[3]),
                    GetNumber(item[4]),
                    GetNumber(item[5]),
                    item[6],
                    item[7]
                });
            }
            await WriteInfoAsync(payments, Sheets.PaymentsSpreadSheetId, $"A2:H{payments.Count + 1}");
        }

        public async Task CorrectFiles()
        {
            var month = new List<IList<object>> { new List<object>() };
            month[0].Add(Date.GetNameCurMonth());

            for (int i = 0; i < Houses.NumberHouses; i++)
            {
                string houseNum = Houses.GetHouseInfo(i).fullHouseNumber;
                int numFlats = Houses.GetNumFlats(i);

                // Change debit and credit
                var debitAndCredit = await ReadInfoAsync(Sheets.WerSpreadSheetId, $"{houseNum}!Q9:R{8 + numFlats}");
                await WriteInfoAsync(GetEmptyList(numFlats, 2), Sheets.WerSpreadSheetId, $"{houseNum}!D9:E{8 + numFlats}");
                await WriteInfoAsync(GetListDoubles(debitAndCredit), Sheets.WerSpreadSheetId, $"{houseNum}!D9:E{8 + numFlats}");

                // Dublicate last values
                var lastValues = await ReadInfoAsync(Sheets.WerSpreadSheetId, $"{houseNum}!H9:H{8 + numFlats}");
                await WriteInfoAsync(GetListDoubles(lastValues), Sheets.WerSpreadSheetId, $"{houseNum}!I9:I{8 + numFlats}");

                // Clear payments
                await WriteInfoAsync(GetEmptyList(numFlats, 2), Sheets.WerSpreadSheetId, $"{houseNum}!O9:P{8 + numFlats}");

                // Clear privileges
                await WriteInfoAsync(GetEmptyList(numFlats, 1), Sheets.WerSpreadSheetId, $"{houseNum}!M9:M{8 + numFlats}");

                //Change month
                await WriteInfoAsync(month, Sheets.WerSpreadSheetId, $"{houseNum}!I2:J2");


                // Change debit and credit
                var debitAndCredit2 = await ReadInfoAsync(Sheets.HeatingSpreadSheetId, $"{houseNum}!P9:Q{8 + numFlats}");
                await WriteInfoAsync(GetEmptyList(numFlats, 2), Sheets.HeatingSpreadSheetId, $"{houseNum}!D9:E{8 + numFlats}");
                await WriteInfoAsync(GetListDoubles(debitAndCredit2), Sheets.HeatingSpreadSheetId, $"{houseNum}!D9:E{8 + numFlats}");

                // Dublicate last values
                var lastValues2 = await ReadInfoAsync(Sheets.HeatingSpreadSheetId, $"{houseNum}!H9:H{8 + numFlats}");
                await WriteInfoAsync(GetListDoubles(lastValues2), Sheets.HeatingSpreadSheetId, $"{houseNum}!I9:I{8 + numFlats}");

                // Clear payments
                await WriteInfoAsync(GetEmptyList(numFlats, 2), Sheets.HeatingSpreadSheetId, $"{houseNum}!N9:O{8 + numFlats}");

                // Clear privileges
                await WriteInfoAsync(GetEmptyList(numFlats, 1), Sheets.HeatingSpreadSheetId, $"{houseNum}!L9:L{8 + numFlats}");

                //Change month
                await WriteInfoAsync(month, Sheets.HeatingSpreadSheetId, $"{houseNum}!I2:J2");
            }
            await WriteInfoAsync(month, Sheets.WerSpreadSheetId, $"Сводная ведомость!H2");
            await WriteInfoAsync(month, Sheets.HeatingSpreadSheetId, $"Сводная ведомость!H2");
        }

        private IList<IList<object>> GetListDoubles(IList<IList<object>> info)
        {
            for (int i = 0; i < info.Count; i++)
            {
                for (int j = 0; j < info[i].Count; j++)
                {
                    info[i][j] = GetNumber(info[i][j]);
                }
            }
            return info;
        }

        private IList<IList<object>> GetEmptyList(int rows, int colomns)
        {
            var list = new List<IList<object>>();
            for (int i = 0; i < rows; i++)
            {
                list.Add(new List<object>());
                for (int j = 0; j < colomns; j++)
                {
                    list[i].Add(0);
                }
            }
            return list;
        }

        private double GetNumber(object value)
        {
            return string.IsNullOrEmpty(value.ToString()) ?
                0.0 : Math.Round(double.Parse(value.ToString().Replace('.', ',')), 3);
        }

        private async Task<IList<IList<object>>> ReadInfoAsync(string spreadSheetId, string readRange)
        {
            var response = await _service.Spreadsheets.Values.Get(spreadSheetId, readRange)
                .ExecuteAsync();
            return response.Values;
        }

        private async Task WriteInfoAsync(IList<IList<object>> info, string spreadSheetId, string writeRange)
        {
            var valueRange = new ValueRange { Values = info };
            var update = _service.Spreadsheets.Values.Update(valueRange, spreadSheetId, writeRange);
            update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            await update.ExecuteAsync();
        }


        //


        public async Task<IList<IList<object>>> GetHouseInfoAsync(string houseNumber)
        {
            return (await _service.Spreadsheets.Values.Get(Sheets.ServiceSpreadSheetId,
                $"{houseNumber.Replace('_', '/')}!A1:AH97").ExecuteAsync()).Values;
        }

        public async Task<IList<object>> GetRatesAsync(string houseNumber)
        {
            var response = await _service.Spreadsheets.Values.Get(Sheets.ServiceSpreadSheetId, $"Rates!B2:H7").ExecuteAsync();
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
            int numMonth = Date.GetNumMonth(month);
            rates.Add(numMonth);
            rates.Add(numMonth + 1);

            return rates;
        }

        public IList<IList<object>> GetHouseInfo(string houseNumber)
        {
            lock (this)
            {
                return _service.Spreadsheets.Values.Get(Sheets.ServiceSpreadSheetId,
                    $"{houseNumber.Replace('_', '/')}!A1:AH97").Execute().Values;
            }
        }

        public IList<object> GetRates(string houseNumber)
        {
            var response = _service.Spreadsheets.Values.Get(Sheets.ServiceSpreadSheetId, $"Rates!B2:H7").Execute();
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
            int numMonth = Date.GetNumMonth(month);
            rates.Add(numMonth);
            rates.Add(numMonth + 1);

            return rates;
        }
    }
}
