using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

public class KycRequestViewModel
{
    [Required]
    public string PassportNumber { get; set; }

    [Required]
    public IFormFile Document { get; set; }
}