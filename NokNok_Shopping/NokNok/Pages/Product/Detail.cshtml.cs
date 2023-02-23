using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static MyRazorPage.Pages.Account.CartModel;

namespace MyRazorPage.Pages.Product
{
    public class DetailModel : PageModel
    {
        private readonly MyRazorPage.Models.PRN221DBContext _context;

        public DetailModel(MyRazorPage.Models.PRN221DBContext context)
        {
            _context = context;
        }
        [BindProperty]
        public Models.Product Product { get; set; }
        [BindProperty]
        public string msg { get; set; }
        [BindProperty]
        public string SelectedImg { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, string? SelectedImg)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }
            if (SelectedImg == null)
            {
                SelectedImg = "p1.jpeg";
            }

            var product = await _context.Products.Include(s => s.Category).FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }
            else
            {

                Product = product;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? ProID)
        {
            //Add Product to cookie
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(30);
            int CountOrderCart;
            var jsonCarts = HttpContext.Request.Cookies["CartItemsAddToCart"];
            var pro = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == ProID);

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

            Product = await _context.Products.SingleOrDefaultAsync(m => m.ProductId == ProID);

            HttpContext.Session.SetString("ProductIdOrdered" + ProID.ToString(), ProID.ToString());
            if (ProID == null || _context.Products == null)
            {
                return NotFound();
            }

            //hiện thi cart
            HttpContext.Session.SetInt32("CountOrderCart", CountOrderCart);

            ViewData["msg"] = "Add to cart susccesfully!";
            return Page();
        }
    }
}
