using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideSharingApp.Data;
using RideSharingApp.Models;
using RideSharingApp.Utilities;
using System;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
            return View(new RideBookingViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Book(RideBookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Create pickup location
                var pickupLocation = new Location
                {
                    LocationID = Guid.NewGuid().ToString(),
                    Address = model.PickupAddress,
                    Latitude = model.PickupLatitude,
                    Longitude = model.PickupLongitude,
                    Timestamp = DateTime.Now,
                    GpsAccuracy = 1.0f
                };

                // Create dropoff location
                var dropoffLocation = new Location
                {
                    LocationID = Guid.NewGuid().ToString(),
                    Address = model.DropoffAddress,
                    Latitude = model.DropoffLatitude,
                    Longitude = model.DropoffLongitude,
                    Timestamp = DateTime.Now,
                    GpsAccuracy = 1.0f
                };

                // Calculate distance between pickup and dropoff
                double distance = DistanceCalculator.CalculateDistance(
                    pickupLocation.Latitude, pickupLocation.Longitude,
                    dropoffLocation.Latitude, dropoffLocation.Longitude);

                // Calculate payment amount (10,000 VND per km)
                decimal ratePerKm = 11000; // Adjust as needed
                decimal amount = (decimal)distance * ratePerKm;

                // Create a Payment record
                var payment = new Payment
                {
                    PaymentID = Guid.NewGuid().ToString(),
                    Amount = amount,
                    PaymentDate = DateTime.Now
                };

                // Get the CustomerID from the logged-in user
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == User.Identity.Name);
                if (customer == null)
                {
                    ModelState.AddModelError("", "Customer not found.");
                    return View(model);
                }

                // Create the RideBooking record
                var booking = new RideBooking
                {
                    BookingID = Guid.NewGuid().ToString(),
                    Status = "Pending",
                    RequestTime = DateTime.Now,
                    PickupLocationID = pickupLocation.LocationID,
                    DropoffLocationID = dropoffLocation.LocationID,
                    CustomerID = customer.CustomerID,
                    PaymentID = payment.PaymentID
                };

                // Save to database
                _context.Locations.AddRange(pickupLocation, dropoffLocation);
                _context.Payments.Add(payment);
                _context.RideBookings.Add(booking);
                await _context.SaveChangesAsync();

                // Redirect to payment processing
                return RedirectToAction("Process", "PaymentFeedback", new { bookingId = booking.BookingID });
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult Preview(RideBookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                double distance = DistanceCalculator.CalculateDistance(
                    model.PickupLatitude, model.PickupLongitude,
                    model.DropoffLatitude, model.DropoffLongitude);

                decimal ratePerKm = 10000; // Same rate as above
                decimal amount = (decimal)distance * ratePerKm;

                ViewBag.Distance = distance;
                ViewBag.Amount = amount;
                return View("Book", model); // Return to the booking form with preview
            }
            return View("Book", model);
        }

        [Authorize]
        [HttpGet]
        public IActionResult Track(string bookingId)
        {
            var booking = _context.RideBookings
                .Include(b => b.Driver)
                .Include(b => b.PickupLocation)
                .Include(b => b.DropoffLocation)
                .FirstOrDefault(b => b.BookingID == bookingId);

            if (booking == null)
                return NotFound();

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FinishRide([FromBody] FinishRideRequest request)
        {
            var booking = await _context.RideBookings.FirstOrDefaultAsync(b => b.BookingID == request.BookingId);
            if (booking == null) return NotFound();

            booking.Status = "Completed";
            await _context.SaveChangesAsync();

            return Ok();
        }

        public class FinishRideRequest
        {
            public string BookingId { get; set; }
        }




    }
}