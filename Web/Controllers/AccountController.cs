using Microsoft.AspNetCore.Mvc;
using Models;

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
            return _serviceContext.GetAccountById(accountId);
        }
    }
}
