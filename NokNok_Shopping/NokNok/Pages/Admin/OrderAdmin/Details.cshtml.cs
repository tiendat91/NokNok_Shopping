using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using NokNok_ShoppingAPI.Models;

namespace NokNok.Pages.Admin.OrderAdmin
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient client = null;
        private string OrderApiUrl = "";
        private string ProductApiUrl = "";
        public IList<Product> Products { get; set; }
        public DetailsModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            OrderApiUrl = "http://localhost:5000/api/Orders/GetOrderById";
            ProductApiUrl = "http://localhost:5000/api/Products/GetAllProducts";
        }
        [BindProperty]
        public Order Order { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            HttpResponseMessage response = await client.GetAsync(OrderApiUrl + $"/{id}");
            string strData = await response.Content.ReadAsStringAsync();
            var order = JsonSerializer.Deserialize<Order>(strData, options);

            //PRODUCT
            HttpResponseMessage responseP = await client.GetAsync(ProductApiUrl);
            string strDataP = await responseP.Content.ReadAsStringAsync();
            Products = JsonSerializer.Deserialize<List<Product>>(strDataP, options);

            if (order == null)
            {
                return NotFound();
            }
            else
            {
                Order = order;
                foreach (var item in order.OrderDetails)
                {
                    var pro = Products.FirstOrDefault(x => x.ProductId == item.ProductId);
                    if (pro != null)
                    {
                        item.Product = pro;
                    }
                }
            }
            return Page();
        }
    }
}
