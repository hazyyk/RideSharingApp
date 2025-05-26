using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace RideSharingApp.Models
{
    public class PaymentViewModel
    {
        public string BookingID { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } // Payment date
        public string PaymentID { get; set; } //
        public string PaymentMethod { get; set; }
        public bool ConfirmPayment { get; set; }




    }
}