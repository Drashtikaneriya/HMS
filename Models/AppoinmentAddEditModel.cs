using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class AppointmentAddEditModel
    {
        public int DoctorID { get; set; }
        public int patientID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentStatus { get; set; }

        public string Description { get; set; }
        public string SpecialRemarks { get; set; }

        [DataType(DataType.Date)] // Optional but helpful for date handling
        public DateTime Modified { get; set; }
        public DateTime Created { get; set; }
        public int UserID { get; set; }
        public Decimal TotalConsultedAmount { get; set; }
    }
}
