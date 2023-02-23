using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MyRazorPage.Pages.Admin
{
    [Authorize(Roles = "1")]
    public class CustomersModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        [BindProperty]
        public PaginatedList<Customer> Customers { get; set; }
        [BindProperty]
        public int TotalPage { get; set; }
        public string CurrentFilterName { get; set; }

        public CustomersModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        public async Task OnGet(string? searchName, string? currentFilterName, int? pageIndex)
        {
            IQueryable<Customer> CustomerIQ = dBContext.Customers;
            if (searchName == null)
            {
                searchName = currentFilterName;
            }
            CurrentFilterName = searchName;
            if (searchName != null)
            {
                CustomerIQ = CustomerIQ.Where(s => s.ContactName.ToLower().Contains(searchName.ToLower().Trim()));
            }
            Customers = await PaginatedList<Customer>.CreateAsync(CustomerIQ.AsNoTracking(), pageIndex ?? 1, 10);
            TotalPage = Customers.TotalPages;

        }
    }
}
