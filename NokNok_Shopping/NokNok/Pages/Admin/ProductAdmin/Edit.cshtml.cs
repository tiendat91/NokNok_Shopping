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
using Microsoft.EntityFrameworkCore;
using NokNok_ShoppingAPI.Controllers;
using NokNok_ShoppingAPI.Models;

namespace NokNok.Pages.Admin.ProductAdmin
{
    public class EditModel : PageModel
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        private string CategoryApiUrl = "";
        public IList<Product> Products { get; set; }
        public IList<Category> Categories { get; set; }

        public EditModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "http://localhost:5000/api/Products/GetProductById";
            CategoryApiUrl = "http://localhost:5000/api/Categories";
        }

        [BindProperty]
        public Product Product { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpResponseMessage response = await client.GetAsync($"http://localhost:5000/api/Products/GetProductById/{id}");
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Product = JsonSerializer.Deserialize<Product>(strData, options);

            HttpResponseMessage responseC = await client.GetAsync(CategoryApiUrl);
            string strDataC = await responseC.Content.ReadAsStringAsync();
            Categories = JsonSerializer.Deserialize<List<Category>>(strDataC, options);

            if (Product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(Categories, "CategoryId", "CategoryName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                string data = JsonSerializer.Serialize(Product);
                var response = await client.PutAsync("http://localhost:5000/api/Products/UpdateProduct", new StringContent(data, Encoding.UTF8, "application/json"));
            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToPage("./Index");
        }

    }
}
