﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NokNok_ShoppingAPI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NokNok.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        private string CategoryApiUrl = "";
        [BindProperty]
        public List<Product> Products { get; set; }
        [BindProperty]
        public List<Product> PopularProducts { get; set; }
        [BindProperty]
        public List<Category> Categories { get; set; }

        public IndexModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "http://localhost:5000/api/Products/GetAllProducts";
            CategoryApiUrl = "http://localhost:5000/api/Categories";
        }

        public async Task OnGet()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            //PRODUCT
            HttpResponseMessage response = await client.GetAsync(ProductApiUrl);
            string strData = await response.Content.ReadAsStringAsync();
            Products = JsonSerializer.Deserialize<List<Product>>(strData, options);

            //POPULAR PRODUCT
            PopularProducts = Products.AsQueryable().OrderByDescending(s=>s.UnitPrice).Take(5).ToList();

            //CATEGORY
            HttpResponseMessage responseC = await client.GetAsync(CategoryApiUrl);
            string strDataC = await responseC.Content.ReadAsStringAsync();
            Categories = JsonSerializer.Deserialize<List<Category>>(strDataC, options);
        }
    }
}