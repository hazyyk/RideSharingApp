using System.ComponentModel.DataAnnotations;

namespace RideSharingApp.Models
{
    public class PaymentViewModel
    {
        public string BookingID { get; set; }
        [Required]
        public decimal Amount { get; set; }
    }
}