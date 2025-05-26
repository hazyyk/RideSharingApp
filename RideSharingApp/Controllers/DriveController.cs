using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideSharingApp.Data;
using RideSharingApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace RideSharingApp.Controllers
{
    [Authorize]
    public class DriverController : Controller
    {
        private readonly RideSharingDbContext _context;

        public DriverController(RideSharingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            var bookings = _context.RideBookings
                .Where(b => b.Status == "Pending")
                .Select(b => new DriverDashboardViewModel
                {
                    BookingID = b.BookingID,
                    Pickup = _context.Locations
                        .Where(l => l.LocationID == b.PickupLocationID)
                        .Select(l => l.Address)
                        .FirstOrDefault() ?? "Unknown",
                    Dropoff = _context.Locations
                        .Where(l => l.LocationID == b.DropoffLocationID)
                        .Select(l => l.Address)
                        .FirstOrDefault() ?? "Unknown"
                })
                .ToList();

            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptRide(string bookingId)
        {
            var booking = _context.RideBookings.FirstOrDefault(b => b.BookingID == bookingId);
            if (booking != null)
            {
                var driver = _context.Drivers.FirstOrDefault(d => d.PhoneNumber == User.Identity.Name);
                if (driver != null)
                {
                    booking.DriverID = driver.DriverID;
                    booking.Status = "Accepted";
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Dashboard");
        }
    }
}