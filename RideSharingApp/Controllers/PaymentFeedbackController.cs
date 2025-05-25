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
    public class PaymentFeedbackController : Controller
    {
        private readonly RideSharingDbContext _context;

        public PaymentFeedbackController(RideSharingDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Process(string bookingId)
        {
            var model = new PaymentViewModel { BookingID = bookingId };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Process(PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var payment = new Payment
                {
                    PaymentID = Guid.NewGuid().ToString(),
                    Amount = model.Amount,
                    PaymentDate = DateTime.Now
                };

                var booking = _context.RideBookings.FirstOrDefault(b => b.BookingID == model.BookingID);
                if (booking != null)
                {
                    booking.PaymentID = payment.PaymentID;
                    booking.Status = "Completed";
                    _context.Payments.Add(payment);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Feedback", new { bookingId = model.BookingID });
                }
                ModelState.AddModelError(string.Empty, "Booking not found.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Feedback(string bookingId)
        {
            var model = new FeedbackViewModel { BookingID = bookingId };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Feedback(FeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                var feedback = new Feedback
                {
                    FeedbackID = Guid.NewGuid().ToString(),
                    Rating = model.Rating,
                    Comments = model.Comments,
                    Date = DateTime.Now,
                    CustomerID = _context.Customers.FirstOrDefault(c => c.Email == User.Identity.Name)?.CustomerID
                };
                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
    }
}