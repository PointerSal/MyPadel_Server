namespace AuthService.Model
{
    public class Booking
    {
        public int Id { get; set; }                     
        public string SportType { get; set; }        
        public DateTime Date { get; set; }            
        public DateTime EndTime { get; set; }         
        public string? TimeSlot { get; set; }         
        public int FieldId { get; set; }             
        public string? PaymentMethod { get; set; }    
        public decimal Amount { get; set; }          
        public bool FlagBooked { get; set; }         
        public bool FlagCanceled { get; set; }       
        public bool FlagArchived { get; set; }
        public string? Email { get; set; }            
        public string? PaymentStatus { get; set; }
        public string? PaymentId { get; set; }
    }

    public class BookingRequest
    {
        public string SportType { get; set; }  // Type of sport
        public DateTime Date { get; set; }     // Booking date
        public string TimeSlot { get; set; }   // Selected time slot
        public int FieldId { get; set; }       // Selected field ID
        public string PaymentMethod { get; set; } // Payment method (e.g., Cash, PayPal)
        public decimal Amount { get; set; }    // Booking amount
    }
    public class AvailableSlotsRequest
    {
        public DateTime Date { get; set; }    // Date to fetch available slots
        public int FieldId { get; set; }      // Field ID to fetch available slots for that field
    }
    public class BookingDetailsRequest
    {
        public int BookingId { get; set; }  // ID of the booking whose details we want to fetch
    }
    public class BookingHistoryRequest
    {
        public string Email { get; set; }  // Email of the user whose booking history we want to fetch
    }
    public class GetUserBookingsRequest
    {
        public string Email { get; set; }  // Email of the user whose bookings we want to fetch
    }

    public class RescheduleBookingRequest
    {
        public int BookingId { get; set; }       // ID of the booking to be rescheduled
        public DateTime NewDate { get; set; }    // New date for the booking
        public string NewTimeSlot { get; set; }  // New time slot for the booking
        public string Email { get; set; }        // Email of the user rescheduling the booking
    }

    public class CancelBookingRequest
    {
        public int BookingId { get; set; }  // ID of the booking to be canceled
        public string Email { get; set; }   // Email of the user requesting cancellation
    }
    public class ReserveBookingRequest
    {
        public string SportType { get; set; }        // Type of sport
        public DateTime Date { get; set; }           // Booking date (start time)
        public int Duration { get; set; }            // Duration in minutes (e.g., 90 minutes)
        public string TimeSlot { get; set; }         // Selected time slot
        public int FieldId { get; set; }             // Selected field ID
        public string PaymentMethod { get; set; }    // Payment method (e.g., Cash, PayPal)
        public decimal Amount { get; set; }          // Booking amount
        public string Email { get; set; }            // Email of the user making the booking
    }


}
