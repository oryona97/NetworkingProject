using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class UserModel
{   
    [Key]
    public int id {get; set;} 

    [Required(ErrorMessage = "Username is required")]
    [RegularExpression("^[a-zA-Z0-9]+([._]?[a-zA-Z0-9]+)*$")]
    public string? username { get; set; }


    [Required(ErrorMessage = "Password is required")]
    [RegularExpression("^[a-zA-Z0-9]+([._]?[a-zA-Z0-9]+)*$")]
    public string? password { get; set; }


    [Required(ErrorMessage = "Email is required")]
    [RegularExpression("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$", ErrorMessage = "Invalid email address")]
    public string? email { get; set; }


    [Required(ErrorMessage = "First name is required")]
    [RegularExpression("^[A-Za-zא-ת'-]{2,30}$", ErrorMessage = "Invalid first name")]
    public string? firstName { get; set; }


    [Required(ErrorMessage = "Last name is required")]
    [RegularExpression("^[A-Za-zא-ת'-]{2,30}$", ErrorMessage = "Invalid last name")]
    public string? lastName { get; set; }


    [Required]
    [RegularExpression(@"^((\+972|972|0)?)(5[0-9]|[2-4|8|9])[-\s]?[0-9]{7}$", ErrorMessage = "Invalid phone number")]
    public string? phoneNumber { get; set; }

    [Required]
    public string type {get;set;} ="user";
    
    [Required]
    public DateTime createAt {get; set;} = DateTime.Now;

}