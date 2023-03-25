using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NokNok.Pages.Admin.Dashboard
{
    public class IndexModel : PageModel
    {
        [Authorize]
        public void OnGet()
        {
        }
    }
}
