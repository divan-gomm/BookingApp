using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingApp.Controllers.Dashboard
{
    public class CustomerController : Controller
    {
        [Authorize(Roles = "Customer")]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
