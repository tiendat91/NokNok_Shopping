using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MyRazorPage.Pages.Orders
{
    public class CanceledOrderModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        [BindProperty]
        public Customer Customer { get; set; }
        [BindProperty]
        public List<Order> OrderedList { get; set; }
        public CanceledOrderModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public void OnGet()
        {
            string customerID = HttpContext.Session.GetString("CustomerID");
            Customer = dBContext.Customers.SingleOrDefault(s => s.CustomerId == customerID);
            OrderedList = dBContext.Orders.Where(s => s.CustomerId == customerID)
                .Include(s => s.OrderDetails).ThenInclude(s => s.Product)
                .OrderByDescending(s => s.OrderDate)
                .Where(s => s.RequiredDate == null && s.ShippedDate == null).ToList();
        }
    }
}
