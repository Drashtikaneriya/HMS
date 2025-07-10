using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    public class PatientController : Controller
    {
        private IConfiguration configuration;

        public PatientController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult PatientList()
        {

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Pat_Patient_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);

        }
        public IActionResult PatientDelete(int PatientID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Pat_Patient_Delete";
                command.Parameters.Add("@PatientID", SqlDbType.Int).Value = PatientID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("PatientList");
        }
        public IActionResult Index()
        {
            return View("PatientAddEdit");
        }

    }
}
