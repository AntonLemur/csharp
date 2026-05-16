using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataApi.Models;

[Table("ORDERITEMS")]
public class OrderItem
{
    [Key]
    [Column("ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Column("ORDERID")]
    public long OrderId { get; set; }
    public Order Order { get; set; }
    [Column("PRODUCTID")]
    public int ProductId { get; set; }
    public Product Product { get; set; }
    [Column("QUANTITY")]
    public int Quantity { get; set; }
    [Column("PRICE")]
    public decimal Price { get; set; } // цена на момент заказа
}