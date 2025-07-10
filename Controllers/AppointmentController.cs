using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    public class AppointmentController : Controller
    {
        private IConfiguration configuration;

        public AppointmentController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult AppointmentList()
        {

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_App_Appointment_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);

        }
        public IActionResult AppointmentDelete(int AppointmentID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_App_Appointment_Delete";
                command.Parameters.Add("@AppointmentID", SqlDbType.Int).Value = AppointmentID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("AppointmentList");
        }
        public IActionResult AppointmentAddEdit()
        {
            return View();
        }
    }
}
