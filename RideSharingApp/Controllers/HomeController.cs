using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RideSharingApp.Data;
using RideSharingApp.Models;
using System.Diagnostics;

namespace RideSharingApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RideSharingDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(ILogger<HomeController> logger, RideSharingDbContext context, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            List<RideBooking> acceptedRides = new();

            if (User.Identity != null && User.Identity.IsAuthenticated && User.IsInRole("Customer"))
            {
                var user = await _userManager.GetUserAsync(User);
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.IdentityUserId == user.Id); 

                if (customer != null)
                {
                    acceptedRides = await _context.RideBookings
                        .Include(rb => rb.Driver)
                        .Where(rb => rb.CustomerID == customer.CustomerID && rb.Status == "Accepted")
                        .ToListAsync();
                }
            }

            return View(acceptedRides);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
