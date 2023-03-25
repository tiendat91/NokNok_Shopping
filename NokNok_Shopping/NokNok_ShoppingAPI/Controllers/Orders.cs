using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using NokNok_ShoppingAPI.DAO;
using NokNok_ShoppingAPI.Models;
using NokNok_ShoppingAPI.DTO;
using System.Reflection;
using PE_PRN231_Sum22B1.DTO;
using AutoMapper.QueryableExtensions;

namespace NokNok_ShoppingAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class Orders : ControllerBase
    {
        private readonly NokNok_ShoppingContext context = new NokNok_ShoppingContext();
        private MapperConfiguration config;
        private IMapper mapper;
        public Orders(NokNok_ShoppingContext _context)
        {
            context = _context;
            config = new MapperConfiguration(cfg => cfg.AddProfile(new MapperProfile()));
            mapper = config.CreateMapper();
        }

        [HttpPost]
        public IActionResult CreateOrder([FromBody] Order order)
        {
            //Validate data
            var p = OrdersDAO.GetOrderById(order.OrderId);
            if (p == null)
            {
                OrdersDAO.CreateOrder(order);
                return Ok(order);
            }
            return NotFound();
        }

        [HttpPost]
        public IActionResult CreateOrderDetail([FromBody] OrderDetail orderDetail)
        {
                OrderDetailsDAO.CreateOrderDetail(orderDetail);
                return Ok(orderDetail);
        }

        [HttpGet]
        [EnableQuery()]
        public IActionResult GetOrders()
        {
            var orders = OrdersDAO.GetOrders();
            if (orders == null)
            {
                return NotFound();
            }
            return Ok(orders);
        }
        [HttpGet("{id}")]
        public IActionResult GetOrderById([FromRoute] int id)
        {
            var order = OrdersDAO.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            return Ok(order);
        }

        [HttpPut]
        public IActionResult UpdateOrder([FromBody] Order order)
        {
            var p = OrdersDAO.GetOrderById(order.OrderId);
            if (p == null)
            {
                return NotFound();
            }
            OrdersDAO.UpdateOrder(order);
            return Ok(p);
        }

        [HttpDelete("{orderId}")]
        public IActionResult DeleteOrder([FromRoute] int orderId)
        {
            try
            {
                var cus = OrdersDAO.GetOrderById(orderId);
                if (cus == null)
                {
                    return NotFound();
                }
                OrdersDAO.DeleteOrder(cus);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
