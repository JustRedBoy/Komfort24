using Microsoft.AspNetCore.Mvc;
using Models;
using Tools;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ServiceContext _serviceContext;

        public AccountController(ServiceContext serviceContext)
        {
            _serviceContext = serviceContext;
        }

        [HttpGet]
        public Account GetAccount(string accountId)
        {
            if (Matching.IsAccountId(accountId))
            {
                Account account = _serviceContext.GetAccountById(accountId);
                account.House.ClearAccounts();
                return account;
            }
            return null;
        }
    }
}
