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
    public class IndexModel : PageModel
    {
        private readonly HttpClient client = null;
        private string EmployeeApiUrl = "";
        private string DepartmentApiUrl = "";
        public IList<Employee> Employee { get; set; }
        public IList<Department> Departments { get; set; }

        public IndexModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            EmployeeApiUrl = "http://localhost:5000/api/Employee/GetEmployees";
            DepartmentApiUrl = "http://localhost:5000/api/Department/GetDepartments";
        }

        public async Task OnGetAsync()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            HttpResponseMessage response = await client.GetAsync(EmployeeApiUrl);
            string strData = await response.Content.ReadAsStringAsync();
            Employee = JsonSerializer.Deserialize<List<Employee>>(strData, options);

        }
    }
}
