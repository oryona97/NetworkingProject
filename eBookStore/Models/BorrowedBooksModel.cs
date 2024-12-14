using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class BorrowedBooksModel
{   
    public int id {get; set;} 
    public int userId {get; set;}
    public int bookId { get; set; }
    public DateTime createdAt { get; set; }
    
}


  