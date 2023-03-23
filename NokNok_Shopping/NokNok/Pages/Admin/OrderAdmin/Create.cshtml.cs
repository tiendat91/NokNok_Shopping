using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using NokNok_ShoppingAPI.Models;

namespace NokNok.Pages.Admin.OrderAdmin
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient client = null;
        private string OrderApiUrl = "";
        private string CustomerApiUrl = "";
        private string EmployeeApiUrl = "";
        public IList<Customer> Customers { get; set; }
        public IList<Employee> Employees { get; set; }

        public CreateModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            OrderApiUrl = "http://localhost:5000/api/Orders/GetOrderById";
            CustomerApiUrl = "http://localhost:5000/api/Customers/GetCustomers";
            EmployeeApiUrl = "http://localhost:5000/api/Employee/GetEmployees";
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            HttpResponseMessage responseE = await client.GetAsync(EmployeeApiUrl);
            string strDataE = await responseE.Content.ReadAsStringAsync();
            Employees = JsonSerializer.Deserialize<List<Employee>>(strDataE, options);

            HttpResponseMessage responseC = await client.GetAsync(CustomerApiUrl);
            string strDataC = await responseC.Content.ReadAsStringAsync();
            Customers = JsonSerializer.Deserialize<List<Customer>>(strDataC, options);
            ViewData["CustomerId"] = new SelectList(Customers, "CustomerId", "CompanyName");
            ViewData["EmployeeId"] = new SelectList(Employees, "EmployeeId", "LastName");
            return Page();
        }

        [BindProperty]
        public Order Order { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || Order == null)
            {
                return Page();
            }
            Order.Customer = null;
            Order.Employee = null;
            Order.OrderDetails = null;
            string data = JsonSerializer.Serialize(Order);
            await client.PostAsync("http://localhost:5000/api/Orders/CreateOrder", new StringContent(data, Encoding.UTF8, "application/json"));

            return RedirectToPage("./Index");
        }
    }
}
