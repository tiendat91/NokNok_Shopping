using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace MyRazorPage.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        public ProfileModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        [BindProperty]
        public Customer Customer { get; set; }
        [BindProperty]
        public Models.Account Account { get; set; }

        public async Task<IActionResult> OnGet()
        {
            if (HttpContext.Session.GetString("CustomerSession") != null)
            {
                string customerID = HttpContext.Session.GetString("CustomerID");
                Customer = dBContext.Customers.SingleOrDefault(s => s.CustomerId == customerID);
                Account = dBContext.Accounts.SingleOrDefault(s => s.CustomerId == customerID);
                return Page();
            }
            return RedirectToPage("/account/signin");
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                //Xử lý update date xuống: Customer, Account
                var cusID = HttpContext.Session.GetString("CustomerID");
                var customer = await dBContext.Customers.SingleOrDefaultAsync(s => s.CustomerId == cusID);
                customer.CompanyName = Customer.CompanyName.Trim();
                customer.ContactName = Customer.ContactName.Trim();
                customer.ContactTitle = Customer.ContactTitle.Trim();
                customer.Address = Customer.Address.Trim();
                dBContext.Customers.Update(customer);
                await dBContext.SaveChangesAsync();

                var account = await dBContext.Accounts.SingleOrDefaultAsync(s => s.CustomerId == customer.CustomerId);
                //check email
                if (CheckOtherDulicateEmail(cusID, Account.Email) == false)
                {
                    ViewData["msg"] = "This email is duplicated with other account!";
                    return Page();
                }
                account.Email = Account.Email.Trim();
                dBContext.Accounts.Update(account);

                //Bổ sung 2 đối tượng vào database
                await dBContext.SaveChangesAsync();
                ViewData["msg"] = "Update susccesfully!";
                return Page();

            }
        }
        public bool CheckOtherDulicateEmail(string cusID, string newEmail)
        {
            foreach (var item in dBContext.Accounts.Where(s => s.CustomerId != cusID).Select(s => s.Email).ToList())
            {
                if (newEmail.Trim().ToLower().Equals(item.Trim().ToLower()))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
