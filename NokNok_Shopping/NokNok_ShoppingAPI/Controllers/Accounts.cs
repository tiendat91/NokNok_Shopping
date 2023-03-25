using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NokNok_ShoppingAPI.DAO;
using NokNok_ShoppingAPI.Models;

namespace NokNok_ShoppingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Accounts : ControllerBase
    {
        [HttpGet]
        public IActionResult GetAccounts()
        {
            var cats = AccountsDAO.GetAccounts();
            if (cats == null)
            {
                return NotFound();
            }
            return Ok(cats);
        }

        [HttpPost("CreateAccount")]
        public IActionResult CreateAccount([FromBody] Account account)
        {
            //Validate data
            var p = AccountsDAO.GetAccountById(account.AccountId);
            if (p == null)
            {
                AccountsDAO.CreateAccount(account);
                return Ok(account);
            }
            return NotFound();
        }
    }
}
