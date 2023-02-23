using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using SignalR;

namespace MyRazorPage.Pages.Product
{
    [Authorize(Roles = "1")]


    public class EditModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IHubContext<HubServer> hubContext;

        public EditModel(PRN221DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;

        }
        [BindProperty]
        public Models.Product Product { get; set; }
        [BindProperty]
        public List<Category> categories { get; set; }
        public void OnGet(string proID)
        {
            categories = dBContext.Categories.ToList();
            Product = dBContext.Products.SingleOrDefault(s => s.ProductId == Int32.Parse(proID));
        }
        public async Task<IActionResult> OnPost(string? chkDiscontinued)
        {

            categories = dBContext.Categories.ToList();
            var EditProduct = await dBContext.Products.SingleOrDefaultAsync(s => s.ProductId == Product.ProductId);
            EditProduct.Discontinued = true;

            if (EditProduct != null)
            {
                EditProduct.ProductName = Product.ProductName;
                EditProduct.UnitPrice = Product.UnitPrice;
                EditProduct.QuantityPerUnit = Product.QuantityPerUnit;
                EditProduct.UnitsInStock = Product.UnitsInStock;
                EditProduct.CategoryId = Product.CategoryId;
                if (chkDiscontinued == "on")
                {
                    EditProduct.Discontinued = true;
                }
                else
                {
                    EditProduct.Discontinued = false;
                }

                dBContext.Products.Update(EditProduct);
                await dBContext.SaveChangesAsync();

                await hubContext.Clients.All.SendAsync("ReloadProduct");

                return RedirectToPage("/admin/product/index");
            }
            return Page();
        }
    }
}
