using Microsoft.AspNetCore.Mvc;
using SoalTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Net.Mail;
using System.Net.Mime;

namespace SoalTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public RegisterController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost]
        public async Task<IActionResult> Post(UserInfo _userData)
        {
            if (_userData.Email != null && _userData.Password != null && _userData.UserName != null && _userData.FullName != null)
            {
                bool valid = SaveData(_userData);

                if (valid)
                {
                    

                    
                    return Ok();
                }
                else
                {
                    return BadRequest("InvalidData");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        private bool SaveData(UserInfo _userData)
        {
            bool result = false;
            Guid guid = Guid.NewGuid();
            string sqlDataSource = _configuration.GetConnectionString("SoalTestDatabaseConn");
            SqlConnection cn = new SqlConnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cm = new SqlCommand();
            SqlTransaction tr = null;

            try
            {
                string query = @"INSERT INTO [user] (user_email, user_name, user_fullname, user_password,user_guid) VALUES (";
                query = query + @"'" + _userData.Email + "' ";
                query = query + @",'" + _userData.UserName + "' ";
                query = query + @",'" + _userData.FullName + "' ";
                query = query + @",'" + _userData.Password + "' ";
                query = query + @",'" + guid.ToString() + "' ";
                query = query + @")";

              


                cn.ConnectionString = sqlDataSource;
                cn.Open();
                tr = cn.BeginTransaction();
                cm.Transaction = tr;
                cm.Connection = cn;
                cm.CommandTimeout = 0;

                cm.CommandType = CommandType.Text;
                cm.CommandText = query;
                cm.CommandTimeout = 0;
                cm.ExecuteNonQuery();


                string htmlBody = "";


                htmlBody = @"<html>";
                htmlBody = htmlBody + @"<head>";
                htmlBody = htmlBody + @"</head>";
                htmlBody = htmlBody + @"<body>";
                htmlBody = htmlBody + @"<div style=" + Convert.ToChar(34) + "padding-top:100px;padding-bottom:100px;background-color:silver;height:100%;width:100%;font-family:Helvetica;color:gray" + Convert.ToChar(34) + ">";
                htmlBody = htmlBody + @"<center>";
                htmlBody = htmlBody + @"<div style=" + Convert.ToChar(34) + "width:30%;min-width:300px;max-width:800px;background-color:white;padding:20px;border-radius: 10px;" + Convert.ToChar(34) + ">";
                htmlBody = htmlBody + @"<div style=""background-color:white;padding: 0px"">";
                htmlBody = htmlBody + @"<hr>";
                htmlBody = htmlBody + @"</div>";
                htmlBody = htmlBody + @"<div style=""text-align:left; min-height:300px"">";
                htmlBody = htmlBody + @"<h1>Selamat bergabung !</h1>";
                htmlBody = htmlBody + @"<p>";
                htmlBody = htmlBody + @"Silahkan klik link " + @"https://localhost:7020/confirmation?email=" + _userData.Email + @"&guid=" + guid.ToString();
                htmlBody = htmlBody + @"</p>";
                htmlBody = htmlBody + @"</div>";
                htmlBody = htmlBody + @"</body>";
                htmlBody = htmlBody + "</html>";


                MailMessage mail = new MailMessage();

               

                mail.From = new MailAddress(_configuration.GetSection("MailSettings").GetSection("Mail").Value);
                mail.To.Add(_userData.Email);
                mail.Subject = "Selamat bergabung di Soal Test !";



                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(
                                              htmlBody,
                                              Encoding.UTF8,
                                              MediaTypeNames.Text.Html);

                mail.AlternateViews.Add(htmlView);
                mail.IsBodyHtml = true;


                SmtpClient SmtpServer = new SmtpClient(_configuration.GetSection("MailSettings").GetSection("Host").Value);
                SmtpServer.Port = Convert.ToInt16(_configuration.GetSection("MailSettings").GetSection("Port").Value);
                SmtpServer.Credentials = new System.Net.NetworkCredential(_configuration.GetSection("MailSettings").GetSection("Mail").Value, _configuration.GetSection("MailSettings").GetSection("Password").Value);
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);

                tr.Commit();


                result = true;
               
            }
            catch (Exception e)
            {

                try
                {
                    tr.Rollback();
                }
                catch (Exception ex)
                {

                }
                result = false;
               
            }
            

            return result;



        }

    }
}
