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
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            SqlCommand command = new SqlCommand("PR_DocDept_DoctorDepartment_SelectAll", connection);
            command.CommandType = CommandType.StoredProcedure;

            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }

        public IActionResult DoctorDepartmentDelete(int DoctorDepartmentID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = new SqlCommand("PR_DocDept_DoctorDepartment_Delete", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.Add("@DoctorDepartmentID", SqlDbType.Int).Value = DoctorDepartmentID;

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("DoctorDepartmentList");
        }


        #region Doctor Department Add Edit
        [HttpGet]
        public IActionResult DoctorDepartmentAddEdit()
        {
            return View();
        }

        [HttpPost]
        public IActionResult DoctorDepartmentAddEdit(DoctorDepartmentModelAddEdit DoctorDepartmentModelAddEdit)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine("Model Error: " + error.ErrorMessage);
                }
            }

            Console.WriteLine(DoctorDepartmentModelAddEdit.DoctorDepartmentID);
            Console.WriteLine(DoctorDepartmentModelAddEdit.DoctorID);
            Console.WriteLine(DoctorDepartmentModelAddEdit.DepartmentID);
            Console.WriteLine(DoctorDepartmentModelAddEdit.Modified);
            Console.WriteLine(DoctorDepartmentModelAddEdit.UserID);
            if (DoctorDepartmentModelAddEdit.Modified == null)
            {
                DoctorDepartmentModelAddEdit.Modified = DateTime.Now;
            }
            Console.WriteLine("Mosified: " + DoctorDepartmentModelAddEdit.Modified);
            Console.WriteLine("DoctorDepartmentID", DoctorDepartmentModelAddEdit.DoctorDepartmentID);
            Console.WriteLine("DoctorID", DoctorDepartmentModelAddEdit.DoctorID);
            Console.WriteLine("DepartmentID", DoctorDepartmentModelAddEdit.DepartmentID);
            Console.WriteLine(DoctorDepartmentModelAddEdit.UserID);

            if (ModelState.IsValid)
            {
                try
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            Console.WriteLine(DoctorDepartmentModelAddEdit.DoctorDepartmentID);
                            if (DoctorDepartmentModelAddEdit.DoctorDepartmentID != null && DoctorDepartmentModelAddEdit.DoctorDepartmentID > 0)
                            {
                                command.CommandText = "PR_DocDept_DoctorDepartment_Update";
                                command.Parameters.AddWithValue("@DoctorDepartmentID", DoctorDepartmentModelAddEdit.DoctorDepartmentID);
                            }
                            else
                            {
                                command.CommandText = "PR_DocDept_DoctorDepartment_Insert";
                            }

                            command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = DoctorDepartmentModelAddEdit.DoctorID;
                            command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = DoctorDepartmentModelAddEdit.DepartmentID;
                            command.Parameters.Add("@Modified", SqlDbType.DateTime).Value = DoctorDepartmentModelAddEdit.Modified;
                            command.Parameters.Add("@UserID", SqlDbType.Int).Value = DoctorDepartmentModelAddEdit.UserID;

                            command.ExecuteNonQuery();
                        }
                    }

                    return RedirectToAction("DoctorDepartmentList");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error saving Doctor-Department: " + ex.Message;
                }
            }

            return View("DoctorDepartmentAddEdit");
        }
        #endregion

        #region Doctor Department Form Fill
        public IActionResult DoctorDepartmentForm(int ID)
        {
            DoctorDepartmentModelAddEdit model = new DoctorDepartmentModelAddEdit();

            if (ID > 0)
            {
                try
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        using (SqlCommand command = connection.CreateCommand())
                        {
                            command.CommandType = CommandType.StoredProcedure;
                            command.CommandText = "PR_DocDept_DoctorDepartment_SelectByPK";
                            command.Parameters.AddWithValue("DoctorDepartmentID", ID);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    model.UserID = Convert.ToInt32(reader["UserID"]);
                                    model.DoctorID = Convert.ToInt32(reader["DoctorID"]);
                                    model.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
                                    model.DoctorDepartmentID = ID;
                                    model.Modified = DateTime.Now;
                                }
                            }
                        }
                    }

                    return View("DoctorDepartmentAddEdit", model);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error loading Doctor-Department for edit: " + ex.Message;
                    return RedirectToAction("DoctorDepartmentList");
                }
            }
            else
            {
                return View("DoctorDepartmentList", model);
            }
        }
        #endregion

    }
}
