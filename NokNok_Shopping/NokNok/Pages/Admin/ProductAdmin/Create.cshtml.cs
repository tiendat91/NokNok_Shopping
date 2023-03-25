using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NokNok_ShoppingAPI.Models;

namespace NokNok.Pages.Admin.ProductAdmin
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        private string CategoryApiUrl = "";
        public IList<Product> Products { get; set; }
        public IList<Category> Categories { get; set; }

        public CreateModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "http://localhost:5000/api/Products/GetProductById";
            CategoryApiUrl = "http://localhost:5000/api/Categories";
        }

        public async Task<IActionResult> OnGet()
        {
            HttpResponseMessage response = await client.GetAsync(CategoryApiUrl);
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Categories = JsonSerializer.Deserialize<List<Category>>(strData, options);

            ViewData["CategoryId"] = new SelectList(Categories, "CategoryId", "CategoryName");
            return Page();
        }

        [BindProperty]
        public Product Product { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || Product == null)
            {
                return Page();
            }

            string data = JsonSerializer.Serialize(Product);
            await client.PostAsync("http://localhost:5000/api/Products/CreateProduct", new StringContent(data, Encoding.UTF8, "application/json"));

            return RedirectToPage("./Index");
        }
    }
}
