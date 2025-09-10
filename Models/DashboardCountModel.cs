namespace HMS.Models
{
    public class DashboardCountModel
    {
        public int UserCount { get; set; }
        public int DoctorCount { get; set; }
        public int PatientCount { get; set; }
        public int DepartmentCount { get; set; }
        public int AppointmentCount { get; set; }
        public int EmployeeCount { get; set; }

        // For charts
        public List<string> RevenueLabels { get; set; } = new();
        public List<decimal> RevenueData { get; set; } = new();
        public List<string> AppointmentLabels { get; set; } = new();
        public List<int> AppointmentData { get; set; } = new();
    }
}
