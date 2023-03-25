using EmailService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using NokNok_ShoppingAPI.Controllers;
using NokNok_ShoppingAPI.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NokNok.Pages.User.OrderHistory
{
    public class InvoiceModel : PageModel
    {
        private readonly IEmailSender _emailSender;
        private readonly HttpClient client = null;
        private string OrderApiUrl = "";
        private string CustomerApiUrl = "";
        private string ProductApiUrl = "";
        private string OrderDetailApiUrl = "";
        private string AccountApiUrl = "";
        public List<Customer> Customers { get; set; }
        public List<Order> Orders { get; set; }
        public List<Account> Accounts { get; set; }
        public List<Product> Products { get; set; }

        [BindProperty]
        public Customer Customer { get; set; }
        [BindProperty]
        public Order Order { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TaxTotal { get; set; }

        public InvoiceModel(IEmailSender emailSender)
        {
            this._emailSender = emailSender;
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            OrderApiUrl = "http://localhost:5000/api/Orders/GetOrders";
            CustomerApiUrl = "http://localhost:5000/api/Customers/GetCustomerById";
            ProductApiUrl = "http://localhost:5000/api/Products/GetAllProducts";
            OrderDetailApiUrl = "http://localhost:5000/api/Products/GetAllProducts";
            AccountApiUrl = "http://localhost:5000/api/Accounts";
        }
        public async void OnGetAsync(string? orderId)
        {
            string customerID = HttpContext.Session.GetString("CustomerID");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            HttpResponseMessage response = await client.GetAsync(OrderApiUrl);
            string strData = await response.Content.ReadAsStringAsync();
            Order = JsonSerializer.Deserialize<List<Order>>(strData, options).AsQueryable().FirstOrDefault(s=>s.OrderId.ToString() == orderId);

            HttpResponseMessage responseC = await client.GetAsync(CustomerApiUrl + $"/{customerID}");
            string strDataC = await responseC.Content.ReadAsStringAsync();
            Customer = JsonSerializer.Deserialize<Customer>(strDataC, options);

            HttpResponseMessage responseP = await client.GetAsync(ProductApiUrl);
            string strDataP = await responseP.Content.ReadAsStringAsync();
            Products = JsonSerializer.Deserialize<List<Product>>(strDataP, options);


                foreach (var od in Order.OrderDetails)
                {
                    od.Product = Products.AsQueryable().FirstOrDefault(s => s.ProductId == od.ProductId);
                }

            //foreach (var item in Orders.OrderDetails)
            //{
            //    SubTotal += (decimal)item.Product.UnitPrice;
            //}
            TaxTotal = SubTotal / 100 * 8;
            GrandTotal = SubTotal - TaxTotal;
        }

        public async Task<IActionResult> OnGetSendEmail(string? orderId)
        {
            string customerID = HttpContext.Session.GetString("CustomerID");

            HttpResponseMessage response = await client.GetAsync(AccountApiUrl);
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Accounts = JsonSerializer.Deserialize<List<Account>>(strData, options);

            var acc = Accounts.SingleOrDefault(s => s.CustomerId == customerID);
            //var pathToRazorPageFolder = request.PathToRazorPageFolder();

            var msg = RenderToString($"~/orders/orderpdf?orderId={orderId}");

            //var message = new Message(new string[] { acc.Email }, "Order Receipt", "");
            //_emailSender.SendEmail(message);
            return Page();
        }

        public static string RenderToString(string url)
        {
            try
            {
                //Grab page
                WebRequest request = WebRequest.Create(url);
                WebResponse response = request.GetResponse();
                Stream data = response.GetResponseStream();
                string html = String.Empty;
                using (StreamReader sr = new StreamReader(data))
                {
                    html = sr.ReadToEnd();
                }
                return html;
            }
            catch (Exception err)
            {
                return null;
            }
        }
    }
}
