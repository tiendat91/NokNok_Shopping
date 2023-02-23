using DocumentFormat.OpenXml.Spreadsheet;
using EmailService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyRazorPage.Services;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;

namespace MyRazorPage.Pages.Orders
{
    public class OrderPDFModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IEmailSender _emailSender;


        [BindProperty]
        public Customer Customer { get; set; }
        [BindProperty]
        public Order Order { get; set; }
        public decimal SubTotal { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal TaxTotal { get; set; }

        public OrderPDFModel(IEmailSender emailSender, PRN221DBContext dBContext)
        {
            this._emailSender = emailSender;
            this.dBContext = dBContext;
        }
        public void OnGet(string? orderId)
        {
            string customerID = HttpContext.Session.GetString("CustomerID");
            Customer = dBContext.Customers.SingleOrDefault(s => s.CustomerId == customerID);

            Order = dBContext.Orders.Include(s => s.Customer)
                .Include(s => s.OrderDetails).ThenInclude(s => s.Product)
                .OrderByDescending(s => s.OrderDate)
                .SingleOrDefault(s => s.OrderId == Int32.Parse(orderId));

            foreach (var item in Order.OrderDetails)
            {
                SubTotal += (decimal)item.Product.UnitPrice;
            }
            TaxTotal = SubTotal / 100 * 8;
            GrandTotal = SubTotal - TaxTotal;
        }

        public IActionResult OnGetSendEmail(string? orderId)
        {
            string customerID = HttpContext.Session.GetString("CustomerID");
            var acc = dBContext.Accounts.SingleOrDefault(s => s.CustomerId == customerID);

            var msg = RenderToString($"~/orders/orderpdf?orderId={orderId}");

            return Page();
        }

        public static string RenderToString(string url)
        {
            try
            {
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