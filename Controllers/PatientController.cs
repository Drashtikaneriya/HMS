using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public IActionResult PatientAddEdit(PatientAddEditModel PatientAddEditModel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                if (PatientAddEditModel.patientID > 0)
                {
                    command.CommandText = "PR_Pat_patient_UpdateByPK";
                    command.Parameters.AddWithValue("PatientID", PatientAddEditModel.patientID);
                }
                else
                {
                    command.CommandText = "PR_Pat_Patient_Insert";
                }
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
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = PatientAddEditModel.UserID;

                command.ExecuteNonQuery();

                return RedirectToAction("PatientList");
            }
            UserDropDown();
            return View("PatientAddEdit");
        }
        #region User Drop Down
        public void UserDropDown()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_USR_User_SelectForDropDown";

                SqlDataReader reader = command.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                List<SelectListItem> userList = new List<SelectListItem>();
                foreach (DataRow data in dt.Rows)
                {
                    userList.Add(new SelectListItem
                    {
                        Value = data["UserID"].ToString(),
                        Text = data["UserName"].ToString()
                    });
                }

                ViewBag.UserList = userList;
            }
        }

        #endregion

        public IActionResult PatientForm(int ID)
        {
            if (ID > 0)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Pat_Patient_SelectByPK";

                command.Parameters.AddWithValue("PatientID", ID);
                SqlDataReader reader = command.ExecuteReader();

                PatientAddEditModel model = new PatientAddEditModel();

                while (reader.Read())
                {
                    model.patientID = Convert.ToInt32(reader["PatientID"]);
                    model.UserID = Convert.ToInt32(reader["UserID"]);
                    model.City = reader["City"].ToString();
                    model.State = reader["State"].ToString();
                    model.Name = reader["Name"].ToString();
                    model.Gender = reader["Gender"].ToString();
                    model.Phone = reader["Phone"].ToString();
                    model.Email = reader["Email"].ToString();
                    model.Address = reader["Address"].ToString();
                    model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    model.DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]);
                    model.Modified = DateTime.Now;
                }
                UserDropDown();
                return View("PatientAddEdit", model);
            }
            else
            {
                UserDropDown();
                return View("PatientAddEdit", new PatientAddEditModel());
            }
        }


    }
}
