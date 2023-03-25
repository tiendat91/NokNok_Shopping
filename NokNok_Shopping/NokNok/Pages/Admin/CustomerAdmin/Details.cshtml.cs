﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NokNok_ShoppingAPI.Models;

namespace NokNok.Pages.Admin.CustomerAdmin
{
    public class DetailsModel : PageModel
    {
        private readonly HttpClient client = null;
        private string CustomerApiUrl = "";
        public IList<Customer> Customers { get; set; }

        public DetailsModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            CustomerApiUrl = "http://localhost:5000/api/Customers/GetCustomerById";
        }

        public Customer Customer { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            HttpResponseMessage response = await client.GetAsync(CustomerApiUrl + $"/{id}");
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            Customer = JsonSerializer.Deserialize<Customer>(strData, options);

            if (Customer == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
