using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        [BindProperty]
        public string SelectedOrder { get; set; }
        public SelectList OrderOption { get; set; }

        public IEnumerable<OrderOptions> myEnumerable = new List<OrderOptions>
            {
                new OrderOptions { Id = 1, Value = "Sort by price: low to high" },
                new OrderOptions { Id = 2, Value = "Sort by price: high to low" },
                new OrderOptions { Id = 3, Value = "Product Name: A - Z" }
            };

        public IndexModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            ProductApiUrl = "http://localhost:5000/api/Products/GetAllProducts";
            CategoryApiUrl = "http://localhost:5000/api/Categories";
        }

        public class OrderOptions
        {
            public int Id { get; set; }
            public string Value { get; set;}
        }

        public async Task OnGet(string? SelectedOrder, string? search, string? category)
        {

            OrderOption = new SelectList(myEnumerable,"Id","Value",OrderOption);

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            //POPULAR PRODUCT
            HttpResponseMessage responsePopularProducts = await client.GetAsync(ProductApiUrl);
            string strDataPopularProducts = await responsePopularProducts.Content.ReadAsStringAsync();
            PopularProducts = JsonSerializer.Deserialize<List<Product>>(strDataPopularProducts, options).AsQueryable().OrderByDescending(s => s.UnitPrice).Take(5).ToList();

            //FILTER USING ODATA
            try
            {
                if (!String.IsNullOrEmpty(SelectedOrder))
                {
                    SelectedOrder = SelectedOrder;
                    switch (Int32.Parse(SelectedOrder))
                    {
                        case 0://Display all
                            break;
                        case 1://Sort by price: low to high
                            ProductApiUrl += "?$orderby=unitPrice asc";
                            break;
                        case 2://Sort by price: high to low
                            ProductApiUrl += "?$orderby=unitPrice desc";
                            break;
                        case 3://Product Name: A - Z
                            ProductApiUrl += "?$orderby=productName asc";
                            break;
                        default:
                            break;
                    }
                }

                if (!String.IsNullOrEmpty(search))
                {
                    ProductApiUrl += $"?$filter=contains(tolower(productName),%27{search.ToLower().Trim()}%27)";
                }

                if(!String.IsNullOrEmpty(category))
                {
                    ProductApiUrl += $"?$filter=categoryId eq {Int32.Parse(category)}";
                }

                //PRODUCT
                HttpResponseMessage response = await client.GetAsync(ProductApiUrl);
                string strData = await response.Content.ReadAsStringAsync();
                Products = JsonSerializer.Deserialize<List<Product>>(strData, options);


                //CATEGORY
                HttpResponseMessage responseC = await client.GetAsync(CategoryApiUrl);
                string strDataC = await responseC.Content.ReadAsStringAsync();
                Categories = JsonSerializer.Deserialize<List<Category>>(strDataC, options);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}