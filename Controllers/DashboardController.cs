////using HMS.Models;
////using Microsoft.AspNetCore.Mvc;
////using Microsoft.Extensions.Configuration;
////using System;
////using System.Data;
////using System.Data.SqlClient;

////namespace HMS.Controllers
////{
////    public class DashboardController : Controller
////    {
////        private readonly IConfiguration _configuration;

////        public DashboardController(IConfiguration configuration)
////        {
////            _configuration = configuration;
////        }

////        public IActionResult Index() // matches Dashboard.cshtml
////        {
////            DashboardCountModel model = new DashboardCountModel();

////            try
////            {
////                using (SqlConnection conn = new SqlConnection(_configuration.GetConnectionString("HMSConnection")))
////                {
////                    conn.Open();
////                    using (SqlCommand cmd = new SqlCommand("PR_Dashboard_Counts", conn))
////                    {
////                        cmd.CommandType = CommandType.StoredProcedure;

////                        using (SqlDataReader reader = cmd.ExecuteReader())
////                        {
////                            if (reader.Read())
////                            {
////                                model.TotalDepartments= Convert.ToInt32(reader["DepartmentCount"]);
////                                model.TotalUsers = Convert.ToInt32(reader["TotalUsers"]);
////                                model.TotalPatients = Convert.ToInt32(reader["TotalPatients"]);
////                                model.TotalAppointments = Convert.ToInt32(reader["TotalAppointments"]);
////                                model.TotalDoctors = Convert.ToInt32(reader["TotalDoctors"]);
////                                model.TotalDoctorDepartments = Convert.ToInt32(reader["TotalDoctorDepartments"]);

////                            }
////                        }
////                    }
////                }
////            }
////            catch (Exception ex)
////            {
////                ViewBag.Error = "Error loading dashboard: " + ex.Message;
////            }

////            return View("Dashboard", model);
////        }
////    }
////}

using HMS.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace HospitalManagementSystem.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IConfiguration configuration;

        public DashboardController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public IActionResult Index()
        {
            DashboardCountModel model = new DashboardCountModel();

            using (SqlConnection connection = new SqlConnection(configuration.GetConnectionString("ConnectionString")))
            {
                connection.Open();

                // Entity counts
                model.UserCount = ExecuteCount(connection, "SELECT COUNT(*) FROM [User]");
                model.DoctorCount = ExecuteCount(connection, "SELECT COUNT(*) FROM Doctor");
                model.PatientCount = ExecuteCount(connection, "SELECT COUNT(*) FROM Patient");
                model.DepartmentCount = ExecuteCount(connection, "SELECT COUNT(*) FROM Department");
                model.AppointmentCount = ExecuteCount(connection, "SELECT COUNT(*) FROM Appointment");
                model.EmployeeCount = ExecuteCount(connection, "SELECT COUNT(*) FROM DoctorDepartment");

                // Revenue (using Appointment table + TotalConsultedAmount column)
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT DATENAME(MONTH, Created) AS [Month], " +
                    "SUM(TotalConsultedAmount) AS Total " +
                    "FROM Appointment " +
                    "GROUP BY DATENAME(MONTH, Created)", connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.RevenueLabels.Add(reader["Month"].ToString());
                        model.RevenueData.Add(Convert.ToDecimal(reader["Total"]));
                    }
                }

                // Appointments per month (using Created instead of Date)
                using (SqlCommand cmd = new SqlCommand(
                    "SELECT DATENAME(MONTH, Created) AS [Month], " +
                    "COUNT(*) AS Total " +
                    "FROM Appointment " +
                    "GROUP BY DATENAME(MONTH, Created)", connection))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        model.AppointmentLabels.Add(reader["Month"].ToString());
                        model.AppointmentData.Add(Convert.ToInt32(reader["Total"]));
                    }
                }

            }

            return View("Dashboard",model);
        }

        private int ExecuteCount(SqlConnection conn, string query)
        {
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}