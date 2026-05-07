using System;

public class Booking
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string Status { get; set; } // Active / Cancelled / Expired
}