using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class DoctorDepartmentController : Controller
    {
        public IActionResult Index()
        {
            return View("DoctorDepartmentAddEdit");
        }
        public IActionResult DoctorDepartmentList()
        {
            return View();
        }
    }
}
