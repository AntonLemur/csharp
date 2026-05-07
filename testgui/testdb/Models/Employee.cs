using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testdb.Models;
public class Employee
{
    public long Id { get; set; }
    public string? FIRST_NAME { get; set; }
    public string? MIDDLE_NAME { get; set; }
    public string? LAST_NAME { get; set; }
    public long? FIRM_ID { get; set; }
    public short IS_PHOTO { get; set; }
}
