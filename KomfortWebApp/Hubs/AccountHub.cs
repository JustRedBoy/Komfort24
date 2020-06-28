using KomfortWebApp.Models;
using KomfortWebApp.Services;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace KomfortWebApp.Hubs
{
    public class AccountHub : Hub
    {
        private readonly GoogleSheets _googleSheets;

        public AccountHub(GoogleSheets googleSheets)
        {
            _googleSheets = googleSheets;
        }

        //refactor
        public async Task GetAccountInfo(string accountId)
        {
            try
            {
                Account account = await _googleSheets.GetAccountAsync(accountId);
                Rates rates = await _googleSheets.GetRatesAsync(account.HouseId);
                await Clients.Caller.SendAsync("ReceiveAccountInfo",
                    new
                    {
                        account.WerStateStart,
                        account.WerForMonth,
                        account.WerPayment,
                        account.WaterCurrentValue,
                        account.WaterPayment,
                        rates.WaterRate,
                        account.HeatingStateStart,
                        account.HeatingCurrentValue,
                        HeatingRate = account.Type != Account.HeatingType.Simple ?
                            rates.CustomHeatingRate * account.GetHeatingCoefficient() : 0,
                        HeatingForMonth = account.Type == Account.HeatingType.Simple ?
                            account.HeatingSquare * rates.CentralHeatingRate : 0,
                        account.HeatingPayment
                    });
            }
            catch (ArgumentException)
            {
                await Clients.Caller.SendAsync("ReceiveAccountInfo", null);
                return;
            }
        }
    }
}
