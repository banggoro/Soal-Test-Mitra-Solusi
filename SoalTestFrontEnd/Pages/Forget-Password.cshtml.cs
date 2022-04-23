using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SoalTestFrontEnd.Pages
{
    public class ForgotType
    {
        public string useremail { get; set; }
      

    }

    public class Forget_PasswordModel : PageModel
    {
        [BindProperty]
        public ForgotType User { get; set; }
        public bool Succes { get; set; }
        public string Message { get; set; }
        public void OnGet()
        {
        }
    }
}
