using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using NokNok_ShoppingAPI.DAO;
using NokNok_ShoppingAPI.DTO;
using NokNok_ShoppingAPI.Models;
using PE_PRN231_Sum22B1.DTO;

namespace NokNok_ShoppingAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Products : ControllerBase
    {
        private readonly NokNok_ShoppingContext context = new NokNok_ShoppingContext();
        private MapperConfiguration config;
        private IMapper mapper;
        public Products(NokNok_ShoppingContext _context)
        {
            context = _context;
            config = new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfile()));
            mapper = config.CreateMapper();
        }

        [HttpPost]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            //Validate data
            var p = ProductsDAO.GetProductById(product.ProductId);
            if (p ==  null)
            {
                ProductsDAO.CreateProduct(product);
                return Ok(product);
            }
            return NotFound();
        }

        [HttpGet]
        [EnableQuery()]
        public IActionResult GetAllProducts()
        {

            var products = ProductsDAO.GetProducts();
            if (products == null)
            {
                return NotFound();
            }
            return Ok(products);
        }
        [HttpGet("{id}")]
        public IActionResult GetProductById([FromRoute]int id)
        {
            var product = ProductsDAO.GetProductById(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPut]
        public IActionResult UpdateProduct([FromBody] Product product)
        {
            var p = ProductsDAO.GetProductById(product.ProductId);
            if (p == null)
            {
                return NotFound();
            }
            product.Discontinued = true;
            ProductsDAO.UpdateProduct(product);
            return Ok(product);
        }

        [HttpDelete("{productId}")]
        public IActionResult DeleteProduct([FromRoute]int productId)
        {
            try
            {
                var cus = ProductsDAO.GetProductById(productId);
                if (cus == null)
                {
                    return NotFound();
                }
                ProductsDAO.DeleteProduct(cus);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
