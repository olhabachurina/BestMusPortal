using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BestMusPortal.Data;
using BestMusPortal.Models;

namespace BestMusPortal.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<RegistrationController> _logger;

        public RegistrationController(ApplicationDbContext context, ILogger<RegistrationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Registration/RegisterAdmin
        public IActionResult RegisterAdmin()
        {
            if (_context.Users.Any(u => u.Role == "Admin"))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult RegisterAdmin(User user)
        {
            if (ModelState.IsValid)
            {
                user.Role = "Admin";
                user.IsApproved = true;
                user.IsActive = true;
                _context.Users.Add(user);
                _context.SaveChanges();
                _logger.LogInformation("Admin user added.");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        _logger.LogWarning("Validation error in field {Field}: {Error}", state.Key, error.ErrorMessage);
                    }
                }
            }
            return View(user);
        }

        // GET: /Registration/Register
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                user.Role = "User";
                user.IsApproved = false;
                user.IsActive = false;
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }
    }
}