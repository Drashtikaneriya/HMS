using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;

namespace HMS.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IConfiguration _configuration;

        public DashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            DashboardCountModel model = new DashboardCountModel();

            try
            {
                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("HMSConnection")))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("PR_Dashboard_Counts", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Null-safe reading
                                model.TotalDepartments = reader["DepartmentCount"] != DBNull.Value ? Convert.ToInt32(reader["DepartmentCount"]) : 0;
                                model.TotalUsers = reader["TotalUsers"] != DBNull.Value ? Convert.ToInt32(reader["TotalUsers"]) : 0;
                                model.TotalPatients = reader["TotalPatients"] != DBNull.Value ? Convert.ToInt32(reader["TotalPatients"]) : 0;
                                model.TotalAppointments = reader["TotalAppointments"] != DBNull.Value ? Convert.ToInt32(reader["TotalAppointments"]) : 0;
                                model.TotalDoctors = reader["TotalDoctors"] != DBNull.Value ? Convert.ToInt32(reader["TotalDoctors"]) : 0;
                                model.TotalDoctorDepartments = reader["TotalDoctorDepartments"] != DBNull.Value ? Convert.ToInt32(reader["TotalDoctorDepartments"]) : 0;
                                model.TotalRevenue = reader["TotalRevenue"] != DBNull.Value ? Convert.ToDecimal(reader["TotalRevenue"]) : 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Display friendly error in view
                ViewBag.Error = "Error loading dashboard: " + ex.Message;

                // Optional: log exception to file/db here
            }

            return View("Dashboard", model);
        }
    }
}
