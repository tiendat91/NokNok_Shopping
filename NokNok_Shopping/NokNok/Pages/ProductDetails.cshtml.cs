using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NokNok_ShoppingAPI.Controllers;
using NokNok_ShoppingAPI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NokNok.Pages
{
    public class ProductDetailsModel : PageModel
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        public Product Product { get; set; }

        public ProductDetailsModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "http://localhost:5000/api/Products/GetProductById";
        }
        public async Task OnGet(int? proId)
        {
            if(proId == null)
            {
                return;
            }
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            HttpResponseMessage response = await client.GetAsync(ProductApiUrl+$"/{proId}");
            string strData = await response.Content.ReadAsStringAsync();
            Product = JsonSerializer.Deserialize<Product>(strData,options);
        }
    }
}
