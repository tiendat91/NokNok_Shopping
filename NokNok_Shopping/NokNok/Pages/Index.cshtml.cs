using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Models;
using System.Text.Json;
using static MyRazorPage.Pages.Account.CartModel;

namespace MyRazorPage.Pages
{
    public class IndexModel : PageModel
    {
        readonly PRN221DBContext dBContext;
        public IndexModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        [BindProperty]
        public List<Category> Category { get; set; }
        [BindProperty]
        public List<Models.Product> BestSeller { get; set; }
        [BindProperty]
        public List<Models.Product> NewProducts { get; set; }
        [BindProperty]
        public List<Models.Product> HotProducts { get; set; }
        public void OnGet()
        {
            //load category
            Category = dBContext.Categories.ToList();

            //Hot (4 sản phẩm có nhiều unitsInStock nhất)
            HotProducts = dBContext.Products.OrderByDescending(s => s.UnitsInStock).Take(4).ToList();

            //Best sell (4 sản phẩm nằm trong nhiều đơn hàng nhất)
            var x = (from od in dBContext.OrderDetails.ToList()
                     group od by od.ProductId into g
                     select new
                     {
                         ProductID = g.Key,
                         NumberProduct = g.Count()
                     }).OrderByDescending(s => s.NumberProduct).Take(4).ToList();
            List<Models.Product> temp = new List<Models.Product>();
            foreach (var item in x)
            {
                temp.Add(dBContext.Products.FirstOrDefault(s => s.ProductId == item.ProductID));
            }
            BestSeller = temp;
            //New Product (4 sản phẩm mới add vào bảng Product)
            NewProducts = dBContext.Products.OrderByDescending(s => s.ProductId).Take(4).ToList();

            //hiển thị số product trong cart, khởi đầu = 0
            if (HttpContext.Session.GetInt32("CountOrderCart") == null)
            {
                HttpContext.Session.SetInt32("CountOrderCart", 0);
            }
        }

        ///Click on BUY NOW
        public async Task<IActionResult> OnGetBuyNow(int? ProID)
        {
            //Add Product to cookie
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(30);
            int CountOrderCart;
            var jsonCarts = HttpContext.Request.Cookies["CartItemsAddToCart"];
            var pro = await dBContext.Products.SingleOrDefaultAsync(m => m.ProductId == ProID);

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

            HttpContext.Session.SetString("ProductIdOrdered" + ProID.ToString(), ProID.ToString());
            if (ProID == null || dBContext.Products == null)
            {
                return NotFound();
            }

            //string jsonCart = JsonSerializer.Serialize(Product);
            //Response.Cookies.Append("Cart", jsonCart);

            //hiện thi cart
            HttpContext.Session.SetInt32("CountOrderCart", CountOrderCart);

            ViewData["msg"] = "Add to cart susccesfully!";
            return RedirectToPage("/account/cart");
        }

    }
}
