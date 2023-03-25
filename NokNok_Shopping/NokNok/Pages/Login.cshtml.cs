using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using NokNok_ShoppingAPI.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace NokNok.Pages
{
    public class LoginModel : PageModel
    {
        private readonly HttpClient client = null;
        private string AccountApiUrl = "";
        public LoginModel()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            AccountApiUrl = "http://localhost:5000/api/Accounts";
        }

        [BindProperty]
        public Account Account { get; set; }
        public List<Account> Accounts { get; set; }
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

            var acc = Accounts.AsQueryable().FirstOrDefault(s => s.Email.Equals(Account.Email) && s.Password.Equals(Account.Password));

            if (acc == null)
            {
                ViewData["msg"] = "This account does not exist or wrong password";
                return Page();
            }
            else
            {
                //COOKIE AUTHENTICATION
                var claims = new List<Claim>() {
                        new Claim(ClaimTypes.Name, acc.Email),
                        new Claim(ClaimTypes.Role, acc.Role.ToString()),
                    };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                //SESSION USER
                HttpContext.Session.SetString("CustomerSession", JsonSerializer.Serialize(acc));
                HttpContext.Session.SetString("CustomerID", acc.CustomerId);

                //SESSION ROLE: 1- ADMIN, 2-USER
                if (acc.Role == 1)
                {
                    HttpContext.Session.SetString("role", "1");
                }
                if (acc.Role == 2)
                {
                    HttpContext.Session.SetString("role", "2");
                }
                return RedirectToPage("~/Index");
            }
        }
    }
}
