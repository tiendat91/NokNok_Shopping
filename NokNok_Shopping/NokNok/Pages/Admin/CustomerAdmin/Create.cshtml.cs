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

namespace NokNok.Pages.Admin.CustomerAdmin
{
    public class CreateModel : PageModel
    {
        private readonly HttpClient client = null;
        private string CustomerApiUrl = "";
        private readonly Random _random = new Random();

        public IList<Customer> Customers { get; set; }

        public CreateModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            CustomerApiUrl = "http://localhost:5000/api/Customers/GetCustomerById";
        }


        public IActionResult OnGet()
        {
            return Page();
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

        [BindProperty]
        public Customer Customer { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || Customer == null)
            {
                return Page();
            }
            Customer.CustomerId = GenerateCustomerID();
          Customer.CreatedDate = DateTime.Now;
          
            string data = JsonSerializer.Serialize(Customer);
            await client.PostAsync("http://localhost:5000/api/Customers/CreateCustomer", new StringContent(data, Encoding.UTF8, "application/json"));

            return RedirectToPage("./Index");
        }
    }
}
