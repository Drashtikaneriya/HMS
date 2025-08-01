﻿namespace HMS.Models

{
    public class DoctorDepartmentModelAddEdit
    {
        public int? DoctorDepartmentID { get; set; }
        public int DoctorID  { get; set; }
        public int DepartmentID { get; set; }

        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; } = DateTime.Now;

        public int UserID { get; set; }
    }
}
