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
        [HttpPost]
        public IActionResult DoctorDepartmentAddEdit(DoctorDepartmentModelAddEdit DoctorDepartmentModelAddEdit)
        {
            if (!ModelState.IsValid)
            {
                return View("DoctorDepartmentAddEdit", DoctorDepartmentModelAddEdit);
            }

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            if (DoctorDepartmentModelAddEdit.DoctorDepartmentID > 0)
            {
                command.CommandText = "PR_DocDept_DoctorDepartmen_UpdateByPK";
                command.Parameters.AddWithValue("@DoctorDepartmentID", DoctorDepartmentModelAddEdit.DoctorDepartmentID);
            }
            else
            {
                command.CommandText = "PR_DocDept_DoctorDepartment_Insert";
            }

            command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = DoctorDepartmentModelAddEdit.DoctorID;
            command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = DoctorDepartmentModelAddEdit.DepartmentID;
            command.Parameters.Add("@Created", SqlDbType.DateTime).Value = DoctorDepartmentModelAddEdit.Created == DateTime.MinValue ? DateTime.Now : DoctorDepartmentModelAddEdit.Created;
            command.Parameters.Add("@Modified", SqlDbType.DateTime).Value = DateTime.Now;
            command.Parameters.Add("@UserID", SqlDbType.Int).Value = DoctorDepartmentModelAddEdit.UserID;

            command.ExecuteNonQuery();

            return RedirectToAction("DoctorDepartmentList");
        }

        //public IActionResult DoctorDepartmentAddEdit(DoctorDepartmentModelAddEdit DoctorDepartmentModelAddEdit)
        //{
        //    if (DoctorDepartmentModelAddEdit.DoctorDepartmentID == 0)
        //    {
        //        return View("DoctorDepartmentAddEdit");

        //        }
        //        if (ModelState.IsValid)
        //          {
        //        string connectionString = this.configuration.GetConnectionString("ConnectionString");
        //        SqlConnection connection = new SqlConnection(connectionString);
        //        connection.Open();
        //        SqlCommand command = connection.CreateCommand();
        //        command.CommandType = CommandType.StoredProcedure;


        //        if (DoctorDepartmentModelAddEdit.DoctorDepartmentID > 0)
        //        {
        //            command.CommandText = "PR_DocDept_DoctorDepartmen_UpdateByPK";
        //            command.Parameters.AddWithValue("DoctorDepartmentID", DoctorDepartmentModelAddEdit.DoctorDepartmentID);
        //        }
        //        else
        //        {
        //            command.CommandText = "PR_DocDept_DoctorDepartment_Insert";
        //        }
        //        command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = DoctorDepartmentModelAddEdit.DoctorID;
        //        command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = DoctorDepartmentModelAddEdit.DepartmentID;
        //        command.Parameters.Add("@Created", SqlDbType.DateTime).Value = DateTime.Now;
        //        command.Parameters.Add("@Modified", SqlDbType.DateTime).Value = DateTime.Now;
        //        command.Parameters.Add("@UserID", SqlDbType.Int).Value = DoctorDepartmentModelAddEdit.UserID;

        //        command.ExecuteNonQuery();

        //        return RedirectToAction("DoctorDepartmentList");
        //    }
        //    return View("DoctorDepartmentList");
        //}
        public IActionResult DoctorDepartmentForm(int ID)
        {

            if (ID > 0)
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_DocDept_DoctorDepartment_SelectByPK";

                command.Parameters.AddWithValue("DoctorDepartmentID", ID);
                SqlDataReader reader = command.ExecuteReader();


                DoctorDepartmentModelAddEdit model = new DoctorDepartmentModelAddEdit();

                while (reader.Read())
                {
                    model.UserID = Convert.ToInt32(reader["UserID"]);
                    model.DoctorID = Convert.ToInt32(reader["DoctorID"]);
                    model.DepartmentID= Convert.ToInt32(reader["DepartmentID"]);
                    model.Modified = DateTime.Now;
                }

                return View("DoctorDepartmentAddEdit", model);
            }
            else
            {
                return View("DoctorDepartmentList", new DoctorDepartmentModelAddEdit());
            }
        }
    }
}
