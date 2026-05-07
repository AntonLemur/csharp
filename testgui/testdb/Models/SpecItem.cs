using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using System.Collections.Generic;

namespace testdb.Models;

[Table("SPECIFICATIONS")]
public class SpecItem
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }
    [Column("NAME")]
    public string? Name { get; set; }
    // Внешний ключ
    [Column("PARENT_ID")]
    public int? ParentId { get; set; }
    // Навигационное свойство для родителя
    public virtual SpecItem? Parent { get; set; }
    // Коллекция детей (то, что мы привяжем к TreeDataGrid)
    public virtual ICollection<SpecItem> Children { get; set; } = [];
    [Column("ARTICUL")]
    public string? Articul { get; internal set; }
    [Column("QUANTITY")]
    public decimal Quantity { get; internal set; }
    [Column("UNIT")]
    public string? Unit { get; internal set; }
}
