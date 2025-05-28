using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RideSharingApp.Data;
using RideSharingApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public async Task<IActionResult> Process(string bookingId)
        {
            if (string.IsNullOrEmpty(bookingId))
            {
                return BadRequest("Booking ID is required.");
            }

            var booking = await _context.RideBookings
                .Include(b => b.Payment)
                .FirstOrDefaultAsync(b => b.BookingID == bookingId);

            if (booking == null || booking.Payment == null)
            {
                return NotFound("Booking or payment not found.");
            }

            var model = new PaymentViewModel
            {
                BookingID = bookingId,
                Amount = booking.Payment.Amount,
                PaymentDate = booking.Payment.PaymentDate,
                PaymentID = booking.Payment.PaymentID
            };

            ViewBag.PaymentMethods = new SelectList(new[]
            {
                    "Bank Transfer", "Credit Card", "MoMo", "ZaloPay"
                });

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Process(PaymentViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!model.ConfirmPayment)
                {
                    ModelState.AddModelError("ConfirmPayment", "You must confirm the payment.");
                    ViewBag.PaymentMethods = new SelectList(new[] { "Bank Transfer", "Credit Card", "MoMo", "ZaloPay" });
                    return View(model);
                }

                var booking = await _context.RideBookings
                    .Include(b => b.Payment)
                    .FirstOrDefaultAsync(b => b.BookingID == model.BookingID);

                if (booking == null)
                {
                    ModelState.AddModelError(string.Empty, "Booking not found.");
                    ViewBag.PaymentMethods = new SelectList(new[] { "Bank Transfer", "Credit Card", "MoMo", "ZaloPay" });
                    return View(model);
                }

                // Update the Payment record with the confirmed amount (if editable, use model.Amount; otherwise, keep original)
                var payment = booking.Payment ?? new Payment
                {
                    PaymentID = Guid.NewGuid().ToString(),
                    PaymentDate = DateTime.Now
                };
                payment.Amount = model.Amount > 0 ? model.Amount : booking.Payment.Amount; // Use submitted amount if valid, else original
                if (booking.Payment == null)
                {
                    _context.Payments.Add(payment);
                    booking.PaymentID = payment.PaymentID;
                }

                booking.Status = "Completed";
                await _context.SaveChangesAsync();

                return RedirectToAction("Feedback", new { bookingId = model.BookingID });
            }

            ViewBag.PaymentMethods = new SelectList(new[]
                {
                    "Bank Transfer", "Credit Card", "MoMo", "ZaloPay"
                });

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
                return RedirectToAction("PaymentSuccess", new { bookingId = model.BookingID }); // Redirect to PaymentSuccess
            }
            return View(model);
        }
        

        [HttpGet]
        public IActionResult PaymentSuccess(string bookingId)
        {
            ViewBag.BookingID = bookingId; // Pass bookingId to the view if needed
            return View();
        }
    }
}