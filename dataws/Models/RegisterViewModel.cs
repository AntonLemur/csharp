using System.ComponentModel.DataAnnotations;

public class RegisterViewModel
{
    [Required]
    [EmailAddress(ErrorMessage = "Введите корректный email")]
    public string Email { get; set; }

    [Required]
    [RegularExpression(
    @"^[0-9\-\+\(\)\s]{10,20}$",
    ErrorMessage = "Введите корректный номер телефона")]    
    public string Phone { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [StringLength(
    100,
    MinimumLength = 6,
    ErrorMessage = "Минимум 6 символов")]
    public string Password { get; set; }
}