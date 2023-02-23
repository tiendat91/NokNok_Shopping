using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static MyRazorPage.Pages.Account.CartModel;

namespace MyRazorPage.Pages.Product
{
    public class CategoryModel : PageModel
    {
        readonly PRN221DBContext dBContext;
        private readonly IConfiguration Configuration;

        public CategoryModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        [BindProperty]
        public List<Category> Category { get; set; }
        [BindProperty]
        public PaginatedList<Models.Product> Products { get; set; }
        [ViewData]
        public int selectedCategoryId { get; set; }
        [ViewData]
        public string SortOrder { get; set; }
        [ViewData]
        public int TotalPage { get; set; }

        public async Task OnGetAsync(int? catID, string? sortOrder, string? searchString, int? pageIndex)
        {
            if (catID == null)
            {
                catID = 1;
            }
            selectedCategoryId = (int)catID;
            SortOrder = sortOrder;
            IQueryable<Models.Product> ProductIQ = from s in dBContext.Products
                                                   where s.CategoryId == selectedCategoryId
                                                   select s;
            switch (sortOrder)
            {
                case "desc":
                    ProductIQ = ProductIQ.OrderByDescending(s => s.UnitPrice);
                    break;
                case "asc":
                    ProductIQ = ProductIQ.OrderBy(s => s.UnitPrice);
                    break;
                default:
                    break;
            }

            Products = await PaginatedList<Models.Product>.CreateAsync(ProductIQ.AsNoTracking(), pageIndex ?? 1, 12);

            TotalPage = Products.TotalPages;
            Category = dBContext.Categories.ToList();
        }

        public void OnPost(string? filterCategory)
        {
        }

        public async Task<IActionResult> OnGetBuyNow(int? ProID)
        {
            //Add Product to cookie
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(30);
            int CountOrderCart;
            var jsonCarts = HttpContext.Request.Cookies["CartItemsAddToCart"];
            var pro = await dBContext.Products.SingleOrDefaultAsync(m => m.ProductId == ProID);

            if (jsonCarts != null)
            {

                var listjsonCarts = JsonSerializer.Deserialize<List<CartItems>>(jsonCarts);
                bool duplicatedId = false;
                //check trung ID, tang so luong
                foreach (var item in listjsonCarts.ToList())
                {
                    if (item.product.ProductId == ProID)
                    {
                        item.quantity += 1;
                        duplicatedId = true;
                    }
                }
                if (duplicatedId == false)
                {
                    listjsonCarts.Add(new CartItems { product = pro, quantity = 1 });
                }
                Response.Cookies.Append("CartItemsAddToCart", JsonSerializer.Serialize<List<CartItems>>(listjsonCarts), option);
                CountOrderCart = listjsonCarts.Count();
            }
            else
            {
                List<CartItems> listjsonCarts = new List<CartItems> { };
                listjsonCarts.Add(new CartItems { product = pro, quantity = 1 });
                Response.Cookies.Append("CartItemsAddToCart", JsonSerializer.Serialize<List<CartItems>>(listjsonCarts), option);
                CountOrderCart = listjsonCarts.Count();
            }

            HttpContext.Session.SetString("ProductIdOrdered" + ProID.ToString(), ProID.ToString());
            if (ProID == null || dBContext.Products == null)
            {
                return NotFound();
            }


            //hiện thi cart
            HttpContext.Session.SetInt32("CountOrderCart", CountOrderCart);

            ViewData["msg"] = "Add to cart susccesfully!";
            return RedirectToPage("/account/cart");
        }
    }
}
