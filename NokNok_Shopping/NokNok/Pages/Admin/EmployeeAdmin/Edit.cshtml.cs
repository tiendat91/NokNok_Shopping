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
using NokNok_ShoppingAPI.Models;

namespace NokNok.Pages.Admin.EmployeeAdmin
{
    public class EditModel : PageModel
    {
        private readonly HttpClient client = null;
        private string EmployeeApiUrl = "";
        private string DepartmentApiUrl = "";
        public IList<Employee> Employees { get; set; }
        public IList<Department> Departments { get; set; }

        public EditModel()
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
            HttpResponseMessage response = await client.GetAsync(EmployeeApiUrl+$"/{id}");
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Employee = JsonSerializer.Deserialize<Employee>(strData, options);

            HttpResponseMessage responseD = await client.GetAsync(DepartmentApiUrl);
            string strDataD = await responseD.Content.ReadAsStringAsync();
            Departments = JsonSerializer.Deserialize<List<Department>>(strDataD, options);

            if (Employee == null)
            {
                return NotFound();
            }
           ViewData["DepartmentId"] = new SelectList(Departments, "DepartmentId", "DepartmentName");
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
                string data = JsonSerializer.Serialize(Employee);
                var response = await client.PutAsync($"http://localhost:5000/api/Employee/UpdateEmployee", new StringContent(data, Encoding.UTF8, "application/json"));
            }
            catch (Exception)
            {
                throw;
            }

            return RedirectToPage("./Index");
        }

    }
}
