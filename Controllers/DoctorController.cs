
using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace Hospital_Management_System.Controllers
{
    public class DoctorController : Controller
    {
        private IConfiguration configuration;

        public DoctorController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult DoctorList()
        {

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Doc_Doctor_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);

        }
        public IActionResult DoctorDelete(int DoctorID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Doc_Doctor_Delete";
                command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = DoctorID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("DoctorList");
        }
        
        public IActionResult DoctorForm(int ID)
        {

            if (ID > 0)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Doc_Doctor_SelectByPK";

                command.Parameters.AddWithValue("DoctorID", ID);
                SqlDataReader reader = command.ExecuteReader();


                DoctorAddEditModel model = new DoctorAddEditModel();

                while (reader.Read())
                {
                    model.DoctorID=Convert.ToInt32(reader["DoctorID"]);
                    model.Name = reader["Name"].ToString();
                    model.Phone = reader["Phone"].ToString();
                    model.Email = reader["Email"].ToString();
                    model.Qualification = reader["Qualification"].ToString();
                    model.Specialization = reader["Specialization"].ToString();
                    model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    model.UserID=Convert.ToInt32(reader["UserId"]);
                }

                return View("DoctorAddEdit", model);
            }
            else
            {
                return View("DoctorAddEdit", new DoctorAddEditModel());
            }
        }

        
        public IActionResult DoctorAddEdit(DoctorAddEditModel DoctorAddEditModel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (DoctorAddEditModel.DoctorID > 0)
                {
                    command.CommandText = "PR_Doc_Doctor_UpdateByPK";
                    command.Parameters.AddWithValue("DoctorID", DoctorAddEditModel.DoctorID);
                }
                else
                {
                    command.CommandText = "PR_Doc_Doctor_Insert";
                }

                command.Parameters.Add("@Name", SqlDbType.NVarChar).Value = DoctorAddEditModel.Name;
                command.Parameters.Add("@Phone", SqlDbType.NVarChar).Value = DoctorAddEditModel.Phone;
                command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = DoctorAddEditModel.Email;
                command.Parameters.Add("@Qualification", SqlDbType.NVarChar).Value = DoctorAddEditModel.Qualification;
                command.Parameters.Add("@Specialization", SqlDbType.NVarChar).Value = DoctorAddEditModel.Specialization;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = DoctorAddEditModel.IsActive;
                command.Parameters.Add("@Modified", SqlDbType.DateTime).Value = DoctorAddEditModel.Modified;
                command.Parameters.Add("@UserId", SqlDbType.Int).Value = DoctorAddEditModel.UserID;

                command.ExecuteNonQuery();

                return RedirectToAction("DoctorList");
            }
            return RedirectToAction("DoctorList");
        }


    }
}
