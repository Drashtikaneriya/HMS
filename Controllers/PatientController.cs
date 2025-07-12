using HMS.Models;
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
        public IActionResult Index(PatientAddEditModel PatientAddEditModel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Pat_Patient_Insert";

                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = PatientAddEditModel.Name;
                 command.Parameters.Add("@DateOfBirth", SqlDbType.DateTime).Value = PatientAddEditModel.DateOfBirth;
                command.Parameters.Add("@Gender", SqlDbType.NVarChar).Value = PatientAddEditModel.Gender;
                command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = PatientAddEditModel.Email;
                command.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = PatientAddEditModel.Phone;
                command.Parameters.Add("@Address", SqlDbType.NVarChar).Value = PatientAddEditModel.Address;
                command.Parameters.Add("@City", SqlDbType.NVarChar).Value = PatientAddEditModel.City;
                command.Parameters.Add("@State", SqlDbType.NVarChar).Value = PatientAddEditModel.State;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = PatientAddEditModel.IsActive;

                command.Parameters.Add("@Modified", SqlDbType.DateTime).Value = PatientAddEditModel.Modified;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = PatientAddEditModel.UserId;

                command.ExecuteNonQuery();

                return RedirectToAction("PatientList");
            }
            return View("PatientAddEdit");
        }
    }
}
