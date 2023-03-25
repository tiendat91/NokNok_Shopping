using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NokNok_ShoppingAPI.Models;
using System.Net.Http.Headers;
using System.Text.Json;
using static NokNok.Pages.CartModel;

namespace NokNok.Pages
{
    public class ProductDetailsModel : PageModel
    {
        private readonly HttpClient client = null;
        private string ProductApiUrl = "";
        [BindProperty]
        public Product Product { get; set; }
        [BindProperty]
        public List<Product> Products { get; set; }

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

        public async Task<IActionResult> OnPost(int? ProID)
        {
            //Add Product to cookie
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(30);
            int CountOrderCart;
            var jsonCarts = HttpContext.Request.Cookies["CartItemsAddToCart"];

            HttpResponseMessage response = await client.GetAsync("http://localhost:5000/api/Products/GetAllProducts");
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Products = JsonSerializer.Deserialize<List<Product>>(strData, options);

            HttpResponseMessage responseP = await client.GetAsync(ProductApiUrl + $"/{ProID}");
            string strDataP = await responseP.Content.ReadAsStringAsync();
            Product = JsonSerializer.Deserialize<Product>(strDataP, options);

            Product pro = Product;

            if (jsonCarts != null)
            {

                var listjsonCarts = JsonSerializer.Deserialize<List<CartItems>>(jsonCarts);
                bool duplicatedId = false;
                //check trung ID, tang so luong
                foreach (var item in listjsonCarts.ToList())
                {
                    if (item.product.ProductId == ProID)
                    {
                        item.quantity += 1;
                        duplicatedId = true;
                    }
                }
                if (duplicatedId == false)
                {
                    listjsonCarts.Add(new CartItems { product = pro, quantity = 1 });
                }
                Response.Cookies.Append("CartItemsAddToCart", JsonSerializer.Serialize<List<CartItems>>(listjsonCarts), option);
                CountOrderCart = listjsonCarts.Count();
            }
            else
            {
                List<CartItems> listjsonCarts = new List<CartItems> { };
                listjsonCarts.Add(new CartItems { product = pro, quantity = 1 });
                Response.Cookies.Append("CartItemsAddToCart", JsonSerializer.Serialize<List<CartItems>>(listjsonCarts), option);
                CountOrderCart = listjsonCarts.Count();
            }

            Product = pro;

            HttpContext.Session.SetString("ProductIdOrdered" + ProID.ToString(), ProID.ToString());
            if (ProID == null)
            {
                return NotFound();
            }

            //string jsonCart = JsonSerializer.Serialize(Product);
            //Response.Cookies.Append("Cart", jsonCart);

            //hiện thi cart
            HttpContext.Session.SetInt32("CountOrderCart", CountOrderCart);

            ViewData["msg"] = "Add to cart susccesfully!";
            return Page();
        }
    }
}
