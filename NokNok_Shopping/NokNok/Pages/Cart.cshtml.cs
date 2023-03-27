using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using NokNok_ShoppingAPI.Models;
using System.ComponentModel;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace NokNok.Pages
{
    public class CartModel : PageModel
    {
        private readonly HttpClient client = null;
        private string CustomerApiUrl = "";
        private string OrderApiUrl = "";
        public List<Customer> Customers { get; set; }
        public List<Order> Orders { get; set; }
        public CartModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            CustomerApiUrl = "http://localhost:5000/api/Customers/GetCustomerById";
            OrderApiUrl = "http://localhost:5000/api/Orders/GetOrders";
        }

        public class CartItems
        {
            public int quantity { get; set; }
            public Product product { get; set; }
        }
        [BindProperty]
        public OrderDetail OrderDetail { get; set; }
        [BindProperty]
        public Customer Customer { get; set; }

        [BindProperty]
        public List<OrderDetail> OrderDetails { get; set; }
        [BindProperty]
        public List<CartItems> ListCarItems { get; set; }
        [BindProperty]
        public DateTime orderedDate { get; set; }
        [BindProperty]
        public DateTime requiredDate { get; set; }
        public async Task<IActionResult> OnGet()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ListCarItems = GetCartItemsInCookie();
            requiredDate = DateTime.Now.Date;
            //fill thông tin khách hàng khi đã đăng nhập
            var cusID = HttpContext.Session.GetString("CustomerID");
            if (cusID != null)
            {
                HttpResponseMessage response = await client.GetAsync(CustomerApiUrl + $"/{cusID}");
                string strData = await response.Content.ReadAsStringAsync();

                Customer = JsonSerializer.Deserialize<Customer>(strData, options);
            }

            HttpResponseMessage responseO = await client.GetAsync(OrderApiUrl);
            string strDataO = await responseO.Content.ReadAsStringAsync();
            Orders = JsonSerializer.Deserialize<List<Order>>(strDataO, options);


            HttpContext.Session.SetInt32("CountOrderCart", GetCartItemsInCookie().Count());
            return Page();
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

        public async Task<IActionResult> OnGetOrder()
        {

            //Nếu chưa đăng nhập -> chuyển về signIn
            var cusID = HttpContext.Session.GetString("CustomerID");

            if (cusID == null)
            {
                return RedirectToPage("~/Login");
            }
            else
            {
                //check so luong cart item 
                if (GetCartItemsInCookie().Count() == 0)
                {
                    return Page();
                }

                //TẠM FIX
                return RedirectToPage("/User/OrderHistory");


                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                HttpResponseMessage responseO = await client.GetAsync(OrderApiUrl);
                string strDataO = await responseO.Content.ReadAsStringAsync();
                Orders = JsonSerializer.Deserialize<List<Order>>(strDataO, options);

                //tạo order
                var Ordered = new Order()
                {
                    CustomerId = cusID,
                    EmployeeId = 1, //cho tạm bằng 1 ko báo lỗi
                    OrderDate = DateTime.Now,
                    RequiredDate = requiredDate,
                };

                Ordered.OrderId = OrderedIdByCustomerID();

                string data = JsonSerializer.Serialize(Ordered);
                await client.PostAsync("http://localhost:5000/api/Orders/CreateOrder", new StringContent(data, Encoding.UTF8, "application/json"));

                foreach (var item in GetCartItemsInCookie())
                {
                    var NewOrderDetail = new OrderDetail()
                    {
                        ProductId = item.product.ProductId,
                        OrderId = Ordered.OrderId,
                        UnitPrice = (decimal)item.product.UnitPrice,
                        Quantity = (short)item.quantity,
                        Discount = 0f,
                    };

                    string dataOrderDetail = JsonSerializer.Serialize(NewOrderDetail);
                    await client.PostAsync("http://localhost:5000/api/Orders/CreateOrderDetail", new StringContent(data, Encoding.UTF8, "application/json"));
                }

                //remove cart
                RemoveAllCart();

                return RedirectToPage("~/User/OrderHistory");
            }
        }

        void RemoveAllCart()
        {
            Response.Cookies.Delete("CartItemsAddToCart");
        }


        public int OrderedIdByCustomerID()
        {
            var cusID = HttpContext.Session.GetString("CustomerID");
            return (int)Orders.OrderBy(s => s.OrderId).LastOrDefault(s => s.CustomerId == cusID).OrderId;
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

        public IActionResult OnGetUpdateNumber(int id, string Choice)
        {
            var cart = GetCartItemsInCookie();
            var items = cart.Find(p => p.product.ProductId == id);
            if (items != null)
            {
                if (Choice == "Add")
                {
                    items.quantity += 1;
                }
                if(Choice == "Sub")
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
