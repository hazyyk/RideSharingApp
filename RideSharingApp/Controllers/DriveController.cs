using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpPost]
        public async Task<IActionResult> AcceptRide(string bookingId)
        {
            var booking = await _context.RideBookings.FirstOrDefaultAsync(b => b.BookingID == bookingId);
            if (booking != null)
            {
                var email = User.Identity?.Name;
                var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.Email == email);

                if (driver != null)
                {
                    booking.DriverID = driver.DriverID;
                    booking.Status = "Accepted";
                    await _context.SaveChangesAsync();

                    return RedirectToAction("TrackRide", new { bookingId = bookingId });
                }

                ModelState.AddModelError("", "Driver not found. Please ensure your driver account has the correct email.");
            }

            return RedirectToAction("Dashboard");
        }



        [HttpGet]
        public IActionResult TrackRide(string bookingId)
        {
            var booking = _context.RideBookings
                .Include(b => b.PickupLocation)
                .Include(b => b.DropoffLocation)
                .Include(b => b.Customer)
                .FirstOrDefault(b => b.BookingID == bookingId && b.DriverID != null);

            if (booking == null)
                return NotFound();

            return View(booking);
        }


    }
}