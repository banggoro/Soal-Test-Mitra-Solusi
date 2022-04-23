using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace SoalTestFrontEnd.Pages
{

    public class ConfirmationType
    {
        public string Email { get; set; }
        public string Guid { get; set; }

    }
    public class ConfirmationModel : PageModel
    {
        public ConfirmationType Confirmation { get; set; }
        public bool Succes { get; set; }
        public string Message { get; set; }


        public ConfirmationModel()
        {
            Succes = false;
        }

        public async Task<IActionResult> OnGet()
        {
            Confirmation = new ConfirmationType();
            Confirmation.Email = HttpContext.Request.Query["email"].ToString();
            Confirmation.Guid = HttpContext.Request.Query["guid"].ToString();

            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(Confirmation), System.Text.Encoding.UTF8, "application/json");

                string endpoint = "https://localhost:7184/api/confirmation";

                using (var Response = await client.PostAsync(endpoint, content))
                {
                    if (Response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        Message = "";
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
