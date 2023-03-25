using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NokNok_ShoppingAPI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NokNok.Pages.User.Profile
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient client = null;
        private string CustomerApiUrl = "";
        private string AccountApiUrl = "";
        public IList<Customer> Customers { get; set; }
        [BindProperty]
        public Customer Customer { get; set; }
        [BindProperty]
        public Account Account { get; set; }
        public IndexModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            CustomerApiUrl = "http://localhost:5000/api/Customers/GetCustomerById";
            AccountApiUrl = "http://localhost:5000/api/Accounts";
        }
        public async Task<IActionResult> OnGet()
        {
            if (HttpContext.Session.GetString("CustomerSession") != null)
            {
                string customerID = HttpContext.Session.GetString("CustomerID");

                HttpResponseMessage response = await client.GetAsync(CustomerApiUrl + $"/{customerID}");
                string strData = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                Customer = JsonSerializer.Deserialize<Customer>(strData, options);

                HttpResponseMessage responseAcc = await client.GetAsync(AccountApiUrl);
                string strDataAcc = await responseAcc.Content.ReadAsStringAsync();
                Account = JsonSerializer.Deserialize<List<Account>>(strDataAcc, options).AsQueryable().FirstOrDefault(s=>s.CustomerId == customerID);
            }
            else
            {
                return RedirectToPage("../../Login");
            }
            return Page();
        }


    }
}
