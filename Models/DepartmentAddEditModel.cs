namespace HMS.Models
{
    public class DepartmentAddEditModel
    {
        public string DepartmentName {  get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime Modified {  get; set; }
        public int UserID { get; set; }
    }
}
