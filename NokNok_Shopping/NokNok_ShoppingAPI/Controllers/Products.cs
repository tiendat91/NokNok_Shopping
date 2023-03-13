using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NokNok_ShoppingAPI.DAO;
using NokNok_ShoppingAPI.DTO;
using NokNok_ShoppingAPI.Models;
using PE_PRN231_Sum22B1.DTO;

namespace NokNok_ShoppingAPI.Controllers
{
    [Route("api/[controller]")]
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
            if (product ==  null)
            {
                ProductsDAO.CreateProduct(product);
                return Ok(product);
            }
            return NotFound();
        }

        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts()
        {
            //USING MAPPER
            //List<ProductDTO> productDTOs;
            //productDTOs = context.Products.ProjectTo<ProductDTO>(config).ToList();

            var products = ProductsDAO.GetProducts();
            if (products == null)
            {
                return NotFound();
            }
            return Ok(products);
        }

        [HttpPut]
        public IActionResult UpdateProduct([FromBody] Product product)
        {
            var p = ProductsDAO.GetProductById(product.ProductId);
            if (product == null)
            {
                return NotFound();
            }

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
