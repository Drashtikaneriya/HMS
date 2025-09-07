using ClosedXML.Excel;
using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

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
        public IActionResult DepartmentList(int? page)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            DataTable table = new DataTable();

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Dept_Department_SelectAll";

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

            // ✅ Pass pagination info
            ViewBag.TotalItems = totalRecords;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;

            return View(pagedTable);
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
     
        public IActionResult DeleteSelectedDepartments(List<int> SelectedUserIds)
        {
            if (SelectedUserIds == null || SelectedUserIds.Count == 0)
            {
                TempData["ErrorMessage"] = "No users selected for deletion.";
                return RedirectToAction("DepartmentList");
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


                DepartmentAddEditModel model = new DepartmentAddEditModel
                {
                    DepartmentName = "", // temporary value
                    Description = ""     // temporary value
                };


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
                return View("DepartmentAddEdit", new DepartmentAddEditModel
                {
                    DepartmentName = "",
                    Description = ""
                });
            }
        }

        public IActionResult DepartmentAddEdit(DepartmentAddEditModel departmentModel)
        {
            if (!ModelState.IsValid)
            {
                // View what's going wrong
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                return View("DepartmentAddEdit", departmentModel);
            }

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (departmentModel.DepartmentID == null)
                {
                    command.CommandText = "PR_Dept_Department_Insert";
                }
                else
                {
                    command.CommandText = "PR_Dept_Department_UpdateByPK";
                    command.Parameters.Add("@DepartmentID", SqlDbType.Int).Value = departmentModel.DepartmentID;
                }

                departmentModel.Modified = DateTime.Now;

                command.Parameters.AddWithValue("@DepartmentName", departmentModel.DepartmentName);
                command.Parameters.AddWithValue("@Description", departmentModel.Description);
                command.Parameters.AddWithValue("@IsActive", departmentModel.IsActive);
                command.Parameters.AddWithValue("@Modified", departmentModel.Modified);
                command.Parameters.AddWithValue("@UserID", departmentModel.UserID);

                command.ExecuteNonQuery();
            }

            return RedirectToAction("DepartmentList");
        }
        [HttpGet("ExportDepartmentToExcel")]
        public IActionResult ExportDepartmentToExcel()
        {
            DataTable dt = new DataTable();

            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Dept_Department_SelectAll";
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                }

                using (var workbook = new XLWorkbook())
                {
                    dt.TableName = "Departments";
                    workbook.Worksheets.Add(dt);

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "DepartmentsList.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting department data: " + ex.Message;
                return RedirectToAction("DepartmentList");
            }
        }

        [HttpGet("ExportDepartmentToCSV")]
        public IActionResult ExportDepartmentToCSV()
        {
            DataTable dt = new DataTable();

            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_Dept_Department_SelectAll";
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
                }

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

                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "DepartmentsList.csv");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting department data: " + ex.Message;
                return RedirectToAction("DepartmentList");
            }
        }


    }
}
