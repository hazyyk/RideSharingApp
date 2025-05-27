using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RideSharingApp.Data;
using RideSharingApp.Hubs;
using RideSharingApp.Models;
using System.ComponentModel.DataAnnotations;

[Authorize(Roles = "Driver")]
public class DriverController : Controller
{
    private readonly RideSharingDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IHubContext<RideTrackingHub> _hubContext;

    public DriverController(RideSharingDbContext context, UserManager<IdentityUser> userManager, IHubContext<RideTrackingHub> hubContext)
    {
        _context = context;
        _userManager = userManager;
        _hubContext = hubContext;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var bookings = await _context.RideBookings
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
            .ToListAsync();

        return View(bookings);
    }

    [HttpPost]
    public async Task<IActionResult> AcceptRide(string bookingId)
    {
        if (string.IsNullOrWhiteSpace(bookingId))
            return BadRequest("Booking ID is required.");

        var booking = await _context.RideBookings.FirstOrDefaultAsync(b => b.BookingID == bookingId);
        if (booking == null)
            return NotFound("Booking not found.");

        var user = await _userManager.GetUserAsync(User);
        var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.IdentityUserId == user.Id);

        if (driver == null)
        {
            ModelState.AddModelError("", "Driver not found. Please ensure your driver account is set up correctly.");
            return RedirectToAction("Dashboard");
        }

        booking.DriverID = driver.DriverID;
        booking.Status = "Accepted";
        await _context.SaveChangesAsync();

        await _hubContext.Clients.Group(booking.BookingID).SendAsync("DriverAccepted");

        return RedirectToAction("TrackRide", new { bookingId });
    }

    [HttpGet]
    public async Task<IActionResult> TrackRide(string bookingId)
    {
        if (string.IsNullOrWhiteSpace(bookingId))
            return NotFound();

        var booking = await _context.RideBookings
            .Include(b => b.PickupLocation)
            .Include(b => b.DropoffLocation)
            .Include(b => b.Customer)
            .FirstOrDefaultAsync(b => b.BookingID == bookingId && b.DriverID != null);

        if (booking == null)
            return NotFound();

        return View(booking);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateLocation([FromBody] DriverLocationUpdate model)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid data.");

        var user = await _userManager.GetUserAsync(User);
        var driver = await _context.Drivers.FirstOrDefaultAsync(d => d.IdentityUserId == user.Id);

        if (driver == null) return NotFound("Driver not found.");

        driver.Latitude = model.Latitude;
        driver.Longitude = model.Longitude;
        await _context.SaveChangesAsync();

        var booking = await _context.RideBookings
            .Where(b => b.DriverID == driver.DriverID && b.Status == "Accepted")
            .OrderByDescending(b => b.RequestTime)
            .FirstOrDefaultAsync();

        if (booking != null)
        {
            await _hubContext.Clients.Group(booking.BookingID)
                .SendAsync("ReceiveDriverLocation", model.Latitude, model.Longitude);
        }

        return Ok();
    }

    public class DriverLocationUpdate
    {
        [Required]
        public double Latitude { get; set; }

        [Required]
        public double Longitude { get; set; }
    }
}
