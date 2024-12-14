using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class BookRentQueueModel
{   
    public int id {get; set;} 
    
    [Required(ErrorMessage = "bookId is required")]
    public int bookId {get; set;}

    [Required(ErrorMessage = "userId is required")]
    public int userId {get; set;}
    public DateTime createdAt { get; set; } = DateTime.now
    
}


  