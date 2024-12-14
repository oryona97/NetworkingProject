using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class FeedbackModel

{  
    [Required(ErrorMessage = "userId is required")]
    public int userId {get; set;} 


    [Required(ErrorMessage = "bookId is required")]
    public int bookId {get; set;}


    [Required(ErrorMessage = "Comment is required")]
    public string comment {get; set;}

    public DateTime createdAt { get; set; } = DateTime.now
    
}
  