namespace RideSharingApp.Models
{
    public class DriverDashboardViewModel
    {
        public string BookingID { get; set; }
        public string Pickup { get; set; }
        public string Dropoff { get; set; }

        public double? DriverLatitude { get; set; }
        public double? DriverLongitude { get; set; }
        public string? DriverAddress { get; set; }
    }
}
