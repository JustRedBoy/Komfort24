using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
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

        /// <summary>
        /// Get information from a range in a spreadsheet
        /// </summary>
        /// <param name="spreadSheetId">Source spreadsheet ID</param>
        /// <param name="readRange">Range with information</param>
        /// <returns>Requested information</returns>
        /// <exception cref="AccessDeniedException">Thrown when had problem with there was an access problem with Google Drive</exception>
        private async Task<IList<IList<object>>> ReadInfoAsync(string spreadSheetId, string readRange)
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

        /// <summary>
        /// Write information to a range in a spreadsheet
        /// </summary>
        /// <param name="info">Information to write</param>
        /// <param name="spreadSheetId">Source spreadsheet ID</param>
        /// <param name="writeRange">Range to write</param>
        /// <exception cref="AccessDeniedException">Thrown when had problem with there was an access problem with Google Drive</exception>
        private async Task WriteInfoAsync(IList<IList<object>> info, string spreadSheetId, string writeRange)
        {
            try
            {
                var valueRange = new ValueRange { Values = info };
                var update = _service.Spreadsheets.Values.Update(valueRange, spreadSheetId, writeRange);
                update.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                await update.ExecuteAsync();
            }
            catch (Exception e)
            {
                throw AccessDeniedException.CreateException(e);
            }
        }

        /// <summary>
        /// Get information about house
        /// </summary>
        /// <returns>Accounts information</returns>
        /// <exception cref="AccessDeniedException">Thrown when had problem with there was an access problem with Google Drive</exception>
        public async Task<IList<IList<object>>> GetAccountsInfoAsync()
        {
            return await ReadInfoAsync(Sheets.ServiceSpreadSheetId, "Houses!A1:AG1000");
        }

        /// <summary>
        /// Get information about rates
        /// </summary>
        /// <returns>Rates information</returns>
        /// <exception cref="AccessDeniedException">Thrown when had problem with there was an access problem with Google Drive</exception>
        public async Task<IList<IList<object>>> GetRatesInfoAsync()
        {
            return await ReadInfoAsync(Sheets.ServiceSpreadSheetId, "Rates!B2:G10");
        }

        /// <summary>
        /// Get information about archive reports
        /// </summary>
        /// <returns>All archive reports</returns>
        /// <exception cref="AccessDeniedException">Thrown when had problem with there was an access problem with Google Drive</exception>
        public async Task<IList<IList<object>>> GetArchiveReportsInfoAsync()
        {
            return await ReadInfoAsync(Sheets.ReportsSpreadSheetId, "Info!A1:AH250000");
        }

        /// <summary>
        /// Update information about archive reports
        /// </summary>
        /// <param name="reports">All reports</param>
        /// <exception cref="AccessDeniedException">Thrown when had problem with there was an access problem with Google Drive</exception>
        public async Task UpdateArchiveReportsInfoAsync(IList<IList<object>> reports)
        {
            await WriteInfoAsync(reports, Sheets.ReportsSpreadSheetId, $"Info!A1:AH{reports.Count}");
        }

        /// <summary>
        /// Update information about house
        /// </summary>
        /// <param name="houseShortAdress">Short adress of house</param>
        /// <param name="heating">List of values for heating document</param>
        /// <param name="wer">List of values for wer document</param>
        /// <param name="month">Currect month for write</param>
        /// <exception cref="AccessDeniedException">Thrown when had problem with there was an access problem with Google Drive</exception>
        public async Task UpdateHouseInfoAsync(string houseShortAdress, IList<IList<object>> heating, 
            IList<IList<object>> wer, IList<IList<object>> month)
        {
            int offset = 9;
            await WriteInfoAsync(heating, Sheets.HeatingSpreadSheetId, $"{houseShortAdress}!D{offset}:Q{heating.Count + offset}");
            await WriteInfoAsync(month, Sheets.HeatingSpreadSheetId, $"{houseShortAdress}!I2:J2");
            await WriteInfoAsync(wer, Sheets.WerSpreadSheetId, $"{houseShortAdress}!D{offset}:R{wer.Count + offset}");
            await WriteInfoAsync(month, Sheets.WerSpreadSheetId, $"{houseShortAdress}!I2:J2");
        }

        /// <summary>
        /// Update month in pivot tables
        /// </summary>
        /// <param name="monthAndYear">Currect month and year for write</param>
        /// <exception cref="AccessDeniedException">Thrown when had problem with there was an access problem with Google Drive</exception>
        public async Task UpdateGeneralMonthAsync(IList<IList<object>> monthAndYear)
        {
            await WriteInfoAsync(monthAndYear, Sheets.HeatingSpreadSheetId, "Сводная ведомость!H2:I2");
            await WriteInfoAsync(monthAndYear, Sheets.WerSpreadSheetId, "Сводная ведомость!H2:I2");
        }
    }
}
