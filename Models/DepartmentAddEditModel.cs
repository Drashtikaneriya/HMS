using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class DepartmentAddEditModel
    {

        public int? DepartmentID { get; set; }


        public required string DepartmentName { get; set; }

        public required string Description { get; set; }

        public bool IsActive { get; set; } = true;

        [Required]
        public DateTime Created { get; set; } = DateTime.Now;

        [Required]
        public DateTime Modified { get; set; } = DateTime.Now;

        [Required]
        public int UserID { get; set; }
       }   
    }

