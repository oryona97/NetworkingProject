using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class PublisherModel
{   
    public int id {get; set;} 
    [Required(ErrorMessage = "Name is required")]
    public int name {get; set;}
    public DateTime createdAt { get; set; } = DateTime.now
    
}


  