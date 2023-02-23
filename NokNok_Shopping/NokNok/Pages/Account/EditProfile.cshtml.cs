using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;

namespace MyRazorPage.Pages.Account
{
    [Authorize]
    public class EditProfileModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        public EditProfileModel(PRN221DBContext dBContext)
        {
            this.dBContext = dBContext;
        }
        //khai báo 2 thuộc tính đại diện cho 2 đối tượng kiểu: Account, Customer
        [BindProperty]
        public Customer Customer { get; set; }
        [BindProperty]
        public Models.Account Account { get; set; }
        [ViewData]
        public string msg { get; set; }
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                return Page();
            }
            else
            {
                var acc = await dBContext.Accounts.SingleOrDefaultAsync(a => a.Email.Equals(Account.Email));
                if (acc != null)
                {
                    //Xử lý update date xuống: Customer, Account
                    var cusID = HttpContext.Session.GetString("CustomerID");
                    var customer = await dBContext.Customers.SingleOrDefaultAsync(s => s.CustomerId == cusID);
                    customer.CompanyName = Customer.CompanyName;
                    customer.ContactName = Customer.ContactName;
                    customer.ContactTitle = Customer.ContactTitle;
                    customer.Address = Customer.Address;

                    var account = await dBContext.Accounts.SingleOrDefaultAsync(s => s.CustomerId == cusID);
                    account.Email = Account.Email;
                    account.Password = Account.Password;

                    //Bổ sung 2 đối tượng vào database
                    dBContext.Customers.Update(customer);
                    dBContext.Accounts.Update(account);
                    await dBContext.SaveChangesAsync();
                    return RedirectToPage("/account/profile");
                }
                else
                {
                    ViewData["msg"] = "This email does exist.";
                    return Page();
                }
            }
        }
    }
}
