using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NokNok_ShoppingAPI.DAO;

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
    }
}
