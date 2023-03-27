using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NokNok_ShoppingAPI.Models;

namespace NokNok.Pages.Admin.ProductAdmin
{
    [Authorize(Roles = "1")]
    public class IndexModel : PageModel
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        private string CategoryApiUrl = "";
        public IList<Product> Products { get; set; }
        public IList<Category> Categories { get; set; }

        public IndexModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "http://localhost:5000/api/Products/GetAllProducts";
            CategoryApiUrl = "http://localhost:5000/api/Categories";
        }


        public IList<Product> Product { get;set; } = default!;

        public async Task OnGetAsync()
        {
            HttpResponseMessage response = await client.GetAsync(ProductApiUrl);
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Products = JsonSerializer.Deserialize<List<Product>>(strData, options).AsQueryable().OrderByDescending(s=>s.ProductId).ToList();
        }
    }
}
