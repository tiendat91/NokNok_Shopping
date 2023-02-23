using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using EmailService;
using Syncfusion.Pdf;
using Syncfusion.HtmlConverter;
using System.IO;


namespace MyRazorPage.Pages.Orders
{
    public class ListOrdersModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IEmailSender _emailSender;
        public ListOrdersModel(IEmailSender emailSender, PRN221DBContext db)
        {
            this._emailSender = emailSender;
            this.dBContext = db;
        }
        public class OrderedProduct
        {
            public DateTime? OrderDate { get; set; }
            public List<OrderedProductProperties>? ListOrder { get; set; }

        }

        public class OrderedProductProperties
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string Image { get; set; }
            public decimal UnitPriceOrder { get; set; }
            public short QuantityOrder { get; set; }
        }

        [BindProperty]
        public Customer Customer { get; set; }
        [BindProperty]
        public Models.Account Account { get; set; }
        [BindProperty]
        public List<Order> OrderedList { get; set; }
        //[BindProperty]
        //public List<OrderedProduct> OrderedProducts { get; set; }
        public void OnGet()
        {
            string customerID = HttpContext.Session.GetString("CustomerID");
            Customer = dBContext.Customers.Include(s => s.Accounts).SingleOrDefault(s => s.CustomerId == customerID);

            OrderedList = dBContext.Orders.Where(s => s.CustomerId == customerID)
                .Include(s => s.OrderDetails).ThenInclude(s => s.Product)
                .OrderByDescending(s => s.OrderDate)
                .ToList();
        }

        public IActionResult OnGetDeleteOrder(string? orderId)
        {
            if (orderId != null)
            {
                //Xoa order
                foreach (var item in dBContext.OrderDetails.Where(s => s.OrderId == Int32.Parse(orderId)).ToList())
                {
                    dBContext.OrderDetails.Remove(item);
                }
                var x = dBContext.Orders.SingleOrDefault(s => s.OrderId == Int32.Parse(orderId));
                dBContext.Orders.Remove(x);
                dBContext.SaveChanges();
            }
            return RedirectToPage();

        }

        public async Task<IActionResult> OnPost(string? email, string? orderID)
        {
            bool checkExist = false;
            if (email != null)
            {
                foreach (var item in dBContext.Accounts)
                {
                    if (item.Email.ToLower() == email.ToLower().Trim())
                    {
                        checkExist = true;
                    }
                }
                if (checkExist)
                {
                    var acc = dBContext.Accounts.Include(s => s.Customer).SingleOrDefault(s => s.Email.ToLower().Equals(email.ToLower()));

                    var files = Request.Form.Files.Any() ? Request.Form.Files : new FormFileCollection();

                    String contentSend = $"Hi {acc.Customer.ContactName}, <br><br> Thanks for your order, we hope you enjoyed shopping with us<br>" +
                        $"<h2 style='color:red'>Order ID: {orderID}</h2>" +
                        $"<br>" +
                        $"<a href=\"http://localhost:5000/orders/orderpdf?orderId={orderID}\" target=\"_blank\" rel=\"noopener noreferrer\">Clickhere</a>";

                    var message = new Message(new string[] { email }, "ORDER RECEIPT EMAIL", contentSend, files);
                    _emailSender.SendEmail(message);
                }
            }
            return RedirectToPage();
        }
    }
}
