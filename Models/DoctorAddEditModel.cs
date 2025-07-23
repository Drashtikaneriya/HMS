using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class DoctorAddEditModel
    {
        public int? DoctorID { get; set; }

        public  string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Qualification { get; set; }
        public string Specialization { get; set; }
        public bool IsActive { get; set; }

        [DataType(DataType.Date)] // Optional but helpful for date handling
        public DateTime Modified { get; set; }

        public int UserID { get; set; }

        }
    }
