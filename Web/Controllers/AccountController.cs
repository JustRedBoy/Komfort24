using Microsoft.AspNetCore.Mvc;
using Models;
using Web.Models;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationContext _appContext;

        public AccountController(ApplicationContext appContext)
        {
            _appContext = appContext;
        }

        [HttpGet]
        public Account GetAccount(string accountId)
        {
            return _appContext?.Service?.GetAccountById(accountId);
        }
    }
}