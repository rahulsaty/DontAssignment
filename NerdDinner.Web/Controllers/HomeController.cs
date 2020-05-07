using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NerdDinner.Web.Models;
using NerdDinner.Web.Persistence;

namespace NerdDinner.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly INerdDinnerRepository _repository;

        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(INerdDinnerRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }
        public async Task<ActionResult> Index()
        {
            List<Dinner> data = await _repository.GetPopularDinnersAsync();
            ViewBag.Item = data[0];
            return  View(data);
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}
