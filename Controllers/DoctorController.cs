using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Hospital_Management_System.Controllers
{
    [Route("doctor")]
    public class DoctorController : Controller
    {
        private IConfiguration configuration;

        public DoctorController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        // ✅ Doctor List
        [HttpGet]
        [Route("doctor-list")]
       
        public IActionResult DoctorList(int? page, string searchTerm)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "PR_Doc_Doctor_SelectAll";

            // ✅ Only add parameter if SP supports it
            // if (!string.IsNullOrEmpty(searchTerm))
            //     command.Parameters.AddWithValue("@SearchTerm", searchTerm);

            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            connection.Close();

            // ✅ Manual Pagination
            var rows = table.AsEnumerable().ToList();
            int totalRecords = rows.Count;

            DataTable pagedTable = table.Clone();
            if (totalRecords > 0)
            {
                var pagedRows = rows.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                if (pagedRows.Any())
                    pagedTable = pagedRows.CopyToDataTable();
            }

            ViewBag.TotalItems = totalRecords;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;
            ViewBag.SearchTerm = searchTerm;

            return View(pagedTable);
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


        // ✅ Delete Multiple Doctors
        [HttpPost]
        [Route("delete-selected-doctors")]
        public IActionResult DeleteSelectedDoctors(List<int> SelectedDoctorIds)
        {
            if (SelectedDoctorIds == null || SelectedDoctorIds.Count == 0)
            {
                TempData["ErrorMessage"] = "No doctors selected for deletion.";
                return RedirectToAction("DoctorList");
            }

            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                foreach (var doctorId in SelectedDoctorIds)
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "PR_Doc_Doctor_Delete";
                    cmd.Parameters.AddWithValue("@DoctorID", doctorId);
                    cmd.ExecuteNonQuery();
                }

                connection.Close();
                TempData["SuccessMessage"] = "✅ Selected doctors deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "❌ Error: " + ex.Message;
            }

            return RedirectToAction("DoctorList");
        }

        // ✅ Delete Single Doctor
        [HttpGet]
        [Route("doctor-delete")]
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

                connection.Close();

                TempData["SuccessMessage"] = "✅ Doctor deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "❌ Error: " + ex.Message;
                Console.WriteLine(ex.ToString());
            }
            return RedirectToAction("DoctorList");
        }

        // ✅ Add/Edit Form
        [HttpGet]
        [Route("doctor-form")]
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
                    model.DoctorID = Convert.ToInt32(reader["DoctorID"]);
                    model.Name = reader["Name"].ToString();
                    model.Phone = reader["Phone"].ToString();
                    model.Email = reader["Email"].ToString();
                    model.Qualification = reader["Qualification"].ToString();
                    model.Specialization = reader["Specialization"].ToString();
                    model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                    model.UserID = Convert.ToInt32(reader["UserId"]);
                }
                UserDropDown();
                return View("DoctorAddEdit", model);
            }
            else
            {
                UserDropDown();
                return View("DoctorAddEdit", new DoctorAddEditModel());
            }
        }

        // ✅ Save Doctor
        [HttpPost]
        [Route("doctor-add-edit")]
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
                UserDropDown();
                return RedirectToAction("DoctorList");
            }
            return RedirectToAction("DoctorList");
        }
       

        [HttpGet("ExportToExcel")]
        public IActionResult ExportToExcel()
        {
            DataTable dt = new DataTable();

            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT DoctorID, Name, Phone, Email, Qualification, Specialization, IsActive FROM Doctor";

                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);

                connection.Close();

                using (var workbook = new ClosedXML.Excel.XLWorkbook())
                {
                    dt.TableName = "Doctors";
                    workbook.Worksheets.Add(dt);

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "DoctorsList.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting doctor data: " + ex.Message;
                return RedirectToAction("DoctorList");
            }
        }

        [HttpGet("ExportToCSV")]
        public IActionResult ExportToCSV()
        {
            DataTable dt = new DataTable();

            try
            {
                string connectionString = this.configuration.GetConnectionString("ConnectionString");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.Text;
                command.CommandText = "SELECT DoctorID, Name, Phone, Email, Qualification, Specialization, IsActive FROM Doctor";

                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);

                connection.Close();

                // ✅ Convert DataTable to CSV
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

                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "DoctorsList.csv");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting doctor data: " + ex.Message;
                return RedirectToAction("DoctorList");
            }
        }


    }
}
