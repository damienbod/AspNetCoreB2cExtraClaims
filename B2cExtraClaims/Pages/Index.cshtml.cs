using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace B2cExtraClaims.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public IEnumerable<Claim> Claims { get; set; } = Enumerable.Empty<Claim>();

        public void OnGet()
        {
            Claims = User.Claims;
        }
    }
}