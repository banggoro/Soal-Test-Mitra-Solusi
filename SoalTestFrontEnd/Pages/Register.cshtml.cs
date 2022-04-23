using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace SoalTestFrontEnd.Pages
{

    public class RegisterType
    {
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
    }

    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterType Register { get; set; }
        public bool Succes { get; set; }
        public string Message { get; set; }
        public void OnGet()
        {
        }

        public RegisterModel()
        {
            Succes = false;
        }

        public async Task<IActionResult> OnPost()
        {

            if (!ModelState.IsValid)
            {
                return Page();
            }

            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(Register), System.Text.Encoding.UTF8, "application/json");

                string endpoint = "https://localhost:7184/api/register";

                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        //string token = "";
                        //token = Response.Content.ReadAsStringAsync().Result;
                        //
                        Succes = true;
                        return Page();
                    }
                    else
                    {
                        Message = Response.Content.ReadAsStringAsync().Result;
                        Succes = false;
                        return Page();

                    }

                }

            }





        }
    }
}
