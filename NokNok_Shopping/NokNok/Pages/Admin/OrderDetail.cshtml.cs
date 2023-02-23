using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MyRazorPage.Pages.Admin
{
    public class OrderDetailModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        [BindProperty]
        public Order order { get; set; }
        public OrderDetailModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public void OnGet(string? id)
        {
            order = dBContext.Orders
                .Include(s => s.OrderDetails).ThenInclude(s => s.Product)
                .SingleOrDefault(s => s.OrderId == Int32.Parse(id));
        }
    }
}
