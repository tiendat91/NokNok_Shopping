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

namespace NokNok.Pages.Admin.EmployeeAdmin
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient client = null;
        private string EmployeeApiUrl = "";
        private string DepartmentApiUrl = "";
        public IList<Employee> Employees { get; set; }
        public IList<Department> Departments { get; set; }
        [BindProperty]
        public Employee Employee { get; set; } = default!;

        public CreateModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            EmployeeApiUrl = "http://localhost:5000/api/Employee/GetEmployees";
            DepartmentApiUrl = "http://localhost:5000/api/Department/GetDepartments";
        }

        public async Task<IActionResult> OnGet()
        {
            HttpResponseMessage response = await client.GetAsync(DepartmentApiUrl);
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Departments = JsonSerializer.Deserialize<List<Department>>(strData, options);
            ViewData["DepartmentId"] = new SelectList(Departments, "DepartmentId", "DepartmentName");
            return Page();
        }


        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || Employee == null)
            {
                return Page();
            }

            string data = JsonSerializer.Serialize(Employee);
            await client.PostAsync("http://localhost:5000/api/Employee/CreateEmployee", new StringContent(data, Encoding.UTF8, "application/json"));

            return RedirectToPage("./Index");
        }
    }
}
