
using System.ComponentModel.DataAnnotations;
using System;

namespace RideSharingApp.Models
{
    public class Location
    {
        [Key]
        public string LocationID { get; set; } 
        public string Address { get; set; } 
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
        public float GpsAccuracy { get; set; }

        
    }
}