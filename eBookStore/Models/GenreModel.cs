using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class GenreModel
{   
    public int id {get; set;} 
    
    [Required(ErrorMessage = "Name is required")]
    public string name {get; set;}
    public DateTime createdAt { get; set; } = DateTime.now
    
}


  