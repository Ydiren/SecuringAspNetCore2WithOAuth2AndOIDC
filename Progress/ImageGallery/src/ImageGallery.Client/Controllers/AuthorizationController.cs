using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ImageGallery.Client.Controllers
{
    public class AuthorizationController : Controller
    {
        // GET: /<controller>/
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}