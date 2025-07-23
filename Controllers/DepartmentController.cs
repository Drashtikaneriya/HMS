using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    public class DepartmentController : Controller
    {
        private IConfiguration configuration;
        private readonly IConfiguration _configuration;

        public DepartmentController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult DepartmentList()
        {

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Dept_Department_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);

        }
        public IActionResult DepartmentDelete(int DepartmentID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Dept_Department_Delete";
                command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = DepartmentID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("DepartmentList");
        }
        public IActionResult DepartmentForm(int ID)
        {
            if (ID > 0)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_Dept_Department_SelectByPK";

                command.Parameters.AddWithValue("DepartmentID", ID);
                SqlDataReader reader = command.ExecuteReader();


                DepartmentAddEditModel model = new DepartmentAddEditModel();

                while (reader.Read())
                {
                    model.DepartmentID = ID;
                    model.UserID = Convert.ToInt32(reader["UserID"]);
                    model.DepartmentName = reader["DepartmentName"].ToString();
                    model.Description = reader["Description"].ToString();
                    model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                }

                return View("DepartmentAddEdit", model);
            }
            else
            {
                return View("DepartmentAddEdit", new DepartmentAddEditModel());
            }
        }
      
   
        public IActionResult DepartmentAddEdit(DepartmentAddEditModel DepartmentAddEditModel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
               
                if (DepartmentAddEditModel.DepartmentID > 0)
                {
                    command.CommandText = "PR_Dept_Department_UpdateByPK";
                    command.Parameters.AddWithValue("DepartmentID", DepartmentAddEditModel.DepartmentID);
                }
                else
                {
                    command.CommandText = "PR_Dept_Department_Insert";
                }

                command.Parameters.Add("@DepartmentName", SqlDbType.NVarChar).Value = DepartmentAddEditModel.DepartmentName;
                command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = DepartmentAddEditModel.Description;
                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = DepartmentAddEditModel.IsActive;
                command.Parameters.Add("@Modified", SqlDbType.DateTime).Value = DepartmentAddEditModel.Modified;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = DepartmentAddEditModel.UserID;

                command.ExecuteNonQuery();

                return RedirectToAction("DepartmentList");
            }
            return View("DepartmentAddEdit");
        }
    }
}
