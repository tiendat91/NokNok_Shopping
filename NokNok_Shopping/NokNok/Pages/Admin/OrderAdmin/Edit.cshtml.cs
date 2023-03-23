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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NokNok_ShoppingAPI.Models;

namespace NokNok.Pages.Admin.OrderAdmin
{
    public class EditModel : PageModel
    {
        private readonly HttpClient client = null;
        private string OrderApiUrl = "";
        private string CustomerApiUrl = "";
        private string EmployeeApiUrl = "";
        [BindProperty]
        public Order Order { get; set; }
        public IList<Customer> Customers { get; set; }
        public IList<Employee> Employees { get; set; }

        public EditModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            OrderApiUrl = "http://localhost:5000/api/Orders/GetOrderById";
            CustomerApiUrl = "http://localhost:5000/api/Customers/GetCustomers";
            EmployeeApiUrl = "http://localhost:5000/api/Employee/GetEmployees";
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            HttpResponseMessage response = await client.GetAsync(OrderApiUrl+$"/{id}");
            string strData = await response.Content.ReadAsStringAsync();
            Order = JsonSerializer.Deserialize<Order>(strData, options);

            HttpResponseMessage responseE = await client.GetAsync(EmployeeApiUrl);
            string strDataE = await responseE.Content.ReadAsStringAsync();
            Employees = JsonSerializer.Deserialize<List<Employee>>(strDataE, options);

            HttpResponseMessage responseC = await client.GetAsync(CustomerApiUrl);
            string strDataC = await responseC.Content.ReadAsStringAsync();
            Customers = JsonSerializer.Deserialize<List<Customer>>(strDataC, options);

            if (Order == null)
            {
                return NotFound();
            }
            ViewData["CustomerId"] = new SelectList(Customers, "CustomerId", "CompanyName");
            ViewData["EmployeeId"] = new SelectList(Employees, "EmployeeId", "LastName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            try
            {
                string data = JsonSerializer.Serialize(Order);
                var response = await client.PutAsync($"http://localhost:5000/api/Orders/UpdateOrder", new StringContent(data, Encoding.UTF8, "application/json"));
            }
            catch (Exception)
            {

                throw;
            }
            

            return RedirectToPage("./Index");
        }
    }
}
