
using System.ComponentModel.DataAnnotations;

namespace RideSharingApp.Models
{
    public class FeedbackViewModel
    {
        public string BookingID { get; set; }
        [Required, Range(1, 5)]
        public int Rating { get; set; }
        public string? Comments { get; set; }
    }
}