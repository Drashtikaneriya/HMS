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
        public IActionResult AppointmentForm(int ID)
        {
            if (ID > 0)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_App_Appointment_SelectByPK";

                command.Parameters.AddWithValue("AppointmentID", ID);
                SqlDataReader reader = command.ExecuteReader();


                AppointmentAddEditModel model = new AppointmentAddEditModel();

                while (reader.Read())
                {
                    model.UserID = Convert.ToInt32(reader["UserID"]);
                    model.AppointmentID = Convert.ToInt32(reader["AppointmentID"]);
                    model.DoctorID = Convert.ToInt32(reader["DoctorID"]);
                    model.PatientID = Convert.ToInt32(reader["PatientID"]);
                    model.AppointmentDate = Convert.ToDateTime(reader["AppointmentDate"]);
                    model.AppointmentStatus = reader["AppointmentStatus"].ToString();
                    model.Description = reader["Description"].ToString();
                    model.SpecialRemarks = reader["SpecialRemarks"].ToString();
                    model.Created = Convert.ToDateTime(reader["Created"]);
                    model.UserID = Convert.ToInt32(reader["UserID"]);
                    model.TotalConsultedAmount = Convert.ToDecimal(reader["TotalConsultedAmount"]);
                }

                return View("AppointmentAddEdit", model);
            }
            else
            {
                return View("AppointmentAddEdit", new AppointmentAddEditModel());
            }
        }

        
        public IActionResult AppointmentAddEdit(AppointmentAddEditModel AppointmentAddEditModel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_App_Appointment_Insert";

                if (AppointmentAddEditModel.AppointmentID > 0)
                {
                    command.CommandText = "PR_App_Appointment_UpdateByPK";
                    command.Parameters.AddWithValue("AppointmentID", AppointmentAddEditModel.AppointmentID);
                }
                else
                {
                    command.CommandText = "PR_App_Appointment_Insert";
                }

                command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = AppointmentAddEditModel.DoctorID;
                command.Parameters.Add("@PatientID", SqlDbType.Int).Value = AppointmentAddEditModel.PatientID;
                command.Parameters.Add("@AppointmentDate", SqlDbType.DateTime).Value = AppointmentAddEditModel.AppointmentDate;
                command.Parameters.Add("@AppointmentStatus", SqlDbType.NVarChar).Value = AppointmentAddEditModel.AppointmentStatus;
                command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = AppointmentAddEditModel.Description;
                command.Parameters.Add("@SpecialRemarks", SqlDbType.NVarChar).Value = AppointmentAddEditModel.SpecialRemarks;
                command.Parameters.Add("@Modified", SqlDbType.DateTime).Value = DateTime.Now;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = AppointmentAddEditModel.UserID;
                command.Parameters.Add("@TotalConsultedAmount", SqlDbType.Decimal).Value = AppointmentAddEditModel.TotalConsultedAmount;
                command.ExecuteNonQuery();

                return RedirectToAction("AppointmentList");
            }
            return View("AppointmentAddEdit");
        }
    }
}
