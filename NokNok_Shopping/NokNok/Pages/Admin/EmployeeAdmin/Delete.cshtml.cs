using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NokNok_ShoppingAPI.Models;

namespace NokNok.Pages.Admin.EmployeeAdmin
{
    public class DeleteModel : PageModel
    {
        private readonly HttpClient client = null;
        private string EmployeeApiUrl = "";
        private string DepartmentApiUrl = "";
        public IList<Employee> Employees { get; set; }
        public IList<Department> Departments { get; set; }

        public DeleteModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            EmployeeApiUrl = "http://localhost:5000/api/Employee/GetEmployeeById";
            DepartmentApiUrl = "http://localhost:5000/api/Department/GetDepartments";
        }

        [BindProperty]
      public Employee Employee { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpResponseMessage response = await client.GetAsync(EmployeeApiUrl + $"/{id}");
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Employee = JsonSerializer.Deserialize<Employee>(strData, options);

            if (Employee == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            if (Employee != null)
            {
                await client.DeleteAsync($"http://localhost:5000/api/Employee/DeleteEmployee/{id}");
            }

            return RedirectToPage("./Index");
        }
    }
}
