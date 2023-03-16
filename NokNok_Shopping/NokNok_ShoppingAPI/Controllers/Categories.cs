using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NokNok_ShoppingAPI.DAO;
using NokNok_ShoppingAPI.Models;

namespace NokNok_ShoppingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Categories : Controller
    {
        [HttpGet]
        public IActionResult GetCategories()
        {
            var cats = CategoryDAO.GetCategories();
            if (cats == null)
            {
                return NotFound();
            }
            return Ok(cats);
        }
    }

}
