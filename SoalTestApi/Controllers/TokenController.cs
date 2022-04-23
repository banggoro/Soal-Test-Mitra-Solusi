
using Microsoft.AspNetCore.Mvc;
using SoalTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SoalTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        [HttpPost]
        public async Task<IActionResult> Post(UserInfo _userData)
        {
            if (_userData.UserName != null && _userData.Password != null)
            {
                bool valid = GetUser(_userData.UserName, _userData.Password);

                if (valid)
                {
                    
                    var claims = new[] {
                        new Claim("UserName",_userData.UserName)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        null,
                        null,
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn);

                    var result = new JwtSecurityTokenHandler().WriteToken(token);
                    return Ok(result);
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        private bool GetUser(string username, string password)
        {
            string query = @"select user_name,user_password,user_confirmation from dbo.[user] ";
            query = query + @"where user_name = '" + username + "' ";
            bool result = false;


            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("SoalTestDatabaseConn");

            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            if (table.Rows.Count > 0)
            {
                if (Convert.ToBoolean(table.Rows[0]["user_confirmation"]))
                {
                    if (table.Rows[0]["user_password"].ToString() == password)
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return result;


        }


    }

}
