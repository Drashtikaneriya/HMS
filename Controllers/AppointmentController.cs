using ClosedXML.Excel;
using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using X.PagedList.Extensions;

namespace HMS.Controllers
{
    [Route("appointment")] // ✅ Base route
    public class AppointmentController : Controller
    {
        private readonly IConfiguration _configuration;

        public AppointmentController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        #region Appointment List
        [HttpGet("list")]
        public IActionResult AppointmentList(int? page, string searchTerm)
        {
            int pageSize = 5;
            int pageNumber = page ?? 1;

            DataTable table = new DataTable();

            using (SqlConnection con = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("PR_App_Appointment_SelectAll", con);
                cmd.CommandType = CommandType.StoredProcedure;

             

                SqlDataReader reader = cmd.ExecuteReader();
                table.Load(reader);
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
            ViewBag.SearchTerm = searchTerm;

            return View("AppointmentList", pagedTable);
        }

        #endregion
        // ✅ Delete Multiple Appointments
        [HttpPost]
        [Route("delete-selected-appointments")]
        public IActionResult DeleteSelectedAppointments(List<int> SelectedAppointmentIds)
        {
            if (SelectedAppointmentIds == null || SelectedAppointmentIds.Count == 0)
            {
                TempData["ErrorMessage"] = "No appointments selected for deletion.";
                return RedirectToAction("AppointmentList");
            }

            try
            {
                using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString"));
                connection.Open();

                foreach (var appointmentId in SelectedAppointmentIds)
                {
                    SqlCommand cmd = connection.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "PR_App_Appointment_Delete";
                    cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                    cmd.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "✅ Selected appointments deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "❌ Error: " + ex.Message;
            }

            return RedirectToAction("AppointmentList");
        }

        // ✅ Delete Single Appointment
        [HttpGet]
        [Route("appointment-delete")]
        public IActionResult AppointmentDelete(int AppointmentID)
        {
            try
            {
                using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString"));
                connection.Open();

                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_App_Appointment_Delete";
                command.Parameters.Add("@AppointmentID", SqlDbType.Int).Value = AppointmentID;
                command.ExecuteNonQuery();

                TempData["SuccessMessage"] = "✅ Appointment deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "❌ Error: " + ex.Message;
            }

            return RedirectToAction("AppointmentList");
        }

        #region Add/Edit Appointment

        [HttpGet("Appointment-form")]
        public IActionResult AppointmentForm(int ID)
        {
            if (ID > 0)
            {
                using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString"));
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "PR_App_Appointment_SelectByPK";
                command.Parameters.AddWithValue("AppointmentID", ID);
                SqlDataReader reader = command.ExecuteReader();

                AppointmentAddEditModel model = new AppointmentAddEditModel();

                while (reader.Read())
                {
                    model.UserID = Convert.ToInt32(reader["UserID"]);
                    model.AppointmentID = Convert.ToInt32(reader["AppointmentID"]);
                    model.DoctorID = Convert.ToInt32(reader["DoctorID"]);
                    model.PatientID = Convert.ToInt32(reader["PatientID"]);
                    model.AppointmentDate = Convert.ToDateTime(reader["AppointmentDate"]);
                    model.AppointmentStatus = reader["AppointmentStatus"].ToString();
                    model.Description = reader["Description"].ToString();
                    model.SpecialRemarks = reader["SpecialRemarks"].ToString();
                    model.Created = Convert.ToDateTime(reader["Created"]);

                    model.TotalConsultedAmount = Convert.ToDecimal(reader["TotalConsultedAmount"]);
                }

                return View("AppointmentAddEdit", model);
            }
            else
            {
                return View("AppointmentAddEdit", new AppointmentAddEditModel());
            }
        }


        [HttpPost("save")]
        public IActionResult AppointmentAddEdit(AppointmentAddEditModel AppointmentAddEditModel)
        {
            if (ModelState.IsValid)
            {
                using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ConnectionString"));
                connection.Open();
                SqlCommand command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;

                if (AppointmentAddEditModel.AppointmentID > 0)
                {
                    command.CommandText = "PR_App_Appointment_UpdateByPK";
                    command.Parameters.AddWithValue("AppointmentID", AppointmentAddEditModel.AppointmentID);
                }
                else
                {
                    command.CommandText = "PR_App_Appointment_Insert";
                }

                command.Parameters.Add("@DoctorID", SqlDbType.Int).Value = AppointmentAddEditModel.DoctorID;
                command.Parameters.Add("@PatientID", SqlDbType.Int).Value = AppointmentAddEditModel.PatientID;
                command.Parameters.Add("@AppointmentDate", SqlDbType.DateTime).Value = AppointmentAddEditModel.AppointmentDate;
                command.Parameters.Add("@AppointmentStatus", SqlDbType.NVarChar).Value = AppointmentAddEditModel.AppointmentStatus;
                command.Parameters.Add("@Description", SqlDbType.NVarChar).Value = AppointmentAddEditModel.Description;
                command.Parameters.Add("@SpecialRemarks", SqlDbType.NVarChar).Value = AppointmentAddEditModel.SpecialRemarks;
                command.Parameters.Add("@Modified", SqlDbType.DateTime).Value = DateTime.Now;
                //command.Parameters.Add("@Cr", SqlDbType.DateTime).Value = DateTime.Now;

                command.Parameters.Add("@UserId", SqlDbType.Int).Value = AppointmentAddEditModel.UserID;
                command.Parameters.Add("@TotalConsultedAmount", SqlDbType.Decimal).Value = AppointmentAddEditModel.TotalConsultedAmount;
                command.ExecuteNonQuery();

                return RedirectToAction("AppointmentList");
            }
            return View("AppointmentAddEdit");
        }
        #endregion

        #region Export to Excel
        [HttpGet("ExportToExcel")]
        public IActionResult ExportToExcel()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    using (SqlCommand cmd = new SqlCommand(@"SELECT 
                                                                A.AppointmentID, 
                                                                A.PatientID, 
                                                                A.DoctorID, 
                                                                A.AppointmentStatus, 
                                                                A.AppointmentDate, 
                                                                A.SpecialRemarks, 
                                                                A.TotalConsultedAmount, 
                                                                A.Modified 
                                                            FROM Appointment A", conn))
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
                }

                using (var workbook = new XLWorkbook())
                {
                    dt.TableName = "Appointments";
                    workbook.Worksheets.Add(dt);

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "AppointmentList.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting appointment data: " + ex.Message;
                return RedirectToAction("AppointmentList");
            }
        }
        #endregion

        #region Export to CSV
        [HttpGet("ExportToCSV")]
        public IActionResult ExportToCSV()
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("ConnectionString")))
                {
                    using (SqlCommand cmd = new SqlCommand(@"SELECT 
                                                                A.AppointmentID, 
                                                                A.PatientID, 
                                                                A.DoctorID, 
                                                                A.AppointmentStatus, 
                                                                A.AppointmentDate, 
                                                                A.SpecialRemarks, 
                                                                A.TotalConsultedAmount, 
                                                                A.Modified 
                                                            FROM Appointment A", conn))
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dt);
                    }
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

                return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "AppointmentList.csv");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error exporting appointment data: " + ex.Message;
                return RedirectToAction("AppointmentList");
            }
        }
        #endregion
    }
}
