
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class RecieptModel
{   
    public int id { get; set; }

    [Required(ErrorMessage = "userId is required")]
    public int userId {get; set;} 

    [Required(ErrorMessage = "bookId is required")]
    public int bookId { get; set; }

    [Required(ErrorMessage = "Total is required")]
    public float total { get; set; }
    public DateTime createdAt { get; set; } = DateTime.now
    
}


  