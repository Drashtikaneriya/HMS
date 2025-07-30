using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class UserAddEditModel
    {
        public int? UserID { get; set; }

        [Required(ErrorMessage = "Please enter a username")]
        [Display(Name = "User Name")]
        [StringLength(50, ErrorMessage = "User name cannot exceed 50 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6 to 20 characters")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter an email address")]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a mobile number")]
        [Display(Name = "Mobile Number")]
        [Phone(ErrorMessage = "Invalid Mobile Number")]
        [StringLength(10, ErrorMessage = "Mobile number cannot exceed 10 digits")]
        public string MobileNo { get; set; }

        [Display(Name = "Active User")]
        public bool IsActive { get; set; }

        [Display(Name = "Created Date")]
        public DateTime Created { get; set; } = DateTime.Now;

        [Display(Name = "Modified Date")]
        public DateTime Modified { get; set; } = DateTime.Now;
    }
}
