using BankServer.Model;
using BankServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("")]
        public IEnumerable<Account> GetAll()
        {
            return _accountService.GetAll();
        }

        [HttpGet("{accountId}")]
        public Account Get(int accountId)
        {
            return _accountService.Get(accountId);
        }

        [HttpPost("")]
        public ActionResult<Account> Add(Account account)
        {
            var created = _accountService.Add(account);

            return CreatedAtAction(nameof(Get), new { accountId = created.Id }, created);
        }

        [HttpPatch("{accountId}/credit/{amount}")]
        public Account Credit(int accountId, decimal amount)
        {
            return _accountService.Credit(accountId, amount);
        }

        [HttpPatch("{accountId}/charge/{amount}")]
        public Account Charge(int accountId, decimal amount)
        {
            return _accountService.Charge(accountId, amount);
        }

        [HttpDelete("{accountId}")]
        public IActionResult Delete(int accountId)
        {
            _accountService.Delete(accountId);

            return NoContent();
        }

        [HttpDelete("")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult DeleteAll()
        {
            _accountService.DeleteAll();

            return NoContent();
        }

        private readonly IAccountService _accountService;
    }
}
