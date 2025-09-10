using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        #region Doctor Drop Down
        public void DoctorDropDown()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_DOC_Doctor_SelectForDropDown";

                SqlDataReader reader = command.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                List<SelectListItem> doctorList = new List<SelectListItem>();
                foreach (DataRow data in dt.Rows)
                {
                    doctorList.Add(new SelectListItem
                    {
                        Value = data["DoctorID"].ToString(),
                        Text = data["DoctorName"].ToString()
                    });
                }

                ViewBag.DoctorList = doctorList;
            }
        }
        #endregion

        #region Department Drop Down
        public void DepartmentDropDown()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_DEPT_Department_SelectForDropDown";

                SqlDataReader reader = command.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                List<SelectListItem> departmentList = new List<SelectListItem>();
                foreach (DataRow data in dt.Rows)
                {
                    departmentList.Add(new SelectListItem
                    {
                        Value = data["DepartmentID"].ToString(),
                        Text = data["DepartmentName"].ToString()
                    });
                }

                ViewBag.DepartmentList = departmentList;
            }
        }
        #endregion

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

                // ✅ Success msg
                TempData["SuccessMessage"] = "Doctor-Department deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }

            return RedirectToAction("DoctorDepartmentList");
        }


        //#region Doctor Department Add Edit
        //[HttpGet]
        //public IActionResult DoctorDepartmentAddEdit()
        //{

        //    return View();
        //}

        //[HttpPost]
        //public IActionResult DoctorDepartmentAddEdit(DoctorDepartmentModelAddEdit DoctorDepartmentModelAddEdit)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        //        {
        //            Console.WriteLine("Model Error: " + error.ErrorMessage);
        //        }
        //    }

        //    Console.WriteLine(DoctorDepartmentModelAddEdit.DoctorDepartmentID);
        //    Console.WriteLine(DoctorDepartmentModelAddEdit.DoctorID);
        //    Console.WriteLine(DoctorDepartmentModelAddEdit.DepartmentID);
        //    Console.WriteLine(DoctorDepartmentModelAddEdit.Modified);
        //    Console.WriteLine(DoctorDepartmentModelAddEdit.UserID);
        //    if (DoctorDepartmentModelAddEdit.Modified == null)
        //    {
        //        DoctorDepartmentModelAddEdit.Modified = DateTime.Now;
        //    }
        //    Console.WriteLine("Mosified: " + DoctorDepartmentModelAddEdit.Modified);
        //    Console.WriteLine("DoctorDepartmentID", DoctorDepartmentModelAddEdit.DoctorDepartmentID);
        //    Console.WriteLine("DoctorID", DoctorDepartmentModelAddEdit.DoctorID);
        //    Console.WriteLine("DepartmentID", DoctorDepartmentModelAddEdit.DepartmentID);
        //    Console.WriteLine(DoctorDepartmentModelAddEdit.UserID);

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            string connectionString = this.configuration.GetConnectionString("ConnectionString");
        //            using (SqlConnection connection = new SqlConnection(connectionString))
        //            {
        //                connection.Open();
        //                using (SqlCommand command = connection.CreateCommand())
        //                {
        //                    command.CommandType = CommandType.StoredProcedure;
        //                    Console.WriteLine(DoctorDepartmentModelAddEdit.DoctorDepartmentID);
        //                    if (DoctorDepartmentModelAddEdit.DoctorDepartmentID != null && DoctorDepartmentModelAddEdit.DoctorDepartmentID > 0)
        //                    {
        //                        command.CommandText = "PR_DocDept_DoctorDepartment_Update";
        //                        command.Parameters.AddWithValue("@DoctorDepartmentID", DoctorDepartmentModelAddEdit.DoctorDepartmentID);
        //                    }
        //                    else
        //                    {
        //                        command.CommandText = "PR_DocDept_DoctorDepartment_Insert";
        //                    }

        //                    command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = DoctorDepartmentModelAddEdit.DoctorID;
        //                    command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = DoctorDepartmentModelAddEdit.DepartmentID;
        //                    command.Parameters.Add("@Modified", SqlDbType.DateTime).Value = DoctorDepartmentModelAddEdit.Modified;
        //                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = DoctorDepartmentModelAddEdit.UserID;

        //                    command.ExecuteNonQuery();
        //                }
        //            }
        //            DepartmentDropDown();
        //            return RedirectToAction("DoctorDepartmentList");
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["ErrorMessage"] = "Error saving Doctor-Department: " + ex.Message;
        //        }
        //    }
        //    DepartmentDropDown()

        //    return View("DoctorDepartmentAddEdit");
        //}
        //#endregion

        //#region Doctor Department Form Fill
        //public IActionResult DoctorDepartmentForm(int ID)
        //{
        //    DoctorDepartmentModelAddEdit model = new DoctorDepartmentModelAddEdit();

        //    if (ID > 0)
        //    {
        //        try
        //        {
        //            string connectionString = this.configuration.GetConnectionString("ConnectionString");
        //            using (SqlConnection connection = new SqlConnection(connectionString))
        //            {
        //                connection.Open();
        //                using (SqlCommand command = connection.CreateCommand())
        //                {
        //                    command.CommandType = CommandType.StoredProcedure;
        //                    command.CommandText = "PR_DocDept_DoctorDepartment_SelectByPK";
        //                    command.Parameters.AddWithValue("DoctorDepartmentID", ID);

        //                    using (SqlDataReader reader = command.ExecuteReader())
        //                    {
        //                        while (reader.Read())
        //                        {
        //                            model.UserID = Convert.ToInt32(reader["UserID"]);
        //                            model.DoctorID = Convert.ToInt32(reader["DoctorID"]);
        //                            model.DepartmentID = Convert.ToInt32(reader["DepartmentID"]);
        //                            model.DoctorDepartmentID = ID;
        //                            model.Modified = DateTime.Now;
        //                        }
        //                    }
        //                }
        //            }
        //            DepartmentDropDown();
        //            return View("DoctorDepartmentAddEdit", model);
        //        }
        //        catch (Exception ex)
        //        {
        //            TempData["ErrorMessage"] = "Error loading Doctor-Department for edit: " + ex.Message;
        //            return RedirectToAction("DoctorDepartmentList");
        //        }
        //    }
        //    else
        //    {
        //        DepartmentDropDown();
        //        return View("DoctorDepartmentList", model);
        //    }
        //}
        //#endregion
        #region Doctor Department Add Edit
        [HttpGet]
        public IActionResult DoctorDepartmentAddEdit()
        {
            DoctorDropDown();
            DepartmentDropDown();
            UserDropDown();
            return View();
        }

        [HttpPost]
        public IActionResult DoctorDepartmentAddEdit(DoctorDepartmentModelAddEdit DoctorDepartmentModelAddEdit)
        {
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
                            command.Parameters.Add("@Modified", SqlDbType.DateTime).Value = DoctorDepartmentModelAddEdit.Modified ?? DateTime.Now;
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

            // ⚠️ Important: Reload dropdowns if validation fails
            DoctorDropDown();
            DepartmentDropDown();
            UserDropDown();

            return View("DoctorDepartmentAddEdit", DoctorDepartmentModelAddEdit);
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

                    // ✅ Load all dropdowns
                    DoctorDropDown();
                    DepartmentDropDown();
                    UserDropDown();

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
                // ✅ Also load all dropdowns for add case
                DoctorDropDown();
                DepartmentDropDown();
                UserDropDown();

                return View("DoctorDepartmentAddEdit", model);
            }
        }
        #endregion

    }
}
