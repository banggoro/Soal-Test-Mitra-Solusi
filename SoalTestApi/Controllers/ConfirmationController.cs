using Microsoft.AspNetCore.Mvc;
using SoalTest.Models;
using System.Text;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SoalTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfirmationController : Controller
    {
        private readonly IConfiguration _configuration;

        public ConfirmationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> Post(UserConfirmation _userData)
        {
            string message = "";
            if (_userData.Email != null && _userData.Guid != null)
            {
                bool valid = Update(_userData,out message);

                if (valid)
                {



                    return Ok();
                }
                else
                {
                    return BadRequest(message);
                }
            }
            else
            {
                return BadRequest();
            }
        }

        private bool Update(UserConfirmation _userData,out string message )
        {
            bool result = false;
           
            string sqlDataSource = _configuration.GetConnectionString("SoalTestDatabaseConn");
            SqlConnection cn = new SqlConnection();
            SqlDataAdapter da = new SqlDataAdapter();
            SqlCommand cm = new SqlCommand();
            SqlTransaction tr = null;
            DataTable dt = new DataTable();
            string query = "";

            try
            {

                cn.ConnectionString = sqlDataSource;
                cn.Open();
                tr = cn.BeginTransaction();
                cm.Transaction = tr;
                cm.Connection = cn;
                cm.CommandTimeout = 0;


                query = "select user_name from [user] ";
                query = query + "where user_email = '" + _userData.Email + "' ";
                query = query + "and user_guid = '" + _userData.Guid + "' ";

                dt = new DataTable();
                cm.CommandType = CommandType.Text;
                cm.CommandTimeout = 0;
                cm.CommandText = query;
                da.SelectCommand = cm;
                da.SelectCommand.CommandTimeout = 0;
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    query = @"UPDATE [user] SET user_confirmation = 1 ";
                    query = query + @"WHERE user_email = '" + _userData.Email + "' ";
                    query = query + @"AND user_guid = '" + _userData.Guid + "' ";
                    
                    cm.CommandType = CommandType.Text;
                    cm.CommandText = query;
                    cm.CommandTimeout = 0;
                    cm.ExecuteNonQuery();
                    tr.Commit();
                    message = "";
                    result = true;
                }
                else
                {
                    message = "Expiredlink";
                    tr.Rollback();
                    result = false;
                }





                


               
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
                message = e.Message;
                result = false;

            }


            return result;



        }
    }
}
