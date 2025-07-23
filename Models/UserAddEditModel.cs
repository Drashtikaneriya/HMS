using System.ComponentModel.DataAnnotations;

namespace HMS.Models
{
    public class UserAddEditModel
    {
        public int? UserID {  get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string MobileNo { get; set; }

        public bool IsActive {  get; set; }

        public DateTime Modified { get; set; }


    }
}
