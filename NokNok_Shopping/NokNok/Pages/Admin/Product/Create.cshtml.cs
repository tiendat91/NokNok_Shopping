using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SignalR;

namespace MyRazorPage.Pages.Product
{
    [Authorize(Roles = "1")]
    public class CreateModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly IHubContext<HubServer> hubContext;

        public CreateModel(PRN221DBContext dBContext, IHubContext<HubServer> hubContext)
        {
            this.dBContext = dBContext;
            this.hubContext = hubContext;

        }
        [BindProperty]
        public Models.Product Product { get; set; }
        [BindProperty]
        public List<Category> categories { get; set; }

        public void OnGet()
        {
            categories = dBContext.Categories.ToList();
        }
        public async Task<IActionResult> OnPostAsync([Bind("Category")] string? chkDiscontinued)
        {
            categories = dBContext.Categories.ToList();

            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}

            if (chkDiscontinued == "on")
            {
                Product.Discontinued = false;
            }
            Product.Discontinued = true;

            await dBContext.AddAsync(Product);

            await dBContext.SaveChangesAsync();

            await hubContext.Clients.All.SendAsync("ReloadProduct");

            return RedirectToPage("/admin/product/index");
        }
    }
}
