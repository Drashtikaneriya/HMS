using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{

    public class DoctorDepartmentController : Controller
    {
        private IConfiguration configuration;

        public DoctorDepartmentController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult DoctorDepartmentList()
        {

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_DocDept_DoctorDepartment_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);

        }
        public IActionResult DoctorDepartmentDelete(int DoctorDepartmentID)
        {
            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_DocDept_DoctorDepartment_Delete";
                command.Parameters.Add("@DoctorDepartmentID", SqlDbType.Int).Value = DoctorDepartmentID;
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("DoctorDepartmentList");
        }
        public IActionResult Index(DoctorDepartmentModelAddEdit DoctorDepartmentModelAddEdit)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_DocDept_DoctorDepartment_Insert";

                command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = DoctorDepartmentModelAddEdit.DoctorID;
                command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = DoctorDepartmentModelAddEdit.DepartmentID;
                command.Parameters.Add("@Modified", SqlDbType.DateTime).Value = DoctorDepartmentModelAddEdit.Modified;
                command.Parameters.Add("@UserID", SqlDbType.Int).Value = DoctorDepartmentModelAddEdit.UserID;

                command.ExecuteNonQuery();

                return RedirectToAction("DoctorDepartmentList");
            }
            return View("DoctorDepartmentAddEdit");
        }
    }
}
