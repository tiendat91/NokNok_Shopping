using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace MyRazorPage.Pages.Account
{
    public class SignUpModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private readonly Random _random = new Random();
        public SignUpModel(PRN221DBContext dBContext)
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
            if (ModelState.IsValid) //thuộc tính ID bên cshtml cần được gọi đến để valid (set ẩn ID = 1)
            {
                return Page(); //return về chính page của nó
            }
            else
            {
                var acc = await dBContext.Accounts.SingleOrDefaultAsync(a => a.Email.Equals(Account.Email));
                if (acc == null)
                {
                    //Xử lý insert xuống: Customer, Account
                    var cusID = GenerateCustomerID();
                    var customer = new Customer()
                    {
                        CustomerId = cusID,
                        CompanyName = Customer.CompanyName.Trim(),
                        ContactName = Customer.ContactName.Trim(),
                        ContactTitle = Customer.ContactTitle.Trim(),
                        Address = Customer.Address.Trim(),
                        CreatedDate = DateTime.Now
                    };

                    const int WorkFactor = 14;
                    var HashedPassword = BCrypt.Net.BCrypt.HashPassword(Account.Password, WorkFactor);

                    var newAcc = new Models.Account()
                    {
                        Email = Account.Email.Trim(),
                        Password = HashedPassword,
                        CustomerId = cusID,
                        EmployeeId = 1,
                        Role = 2
                    };

                    //Bổ sung 2 đối tượng vào database
                    await dBContext.Customers.AddAsync(customer);
                    await dBContext.Accounts.AddAsync(newAcc);
                    await dBContext.SaveChangesAsync();
                    return RedirectToPage("/index");
                }
                else
                {
                    ViewData["msg"] = "This email does exist.";
                    return Page();
                }
            }
        }

        //tạo CustomerID - 5 letters
        public string GenerateCustomerID()
        {
            int size = 5;
            var generatedID = new StringBuilder(size);
            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(65, 90);
                generatedID.Append(@char);
            }
            return generatedID.ToString();
        }


    }
}
