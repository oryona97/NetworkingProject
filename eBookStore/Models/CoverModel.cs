using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class CoverModel
{
    public int id { get; set; }
    [Required(ErrorMessage = "imgName is required")]
    public string? imgName { get; set; }


    [Required(ErrorMessage = "bookId is required")]
    public int bookId { get; set; }
    public DateTime createdAt { get; set; } = DateTime.Now;
}



