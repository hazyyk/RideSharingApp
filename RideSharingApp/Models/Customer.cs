
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace RideSharingApp.Models
{
    public class Customer
    {
        [Key]
        public string CustomerID { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public virtual ICollection<RideBooking> RideBookings { get; set; } = new List<RideBooking>();
        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    }
}