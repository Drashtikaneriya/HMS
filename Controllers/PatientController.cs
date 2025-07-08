using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class PatientController : Controller
    {
        public IActionResult Index()
        {
            return View("PatientAddEdit");
        }
        public IActionResult PatientList()
        {
            return View();
        }
    }
}
