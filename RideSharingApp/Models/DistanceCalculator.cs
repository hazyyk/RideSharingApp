using System;
using System.ComponentModel.DataAnnotations;

namespace RideSharingApp.Utilities
{
    public static class DistanceCalculator
    {
        private const double EarthRadiusKm = 6371; // Earth's radius in kilometers

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            // Convert latitude and longitude to radians
            lat1 = ToRadians(lat1);
            lon1 = ToRadians(lon1);
            lat2 = ToRadians(lat2);
            lon2 = ToRadians(lon2);

            // Differences in coordinates
            double dLat = lat2 - lat1;
            double dLon = lon2 - lon1;

            // Haversine formula
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1) * Math.Cos(lat2) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = EarthRadiusKm * c;

            return distance; // Distance in kilometers
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}