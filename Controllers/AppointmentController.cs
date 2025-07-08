using Microsoft.AspNetCore.Mvc;

namespace HMS.Controllers
{
    public class AppointmentController : Controller
    {
        public IActionResult Index()
        {
            return View("AppointmentAddEdit");
        }
        public IActionResult AppoinmentList()
        {
            return View();
        }
    }
}
