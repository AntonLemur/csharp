using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("PRODUCTS")]
public class Product
{
    [Key]
    [Column("ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Column("NAME")]
    public string Name { get; set; }
    [Column("PRICE")]
    public decimal Price { get; set; }
    [Column("AVAILABLEQUANTITY")]
    public int AvailableQuantity { get; set; }

    public ICollection<Booking> Bookings { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; }
}