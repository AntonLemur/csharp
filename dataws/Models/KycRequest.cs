using System;
using System.ComponentModel.DataAnnotations.Schema;

[Table("KYCREQUESTS")]
public class KycRequest
{
    public long Id { get; set; }

    public string UserId { get; set; }

    public ApplicationUser User { get; set; }

    public string PassportNumber { get; set; }

    public string DocumentFileName { get; set; }

    public DateTime CreatedAt { get; set; }

    public KycStatus Status { get; set; }
}