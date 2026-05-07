using System.Collections.Generic;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public int AvailableQuantity { get; set; }

    public ICollection<Booking> Bookings { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}