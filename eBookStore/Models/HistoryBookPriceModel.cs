using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class HistoryBookPriceModel
{   
    public int id {get; set;} 

    [Required(ErrorMessage = "bookId is required")]
    public int bookId {get; set;} 
    

    [Required(ErrorMessage = "price is required")]
    public float price {get; set;}

    public DateTime datePrice { get; set; } = DateTime.now
    public DateTime createdAt { get; set; } = DateTime.now
    
}


  