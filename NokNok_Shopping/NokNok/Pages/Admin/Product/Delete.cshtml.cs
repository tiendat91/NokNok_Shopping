using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using SignalR;

namespace MyRazorPage.Pages.Product
{
    [Authorize(Roles = "1")]


    public class DeleteModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IHubContext<HubServer> hubContext;

        public DeleteModel(PRN221DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;

        }
        [BindProperty]
        public Models.Product Product { get; set; }
        [BindProperty]
        public List<Category> categories { get; set; }
        [BindProperty]
        public List<OrderDetail> ProductInOrder { get; set; }
        [ViewData]
        public string msg { get; set; }
        public void OnGet(string? ProID)
        {
            categories = dBContext.Categories.ToList();
            Product = dBContext.Products.SingleOrDefault(s => s.ProductId == Int32.Parse(ProID));
            ProductInOrder = dBContext.OrderDetails.Include(s => s.Product).Where(s => s.ProductId == Int32.Parse(ProID)).ToList();
        }

        public async Task<IActionResult> OnPost(string? ProID)
        {
            categories = dBContext.Categories.ToList();
            Product = dBContext.Products.SingleOrDefault(s => s.ProductId == Int32.Parse(ProID));
            ProductInOrder = dBContext.OrderDetails.Include(s => s.Product).Where(s => s.ProductId == Int32.Parse(ProID)).ToList();

            //Kiểm tra có trong đơn hàng nào không mới cho xóa
            if (ProductInOrder.Count == 0)
            {
                dBContext.Products.Remove(dBContext.Products.SingleOrDefault(s => s.ProductId == Product.ProductId));
                await dBContext.SaveChangesAsync();

                await hubContext.Clients.All.SendAsync("ReloadProduct");

                return RedirectToPage("/admin/product/index");
            }
            else
            {
                ViewData["msg"] = "Delete Failed";
                return Page();
            }

        }
    }
}
