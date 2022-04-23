using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace SoalTestFrontEnd.Pages
{

    public class UserType
    {
        public string username { get; set; }
        public string password { get; set; }

    }

    public class LoginModel : PageModel
    {
        [BindProperty]
        public UserType User { get; set; }
        public bool Succes { get; set; }
        public string Message { get; set; }
        public void OnGet()
        {
            
           
        }

        public async Task<IActionResult> OnPostLogout()
        {
            HttpContext.Session.Remove("Token");
            HttpContext.Session.Remove("UserName");

         
            return  Page(); 

        }

        public async Task<IActionResult> OnPost()
        {
            string token = "";
            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(User), System.Text.Encoding.UTF8, "application/json");

                string endpoint = "https://localhost:7184/api/token";

                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Succes = true;
                        Message = "";
                        token = Response.Content.ReadAsStringAsync().Result;
                        HttpContext.Session.SetString("Token", token);
                        HttpContext.Session.SetString("UserName", User.username);

                        return RedirectToPage("Index");
                    }
                    else
                    {
                        Message = Response.Content.ReadAsStringAsync().Result;
                        return Page();

                    }

                }

            }
        }
    }
}
