using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers.Dashboard
{
    public class StaffController : Controller
    {
        [Authorize(Roles = "BusinessOwner,")]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
