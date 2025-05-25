// Models/RideBooking.cs
using System.ComponentModel.DataAnnotations;
using System;

namespace RideSharingApp.Models
{
    public class RideBooking
    {
        [Key]
        public string BookingID { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime RequestTime { get; set; }
        public string PickupLocationID { get; set; } = string.Empty;
        public string DropoffLocationID { get; set; } = string.Empty;
        public string CustomerID { get; set; } = string.Empty;
        public string? DriverID { get; set; }
        public string? PaymentID { get; set; }
        public virtual Location PickupLocation { get; set; }
        public virtual Location DropoffLocation { get; set; }
        public virtual Customer Customer { get; set; } = null!;
        public virtual Driver? Driver { get; set; }
        public virtual Payment? Payment { get; set; }
    }
}