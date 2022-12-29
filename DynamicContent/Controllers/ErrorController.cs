using Microsoft.AspNetCore.Mvc;

namespace DynamicContent.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error")]
        public IActionResult Error()
        {
            return View("NotFound");
        }
    }
}
