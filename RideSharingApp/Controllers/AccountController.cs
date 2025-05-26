using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RideSharingApp.Data;
using RideSharingApp.Models;
using System;
using System.Threading.Tasks;

namespace RideSharingApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RideSharingDbContext _context;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RideSharingDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (model.IsDriver)
                    {
                        await _userManager.AddToRoleAsync(user, "Driver");

                        var vehicle = new Vehicle
                        {
                            VehicleID = Guid.NewGuid().ToString(),
                            Model = model.VehicleModel ?? "Unknown",
                            Type = model.VehicleType ?? "Unknown",
                            LicensePlate = model.LicensePlate ?? "Unknown",
                            Capacity = model.Capacity ?? 4
                        };
                        _context.Vehicles.Add(vehicle);
                        await _context.SaveChangesAsync();

                        var driver = new Driver
                        {
                            DriverID = Guid.NewGuid().ToString(),
                            Name = model.Name,
                            Email = model.Email,               // Set email here
                            PhoneNumber = model.PhoneNumber,
                            LicenseNumber = model.LicenseNumber ?? "Pending",
                            VehicleID = vehicle.VehicleID,
                            IdentityUserId = user.Id           // Link driver to Identity user
                        };
                        _context.Drivers.Add(driver);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, "Customer");

                        var customer = new Customer
                        {
                            CustomerID = Guid.NewGuid().ToString(),
                            Name = model.Name,
                            Email = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            IdentityUserId = user.Id          // Link customer to Identity user
                        };
                        _context.Customers.Add(customer);
                        await _context.SaveChangesAsync();
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}