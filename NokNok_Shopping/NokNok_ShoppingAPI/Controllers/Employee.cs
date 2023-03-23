using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using NokNok_ShoppingAPI.DAO;
using NokNok_ShoppingAPI.Models;
using PE_PRN231_Sum22B1.DTO;

namespace NokNok_ShoppingAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Employee : ControllerBase
    {
        private readonly NokNok_ShoppingContext context = new NokNok_ShoppingContext();
        private MapperConfiguration config;
        private IMapper mapper;
        public Employee(NokNok_ShoppingContext _context)
        {
            context = _context;
            config = new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfile()));
            mapper = config.CreateMapper();
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Models.Employee e)
        {
            //Validate data
            var p = EmployeeDAO.GetEmployeeById(e.EmployeeId);
            if (p == null)
            {
                EmployeeDAO.CreateEmployee(e);
                return Ok(e);
            }
            return NotFound();
        }

        [HttpGet]
        [EnableQuery()]
        public IActionResult GetEmployees()
        {
            var employees = EmployeeDAO.GetEmployees();
            if (employees == null)
            {
                return NotFound();
            }
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public IActionResult GetEmployeeById([FromRoute] int id)
        {
            var employee = EmployeeDAO.GetEmployeeById(id);
            if (employee == null)
            {
                return NotFound();
            }
            return Ok(employee);
        }

        [HttpPut]
        public IActionResult UpdateEmployee([FromBody] Models.Employee employee)
        {
            var p = EmployeeDAO.GetEmployeeById(employee.EmployeeId);
            if (p == null)
            {
                return NotFound();
            }
            EmployeeDAO.UpdateEmployee(employee);
            return Ok(employee);
        }

        [HttpDelete("{cusId}")]
        public IActionResult DeleteEmployee([FromRoute] int id)
        {
            try
            {
                var em = EmployeeDAO.GetEmployeeById(id);
                if (em == null)
                {
                    return NotFound();
                }
                EmployeeDAO.DeleteEmployee(em);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
