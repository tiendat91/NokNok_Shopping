using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NokNok_ShoppingAPI.Controllers;
using NokNok_ShoppingAPI.Models;

namespace NokNok.Pages.Admin.OrderAdmin
{
    [Authorize(Roles = "1")]
    public class IndexModel : PageModel
    {
        private readonly HttpClient client = null;
        private string OrderApiUrl = "";
        private string CustomerApiUrl = "";
        public IList<Order> Order { get; set; }
        public IList<Customer> Customers { get; set; }

        public IndexModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            OrderApiUrl = "http://localhost:5000/api/Orders/GetOrders";
            CustomerApiUrl = "http://localhost:5000/api/Customers/GetCustomers";
        }


        public async Task OnGetAsync()
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                HttpResponseMessage response = await client.GetAsync(OrderApiUrl);
                string strData = await response.Content.ReadAsStringAsync();
                Order = JsonSerializer.Deserialize<List<Order>>(strData, options).AsQueryable().OrderByDescending(s=>s.OrderDate).ToList();

                HttpResponseMessage responseC = await client.GetAsync(CustomerApiUrl);
                string strDataC = await responseC.Content.ReadAsStringAsync();
                Customers = JsonSerializer.Deserialize<List<Customer>>(strDataC, options);

                foreach (var item in Order)
                {
                    item.Customer = Customers.AsQueryable().FirstOrDefault(s => s.CustomerId == item.CustomerId);
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
