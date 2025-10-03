using APPR_Foudation.Models;
using Microsoft.AspNetCore.Mvc;

namespace APPR_Foudation.Controllers
{
    public class ContactController : Controller
    {
        private readonly string email = "info@giftofthegivers.org";
        private readonly string address = "South Africa";

        public IActionResult Index()
        {
            ViewData["EmailAddress"] = email;
            ViewBag.Address = address;

            return View();
        }

        [HttpPost]
        public IActionResult Index(ContactDto model)
        {
            ViewData["EmailAddress"] = email;
            ViewBag.Address = address;

            if (!ModelState.IsValid)
            {
                return View(model);
}
            //Store the data in the database
            ViewBag.SuccessMessage = "Your message is received successfully!";
            ModelState.Clear();
            return View();
        }
    }
}
