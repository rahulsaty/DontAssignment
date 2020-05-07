using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NerdDinner.Web.Models;
using NerdDinner.Web.Persistence;

namespace NerdDinner.Web.Controllers
{

    [Authorize]
        public class DinnersController : Controller
        {
            private readonly INerdDinnerRepository _repository;

            private readonly UserManager<ApplicationUser> _userManager;

            public DinnersController(INerdDinnerRepository repository, UserManager<ApplicationUser> userManager)
            {
                _repository = repository;
                _userManager = userManager;
            }

            [HttpGet]
            [AllowAnonymous]
            public async Task<IActionResult> Detail(int id)
            {
                Dinner dinner = await _repository.GetDinnerAsync(id);
                if (dinner == null)
                {
                    return NotFound();
                
                }

                try
                {
                    var user = await _userManager.FindByNameAsync(HttpContext.User.Identity.Name);
               
                        ViewBag.isHost = dinner.IsUserHost(user.UserName);
           
                }
                catch (ArgumentNullException e)
                {
                    ViewBag.isHost = false;
                }
            
                return  View(dinner);
            }


            public IActionResult AddDinner()
            {
                return View();
            }

            [HttpGet]
            [AllowAnonymous]
            public async Task<IEnumerable<Dinner>> GetDinnersAsync(
                DateTime? startDate,
                DateTime? endDate,
                double? lat,
                double? lng,
                int? pageIndex,
                int? pageSize,
                string searchQuery = null,
                string sort = null,
                bool descending = false)
            {
                return await _repository.GetDinnersAsync(startDate, endDate, string.Empty, searchQuery, sort, descending, lat, lng, pageIndex, pageSize);
            }



            [HttpGet]
            [AllowAnonymous]
            public async Task<ActionResult> MyDinner(
              DateTime? startDate,
              DateTime? endDate,
              double? lat,
              double? lng,
              int? pageIndex,
              int? pageSize,
              string searchQuery = null,
              string sort = null,
              bool descending = false)
            {
                var user = HttpContext.User.Identity.Name;
                 List<Dinner> dinners =await _repository.GetDinnersAsync(startDate, endDate, user, searchQuery, sort, descending, lat, lng, pageIndex, pageSize);
                return View(dinners);
            }

            [Authorize]
            [HttpPost]
            public async Task<IActionResult> AddDinnerPost(Dinner dinner)
            {
                if (ModelState.IsValid)
                {
                    dinner.UserName = User.Identity.Name;
                    await _repository.CreateDinnerAsync(dinner);
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Error", "Home");
            }

      
            public async Task<IActionResult> EditDinner(int id)
            {
           
                Dinner dinner = await _repository.GetDinnerAsync(id);
                return View(dinner);
          
            }
            [Authorize]
            [HttpPost]
            public async Task<IActionResult> UpdateDinner(int id, Dinner dinner)
            {
                if (ModelState.IsValid)
                {
                    Dinner dinner1 = await _repository.GetDinnerAsync(id);
                    dinner1.Address = dinner.Address;
                    dinner1.EventDate = dinner.EventDate;
                    dinner1.Title = dinner.Title;
                    dinner1.Description = dinner.Description;
                    dinner1.ContactPhone = dinner.ContactPhone;
                    dinner1.Latitude = dinner.Latitude;
                    dinner1.Longitude = dinner.Longitude;
                    await _repository.UpdateDinnerAsync(dinner1);
                    return   RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Error", "Home");
            }

            public async Task<IActionResult> DeleteDinner(int id)
            {
                Dinner dinner = await _repository.GetDinnerAsync(id);
                return View(dinner);

            }

            [HttpPost]
            [Authorize]
            public async Task<IActionResult> DeleteDinnerByID(int id)
            {
                var dinner = await _repository.GetDinnerAsync(id);
                var user = HttpContext.User.Identity.Name;

                if (!dinner.IsUserHost(user))
                {
                    return RedirectToAction("Error","Home");
                }

                await _repository.DeleteDinnerAsync(id);
                return RedirectToAction("Index","Home");
            }
        }
    }
