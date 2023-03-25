using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using NokNok_ShoppingAPI.Models;
using System.Text;
using System;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NokNok.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly HttpClient client = null;
        private readonly Random _random = new Random();
        private string AccountApiUrl = "";
        public List<Account> Accounts { get; set; }

        [BindProperty]
        public Customer Customer { get; set; }
        [BindProperty]
        public Account Account { get; set; }
        [ViewData]
        public string msg { get; set; }

        public RegisterModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            AccountApiUrl = "http://localhost:5000/api/Accounts";
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
                HttpResponseMessage response = await client.GetAsync(AccountApiUrl);
                string strData = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                Accounts = JsonSerializer.Deserialize<List<Account>>(strData, options);

                Account acc = Accounts.AsQueryable().FirstOrDefault(a => a.Email.Equals(Account.Email));

                if (acc == null)
                {
                    //Xử lý insert xuống: Customer, Account
                    var cusID = GenerateCustomerID();
                    var newCustomer = new Customer()
                    {
                        CustomerId = cusID,
                        CompanyName = Customer.CompanyName.Trim(),
                        ContactName = Customer.ContactName.Trim(),
                        ContactTitle = Customer.ContactTitle.Trim(),
                        Address = Customer.Address.Trim(),
                        CreatedDate = DateTime.Now
                    };

                    var newAcc = new Account()
                    {
                        Email = Account.Email.Trim(),
                        Password = Account.Password.Trim(),
                        CustomerId = cusID,
                        EmployeeId = 1,
                        Role = 2
                    };

                    //ADD NEW CUSTOMER
                    string dataCustomer = JsonSerializer.Serialize(newCustomer);
                    await client.PostAsync("http://localhost:5000/api/Customers/CreateCustomer", new StringContent(dataCustomer, Encoding.UTF8, "application/json"));
                    
                    //ADD NEW ACCOUNT
                    string dataAccount = JsonSerializer.Serialize(newAcc);
                    await client.PostAsync("http://localhost:5000/api/Accounts/CreateAccount", new StringContent(dataAccount, Encoding.UTF8, "application/json"));

                    return RedirectToPage("/index");
                }
                else
                {
                    ViewData["msg"] = "This email does exist.";
                    return Page();
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
