using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class GeneralFeedbackModel
{   
    public int id {get; set;} 

    [Required(ErrorMessage = "userId is required")]
    public int userId {get; set;} 
    
    [Required(ErrorMessage = "Comment is required")]
    public string comment {get; set;}
    public DateTime createdAt { get; set; } = DateTime.now
    
}


  