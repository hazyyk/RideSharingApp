using System.ComponentModel.DataAnnotations;

namespace RideSharingApp.Models
{
    public class RideBookingViewModel
    {
        [Required]
        public string PickupAddress { get; set; } = string.Empty;
        public double PickupLatitude { get; set; }
        public double PickupLongitude { get; set; }
        [Required]
        public string DropoffAddress { get; set; } = string.Empty;
        public double DropoffLatitude { get; set; }
        public double DropoffLongitude { get; set; }

         
    }
}