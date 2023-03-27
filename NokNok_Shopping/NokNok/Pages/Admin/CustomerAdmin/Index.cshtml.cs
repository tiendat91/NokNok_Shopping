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
using NokNok_ShoppingAPI.Models;

namespace NokNok.Pages.Admin.CustomerAdmin
{
    [Authorize(Roles = "1")]
    public class IndexModel : PageModel
    {
        private readonly HttpClient client = null;
        private string CustomerApiUrl = "";
        public IList<Customer> Customers { get; set; }
        public IndexModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            CustomerApiUrl = "http://localhost:5000/api/Customers/GetCustomers";
        }

        public IList<Customer> Customer { get;set; } = default!;

        public async Task OnGetAsync()
        {
            HttpResponseMessage response = await client.GetAsync(CustomerApiUrl);
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Customers = JsonSerializer.Deserialize<List<Customer>>(strData, options).AsQueryable().OrderByDescending(s=>s.CreatedDate).ToList();
        }
    }
}
