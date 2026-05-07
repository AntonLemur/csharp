    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataApi.Models;
    
[Table("ORDERS")]
public class Order
{
    [Key]
    [Column("ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Column("ORDER_DATE")]
    public DateTime? OrderDate { get; set; }

    [Column("AMOUNT")]
    public float Amount { get; set; }

    [Column("STATUS")]
    public short Status { get; set; }

    [Column("CUSTOMER_ID")] // Внешний ключ
    public long CustomerId { get; set; }

    // 2. Навигационное свойство — сама сущность клиента
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }

    public ICollection<OrderItem> Items { get; set; }    
}