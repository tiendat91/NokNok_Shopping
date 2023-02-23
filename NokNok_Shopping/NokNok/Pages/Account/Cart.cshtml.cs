using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Text.Json;

namespace MyRazorPage.Pages.Account
{
    public class CartModel : PageModel
    {
        private readonly MyRazorPage.Models.PRN221DBContext _context;

        public CartModel(MyRazorPage.Models.PRN221DBContext context)
        {
            _context = context;
        }

        public class CartItems
        {
            public int quantity { get; set; }
            public Models.Product product { get; set; }
        }
        [BindProperty]
        public Models.OrderDetail OrderDetail { get; set; }
        [BindProperty]
        public Models.Customer Customer { get; set; }

        [BindProperty]
        public List<Models.OrderDetail> OrderDetails { get; set; }
        [BindProperty]
        public List<CartItems> ListCarItems { get; set; }
        [BindProperty]
        public DateTime orderedDate { get; set; }
        [BindProperty]
        public DateTime requiredDate { get; set; }
        public void OnGet()
        {

            ListCarItems = GetCartItemsInCookie();
            requiredDate = DateTime.Now.Date;
            //fill thông tin khách hàng khi đã đăng nhập
            var cusID = HttpContext.Session.GetString("CustomerID");
            if (cusID != null)
            {
                Customer = _context.Customers.SingleOrDefault(s => s.CustomerId == cusID);
            }

            HttpContext.Session.SetInt32("CountOrderCart", GetCartItemsInCookie().Count());

        }

        public List<CartItems> GetCartItemsInCookie()
        {
            var jsonCart = HttpContext.Request.Cookies["CartItemsAddToCart"];
            if (!string.IsNullOrEmpty(jsonCart))
            {
                return JsonSerializer.Deserialize<List<CartItems>>(jsonCart);
            }
            return new List<CartItems>();
        }

        public async Task<IActionResult> OnPost()
        {

            //Nếu chưa đăng nhập -> chuyển về signIn
            var cusID = HttpContext.Session.GetString("CustomerID");

            if (cusID == null)
            {
                return RedirectToPage("/account/signin");
            }
            else
            {
                //check so luong cart item 
                if (GetCartItemsInCookie().Count() == 0)
                {
                    return Page();
                }

                //tạo order
                var Ordered = new Models.Order()
                {
                    CustomerId = cusID,
                    EmployeeId = 1, //cho tạm bằng 1 ko báo lỗi
                    OrderDate = DateTime.Now,
                    RequiredDate = requiredDate,
                };
                _context.Orders.Add(Ordered);
                _context.SaveChanges();

                foreach (var item in GetCartItemsInCookie())
                {
                    var NewOrderDetail = new Models.OrderDetail()
                    {
                        ProductId = item.product.ProductId,
                        OrderId = Ordered.OrderId,
                        UnitPrice = (decimal)item.product.UnitPrice,
                        Quantity = (short)item.quantity,
                        Discount = 0,
                    };
                    _context.OrderDetails.Add(NewOrderDetail);
                }
                _context.SaveChanges();

                //remove cart
                RemoveAllCart();

                return RedirectToPage("/orders/listorders");
            }
        }

        void RemoveAllCart()
        {
            Response.Cookies.Delete("CartItemsAddToCart");
        }


        public int OrderedIdByCustomerID()
        {
            var cusID = HttpContext.Session.GetString("CustomerID");
            return (int)_context.Orders.OrderBy(s => s.OrderId).LastOrDefault(s => s.CustomerId == cusID).OrderId;
        }

        public IActionResult OnGetRemoveCart(int id)
        {
            var cart = GetCartItemsInCookie();
            var items = cart.Find(p => p.product.ProductId == id);
            if (items != null)
            {
                cart.Remove(items);
            }
            SaveCartToCookie(cart);
            return RedirectToPage();
        }

        private void SaveCartToCookie(List<CartItems> cart)
        {
            string jsonCart = JsonSerializer.Serialize(cart);
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddMinutes(30);
            Response.Cookies.Append("CartItemsAddToCart", jsonCart, option);
        }

        public IActionResult OnGetUpdateQuantity(int id, int action)
        {
            var cart = GetCartItemsInCookie();
            var items = cart.Find(p => p.product.ProductId == id);
            if (items != null)
            {
                if (action == 2)
                {
                    items.quantity += 1;
                }
                else
                {
                    items.quantity -= 1;
                    if (items.quantity <= 0)
                    {
                        cart.Remove(items);
                    }
                }
            }
            SaveCartToCookie(cart);
            return RedirectToPage();
        }
    }
}
