using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Cryptography;

namespace MyRazorPage.Pages.Account
{
    public class SignInModel : PageModel
    {
        private readonly PRN221DBContext dBContext;
        private IConfiguration _configuration;
        public SignInModel(PRN221DBContext dBContext, IConfiguration configuration)
        {
            this.dBContext = dBContext;
            _configuration = configuration;
        }
        [BindProperty]
        public Models.Account Account { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid) ///BUG
            {
                return Page();
            }
            var acc = await dBContext.Accounts.SingleOrDefaultAsync(s => s.Email.Equals(Account.Email) && s.Password.Equals(Account.Password));

            if (acc == null)
            {
                ViewData["msg"] = "This account does not exist or wrong password";
                return Page();
            }
            else
            {
                var claims = new List<Claim>() {
                        new Claim(ClaimTypes.Name, acc.Email),
                        new Claim(ClaimTypes.Role, acc.Role.ToString()),
                    };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString("CustomerSession", JsonSerializer.Serialize(acc));
                HttpContext.Session.SetString("CustomerID", acc.CustomerId);
                if (acc.Role == 2)
                {
                    HttpContext.Session.SetString("role", "2");
                }
                return RedirectToPage("/index");
            }
        }


    }
}
