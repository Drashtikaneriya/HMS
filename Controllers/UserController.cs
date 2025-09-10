using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.Models;
using HMS.NewFolder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace HMS.Controllers
{
    //[CheckAccess]
    //[Route("user")]
    public class UserController : Controller
    {
        private readonly IConfiguration _configuration;

        public UserController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public IActionResult UserLogin(UserLoginModel userLoginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this._configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "PR_User_ValidateLogin";
                    sqlCommand.Parameters.Add("@Username", SqlDbType.VarChar).Value = userLoginModel.Username;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userLoginModel.Password;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(sqlDataReader);
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                            HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                            HttpContext.Session.SetString("EmailAddress", dr["Email"].ToString());
                        }

                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "User is not found";
                        return RedirectToAction("Login", "User");
                    }

                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return RedirectToAction("Login");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "User");
        }
        //[Route("Login")]
        public IActionResult Login()
        {
            return View();
        }
        //  User List with Pagination
        [HttpGet]
        //[Route("user-list")]
        public IActionResult UserList(int? page, string name, string email, string mobileNo)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            DataTable table = new DataTable();

            string connectionString = this._configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand("PR_User_User_SelectAll", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Pass parameters (if empty, send NULL)
                    command.Parameters.AddWithValue("@Name", string.IsNullOrEmpty(name) ? DBNull.Value : name);
                    command.Parameters.AddWithValue("@Email", string.IsNullOrEmpty(email) ? DBNull.Value : email);
                    command.Parameters.AddWithValue("@MobileNo", string.IsNullOrEmpty(mobileNo) ? DBNull.Value : mobileNo);

                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    table.Load(reader);
                }
            }

            // ✅ Manual pagination
            var rows = table.AsEnumerable().ToList();
            int totalRecords = rows.Count;

            DataTable pagedTable = table.Clone();
            if (totalRecords > 0)
            {
                var pagedRows = rows.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                if (pagedRows.Any())
                    pagedTable = pagedRows.CopyToDataTable();
            }

            // ✅ Pass pagination + filter values
            ViewBag.TotalItems = totalRecords;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;
            ViewBag.Name = name;
            ViewBag.Email = email;
            ViewBag.MobileNo = mobileNo;

            return View(pagedTable);
        }

        //public IActionResult UserList(int? page)
        //{
        //    int pageSize = 5;
        //    int pageNumber = page ?? 1;

        //    DataTable table = new DataTable();

        //    string connectionString = this._configuration.GetConnectionString("ConnectionString");
        //    SqlConnection connection = new SqlConnection(connectionString);
        //    connection.Open();
        //    SqlCommand command = connection.CreateCommand();
        //    command.CommandType = CommandType.StoredProcedure;
        //    command.CommandText = "PR_User_User_SelectAll";
        //    SqlDataReader reader = command.ExecuteReader();
        //    table.Load(reader);

        //    //// ✅ Search filter in C#
        //    //if (!string.IsNullOrEmpty(searchTerm))
        //    //{
        //    //    var filteredRows = table.AsEnumerable()
        //    //        .Where(r => r["UserName"].ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
        //    //                 || r["Email"].ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
        //    //        .ToList();

        //    //    table = filteredRows.Any() ? filteredRows.CopyToDataTable() : table.Clone();
        //    //}

        //    // ✅ Manual pagination
        //    var rows = table.AsEnumerable().ToList();
        //    int totalRecords = rows.Count;

        //    DataTable pagedTable = table.Clone();
        //    if (totalRecords > 0)
        //    {
        //        var pagedRows = rows.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        //        if (pagedRows.Any())
        //            pagedTable = pagedRows.CopyToDataTable();
        //    }

        //    //// Pass pagination info
        //    ViewBag.TotalItems = totalRecords;
        //    ViewBag.PageSize = pageSize;
        //    ViewBag.PageNumber = pageNumber;

        //    return View(pagedTable);
        //}

        //  User Form (Edit / Add)
        //[Route("user-edit")]
        //public IActionResult UserForm(int ID)
        //{

        //    if (ID > 0)
        //    {

        //        string connectionString = this._configuration.GetConnectionString("ConnectionString");
        //        SqlConnection connection = new SqlConnection(connectionString);
        //        connection.Open();
        //        SqlCommand command = connection.CreateCommand();
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.CommandText = "PR_User_User_SelectByPK";
        //        command.Parameters.AddWithValue("@UserID", ID);
        //        SqlDataReader reader = command.ExecuteReader();

        //        UserAddEditModel model = new UserAddEditModel();
        //        while (reader.Read())
        //        {
        //            model.UserID = Convert.ToInt32(reader["UserID"]);
        //            model.UserName = reader["UserName"].ToString();
        //            model.Password = reader["Password"].ToString();
        //            model.Email = reader["Email"].ToString();
        //            model.MobileNo = reader["MobileNo"].ToString();
        //            model.IsActive = Convert.ToBoolean(reader["IsActive"]);
        //        }

        //        return View("UserAddEdit", model);
        //    }
        //    else
        //    {
        //        return View("UserAddEdit", new UserAddEditModel());
        //    }
        //}
        public IActionResult UserForm(string ID)
        {
            int decryptedUserID = 0;

            if (!string.IsNullOrEmpty(ID))
            {
                decryptedUserID = Convert.ToInt32(UrlEncryptor.Decrypt(ID));
            }

            if (decryptedUserID > 0)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_User_User_SelectByPK";
                    command.Parameters.AddWithValue("@UserID", decryptedUserID);

                    SqlDataReader reader = command.ExecuteReader();

                    UserAddEditModel model = new UserAddEditModel();
                    while (reader.Read())
                    {
                        model.UserID = Convert.ToInt32(reader["UserID"]);
                        model.UserName = reader["UserName"].ToString();
                        model.Password = reader["Password"].ToString();
                        model.Email = reader["Email"].ToString();
                        model.MobileNo = reader["MobileNo"].ToString();
                        model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    }

                    return View("UserAddEdit", model);
                }
            }
            else
            {
                return View("UserAddEdit", new UserAddEditModel());
            }
        }

        // User Add/Edit (Insert or Update)
        //[Route("user-edit-add")]
        public IActionResult UserAddEdit(UserAddEditModel UserAddEditModel)
        {
            if (ModelState.IsValid)
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (UserAddEditModel.UserID > 0)
                {
                    // ✅ Update case
                    command.CommandText = "PR_User_User_UpdateByPK";
                    command.Parameters.AddWithValue("@UserID", UserAddEditModel.UserID);
                }
                else
                {
                    // ✅ Insert case
                    command.CommandText = "PR_User_User_Insert";
                    command.Parameters.AddWithValue("@Created", DateTime.Now);
                }

                command.Parameters.AddWithValue("@UserName", UserAddEditModel.UserName);
                command.Parameters.AddWithValue("@Password", UserAddEditModel.Password);
                command.Parameters.AddWithValue("@Email", UserAddEditModel.Email);
                command.Parameters.AddWithValue("@MobileNo", UserAddEditModel.MobileNo);
                command.Parameters.AddWithValue("@IsActive", UserAddEditModel.IsActive);
                command.Parameters.AddWithValue("@Modified", DateTime.Now);

                command.ExecuteNonQuery();

                // ✅ Success message set karo
                if (UserAddEditModel.UserID > 0)
                {
                    TempData["SuccessMessage"] = "✅ User updated successfully.";
                }
                else
                {
                    TempData["SuccessMessage"] = "✅ User inserted successfully.";
                }

                return RedirectToAction("UserList");
            }

            return View("UserAddEdit");
        }


        //  Delete All Users
        [HttpPost]
        //[Route("delete-all")]
        public IActionResult DeleteAllUsers()
        {
            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_User_DeleteAll";
                command.ExecuteNonQuery();

                TempData["SuccessMessage"] = "✅ All users deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }

            return RedirectToAction("UserList");
        }

        //  Delete Selected Users
        [HttpPost]
        //[Route("DeleteSelectedUsers")]
        public IActionResult DeleteSelectedUsers(List<int> SelectedUserIds)
        {
            if (SelectedUserIds == null || SelectedUserIds.Count == 0)
            {
                TempData["ErrorMessage"] = "No users selected for deletion.";
                return RedirectToAction("UserList");
            }

            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                foreach (var userId in SelectedUserIds)
                {
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_User_User_Delete";
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Selected users deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error: " + ex.Message;
            }

            return RedirectToAction("UserList");
        }
        public IActionResult UserDelete(string UserID)
        {
            try
            {
                int decryptedUserID = Convert.ToInt32(UrlEncryptor.Decrypt(UserID));

                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "PR_User_User_Delete";
                        command.Parameters.Add("@UserID", SqlDbType.Int).Value = decryptedUserID;
                        command.ExecuteNonQuery();
                    }
                }

                TempData["SuccessMessage"] = "✅ User Deleted Successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction("UserList");
        }

        //#region Delete Single User
        ////[Route("user-delete")]
        //public IActionResult UserDelete(int UserID)
        //{
        //    try
        //    {
        //        string connectionString = this._configuration.GetConnectionString("ConnectionString");
        //        SqlConnection connection = new SqlConnection(connectionString);
        //        connection.Open();
        //        SqlCommand command = connection.CreateCommand();
        //        command.CommandType = CommandType.StoredProcedure;
        //        command.CommandText = "PR_User_User_Delete";
        //        command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;
        //        command.ExecuteNonQuery();
        //        TempData["SuccessMessage"] = "✅ User Deleted Successfully.";
        //    }
        //    catch (Exception ex)
        //    {
        //        TempData["ErrorMessage"] = ex.Message;
        //        Console.WriteLine(ex.ToString());
        //    }
        //    return RedirectToAction("UserList");
        //}
        //#endregion

        // Export to Excel
        [HttpGet("ExportToExcel")]
        public IActionResult ExportToExcel()
        {
            DataTable dt = new DataTable();

            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT UserID, UserName, MobileNO, Email, IsActive, Created, Modified FROM [User]";
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);

                using (var workbook = new XLWorkbook())
                {
                    dt.TableName = "Users";
                    workbook.Worksheets.Add(dt);

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "UsersList.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting data: " + ex.Message;
                return RedirectToAction("UserList");
            }
        }

        //  Export to CSV
        [HttpGet("ExportToCSV")]
        public IActionResult ExportToCSV()
        {
            DataTable dt = new DataTable();

            try
            {
                string connectionString = this._configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "SELECT UserID, UserName, Email, IsActive, Created, Modified FROM [User]";
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);

                StringBuilder sb = new StringBuilder();

                // Add column headers
                IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                sb.AppendLine(string.Join(",", columnNames));

                // Add rows
                foreach (DataRow row in dt.Rows)
                {
                    IEnumerable<string> fields = row.ItemArray.Select(field => "\"" + field.ToString().Replace("\"", "\"\"") + "\"");
                    sb.AppendLine(string.Join(",", fields));
                }

                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "UsersList.csv");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting data: " + ex.Message;
                return RedirectToAction("UserList");
            }
        }
    }
}
