using HMS.Models;
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
        public IActionResult Index(AppointmentAddEditModel AppointmentAddEditModel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_App_Appointment_Insert";

                command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = AppointmentAddEditModel.DoctorID;
                command.Parameters.Add("@PatientID", SqlDbType.Int).Value = AppointmentAddEditModel.patientID;
                command.Parameters.Add("@AppointmentDate", SqlDbType.DateTime).Value = AppointmentAddEditModel.AppointmentDate;
                command.Parameters.Add("@AppointmentStatus", SqlDbType.NVarChar).Value = AppointmentAddEditModel.AppointmentStatus;
                command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = AppointmentAddEditModel.Description;
                command.Parameters.Add("@SpecialRemarks", SqlDbType.NVarChar).Value = AppointmentAddEditModel.SpecialRemarks;
                command.Parameters.Add("@Created", SqlDbType.DateTime).Value = AppointmentAddEditModel.Created;
                command.Parameters.Add("@Modified", SqlDbType.DateTime).Value = AppointmentAddEditModel.Modified;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = AppointmentAddEditModel.UserID;
                command.Parameters.Add("@TotalConsultedAmount", SqlDbType.Decimal).Value = AppointmentAddEditModel.TotalConsultedAmount;
                command.ExecuteNonQuery();

                return RedirectToAction("AppointmentList");
            }
            return View("AppointmentAddEdit");
        }
    }
}
