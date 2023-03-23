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
    public class Customers : ControllerBase
    {
        private readonly NokNok_ShoppingContext context = new NokNok_ShoppingContext();
        private MapperConfiguration config;
        private IMapper mapper;
        public Customers(NokNok_ShoppingContext _context)
        {
            context = _context;
            config = new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfile()));
            mapper = config.CreateMapper();
        }

        [HttpPost]
        public IActionResult CreateCustomer([FromBody] Customer customer)
        {
            //Validate data
            var p = CustomersDAO.GetCustomerById(customer.CustomerId);
            if (p == null)
            {
                CustomersDAO.CreateCustomer(customer);
                return Ok(customer);
            }
            return NotFound();
        }

        [HttpGet]
        [EnableQuery()]
        public IActionResult GetCustomers()
        {
            var customers = CustomersDAO.GetCustomers();
            if (customers == null)
            {
                return NotFound();
            }
            return Ok(customers);
        }
        [HttpGet("{id}")]
        public IActionResult GetCustomerById([FromRoute] string id)
        {
            var customers = CustomersDAO.GetCustomerById(id);
            if (customers == null)
            {
                return NotFound();
            }
            return Ok(customers);
        }

        [HttpPut]
        public IActionResult UpdateCustomer([FromBody] Customer customer)
        {
            var p = CustomersDAO.GetCustomerById(customer.CustomerId);
            if (p == null)
            {
                return NotFound();
            }
            CustomersDAO.UpdateCustomer(customer);
            return Ok(customer);
        }

        [HttpDelete("{cusId}")]
        public IActionResult DeleteCustomer([FromRoute] string cusId)
        {
            try
            {
                var cus = CustomersDAO.GetCustomerById(cusId);
                if (cus == null)
                {
                    return NotFound();
                }
                CustomersDAO.DeleteCustomer(cus);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
