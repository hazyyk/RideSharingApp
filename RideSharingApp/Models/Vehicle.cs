
using System.ComponentModel.DataAnnotations;

namespace RideSharingApp.Models
{
    public class Vehicle
    {
        [Key]
        public string VehicleID { get; set; } = Guid.NewGuid().ToString();
        public string LicensePlate { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Capacity { get; set; }
    }
}