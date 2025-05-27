using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace RideSharingApp.Hubs
{
    public class RideTrackingHub : Hub
    {
        // Called when driver sends location update
        public async Task UpdateDriverLocation(string bookingId, double latitude, double longitude)
        {
            // Send the location update to the customer group for this booking
            await Clients.Group(bookingId).SendAsync("ReceiveDriverLocation", latitude, longitude);
        }

        public async Task SendDriverLocation(string bookingId, double lat, double lng)
        {
            await Clients.Group(bookingId).SendAsync("ReceiveDriverLocation", lat, lng);
        }

        public async Task NotifyDriverAccepted(string bookingId)
        {
            // Notify all clients in this booking group (including the customer)
            await Clients.Group(bookingId).SendAsync("DriverAccepted");
        }

        // Customer or driver join group to receive/send updates
        public async Task JoinBookingGroup(string bookingId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, bookingId);
        }

        public async Task LeaveBookingGroup(string bookingId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, bookingId);
        }
    }
}