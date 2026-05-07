using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataApi.Models;
    
//Customer = бизнес-данные
[Table("CUSTOMERS")]
public class Customer
{
    [Key]
    [Column("ID")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Добавьте эту строку
    public long Id { get; set; }
    [Column("EMAIL")]
    public string Email { get; set; }     
    [Column("ADDRESS")]
    public string Address { get; set; }
    [Column("FIRSTNAME")]
    public string FirstName { get; set; }
    [Column("MIDDLENAME")]
    public string MiddleName { get; set; }
    [Column("LASTNAME")]
    public string LastName { get; set; }
    [Column("USERID")]
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
}
