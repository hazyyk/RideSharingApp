using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideSharingApp.Data;
using RideSharingApp.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace RideSharingApp.Controllers
{
    [Authorize]
    public class RideBookingController : Controller
    {
        private readonly RideSharingDbContext _context;

        public RideBookingController(RideSharingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Book()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Book(RideBookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var pickupLocation = new Location
                {
                    LocationID = Guid.NewGuid().ToString(),
                    Address = model.PickupAddress,
                    Latitude = model.PickupLatitude,
                    Longitude = model.PickupLongitude,
                    Timestamp = DateTime.Now,
                    GpsAccuracy = 1.0f
                };

                var dropoffLocation = new Location
                {
                    LocationID = Guid.NewGuid().ToString(),
                    Address = model.DropoffAddress,
                    Latitude = model.DropoffLatitude,
                    Longitude = model.DropoffLongitude,
                    Timestamp = DateTime.Now,
                    GpsAccuracy = 1.0f
                };

                var booking = new RideBooking
                {
                    BookingID = Guid.NewGuid().ToString(),
                    Status = "Pending",
                    RequestTime = DateTime.Now,
                    PickupLocationID = pickupLocation.LocationID,
                    DropoffLocationID = dropoffLocation.LocationID,
                    CustomerID = _context.Customers.FirstOrDefault(c => c.Email == User.Identity.Name)?.CustomerID
                };

                _context.Locations.AddRange(pickupLocation, dropoffLocation);
                _context.RideBookings.Add(booking);
                await _context.SaveChangesAsync();

                // Redirect to payment after booking (for testing; in production, wait for driver acceptance)
                return RedirectToAction("Process", "PaymentFeedback", new { bookingId = booking.BookingID });
            }
            return View(model);
        }

    }
}