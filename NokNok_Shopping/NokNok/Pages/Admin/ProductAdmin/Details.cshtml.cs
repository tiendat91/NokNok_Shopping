using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NokNok_ShoppingAPI.Models;

namespace NokNok.Pages.Admin.ProductAdmin
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        private string CategoryApiUrl = "";
        public IList<Product> Products { get; set; }
        public IList<Category> Categories { get; set; }

        public DetailsModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "http://localhost:5000/api/Products/GetProductById";
            CategoryApiUrl = "http://localhost:5000/api/Categories";
        }

        public Product Product { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpResponseMessage response = await client.GetAsync(ProductApiUrl + $"/{id}");
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Product = JsonSerializer.Deserialize<Product>(strData, options);

            if (Product == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
