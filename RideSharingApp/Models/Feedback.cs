using System.ComponentModel.DataAnnotations;
using System;

namespace RideSharingApp.Models
{
    public class Feedback
    {
        [Key]
        public string FeedbackID { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comments { get; set; }
        public DateTime Date { get; set; }
        public string CustomerID { get; set; } = string.Empty;
        public virtual Customer Customer { get; set; } = null!;
    }
}