using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;


public class AdminModel
{
    [Required(ErrorMessage = "Username is required")]
    [RegularExpression("^[a-zA-Z0-9]+([._]?[a-zA-Z0-9]+)*$")]
    public string? Username { get; set; }
    [Required(ErrorMessage = "Password is required")]
    [RegularExpression("^[a-zA-Z0-9]+([._]?[a-zA-Z0-9]+)*$")]
    public string? Password { get; set; }
    [Required(ErrorMessage = "Email is required")]
    [RegularExpression("^[a-zA-Z0-9_.]+[a-zA-z0-9-]+.+[a-zA-Z0-9-.]+$")]
    public string? Email { get; set; }
    
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }


}