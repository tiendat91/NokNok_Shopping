using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NokNok_ShoppingAPI.DAO;

namespace NokNok_ShoppingAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Department : ControllerBase
    {
        [HttpGet]
        public IActionResult GetDepartments()
        {
            var cats = DepartmentDAO.GetDepartments();
            if (cats == null)
            {
                return NotFound();
            }
            return Ok(cats);
        }
    }
}
