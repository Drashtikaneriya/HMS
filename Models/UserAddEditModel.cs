using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class UserAddEditModel
    {

        public int? UserID { get; set; }

        [Required(ErrorMessage = "User name is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [Phone(ErrorMessage = "Invalid Mobile Number")]
        public string MobileNo { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime Created { get; set; }

  
        public DateTime Modified { get; set; }
    }
}
