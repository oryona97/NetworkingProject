using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class RatingModel
{   
    public int id {get; set;} 
    
    [Required(ErrorMessage = "id is required")]
    public int userId {get; set;} 

    [Required(ErrorMessage = "bookId is required")]
    public int bookId {get; set;} 
    public DateTime createdAt { get; set; } = DateTime.now
    
}


  