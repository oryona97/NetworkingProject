using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class GeneralRatingModel
{   
    public int id {get; set;} 
    
    [Required(ErrorMessage = "starRating is required")]
    public int starRating {get; set;}

    [Required(ErrorMessage = "userId is required")]
    public int userId {get; set;}

    public DateTime createdAt { get; set; } = DateTime.now
    
}


  