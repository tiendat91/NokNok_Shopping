using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NokNok_ShoppingAPI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NokNok.Pages.User.OrderHistory
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient client = null;
        private string OrderApiUrl = "";
        private string CustomerApiUrl = "";
        private string ProductApiUrl = "";
        private string OrderDetailApiUrl = "";
        [BindProperty]
        public IList<Order> Order { get; set; }
        public IList<Product> Products { get; set; }
        public IList<OrderDetail> OrderDetails { get; set; }
        [BindProperty]
        public Customer Customer { get; set; }
        public IndexModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            OrderApiUrl = "http://localhost:5000/api/Orders/GetOrders";
            CustomerApiUrl = "http://localhost:5000/api/Customers/GetCustomerById";
            ProductApiUrl = "http://localhost:5000/api/Products/GetAllProducts";
            OrderDetailApiUrl = "http://localhost:5000/api/Products/GetAllProducts";
        }


        public async Task<IActionResult> OnGetAsync()
        {

            try
            {
                var customerId = HttpContext.Session.GetString("CustomerID");
                if (customerId == null)
                { 
                    return NotFound();
                }

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                HttpResponseMessage response = await client.GetAsync(OrderApiUrl);
                string strData = await response.Content.ReadAsStringAsync();
                Order = JsonSerializer.Deserialize<List<Order>>(strData, options).AsQueryable().OrderByDescending(s => s.OrderDate).Where(s=>s.CustomerId == customerId).ToList();

                HttpResponseMessage responseC = await client.GetAsync(CustomerApiUrl+$"/{customerId}");
                string strDataC = await responseC.Content.ReadAsStringAsync();
                Customer = JsonSerializer.Deserialize<Customer>(strDataC, options);

                HttpResponseMessage responseP = await client.GetAsync(ProductApiUrl);
                string strDataP = await responseP.Content.ReadAsStringAsync();
                Products = JsonSerializer.Deserialize<List<Product>>(strDataP, options);

                foreach (var order in Order)
                {
                    foreach (var od in order.OrderDetails)
                    {
                        od.Product = Products.AsQueryable().FirstOrDefault(s=>s.ProductId == od.ProductId);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return Page();
        }
    }
}
