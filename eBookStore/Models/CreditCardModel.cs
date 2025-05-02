using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;

public class CreditCardModel
{
    [Required(ErrorMessage = "UserId is required")]
    public int userId { get; set; }

    [Required(ErrorMessage = "creditCardNumber is required")]
    [RegularExpression(@"^\d{16}$", ErrorMessage = "Credit card number must be 16 digits")]
    public string creditCardNumber { get; set; }

    [Required(ErrorMessage = "validDate is required")]
    [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Valid date must be in MM/YY format")]
    public string validDate { get; set; }

    [Required(ErrorMessage = "cvc is required")]
    [RegularExpression(@"^\d{3}$", ErrorMessage = "CVC must be 3 ")]
    public string cvc { get; set; }
}