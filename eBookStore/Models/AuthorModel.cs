using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;

public class AuthorModel
{
    [Required(ErrorMessage = "id is required")]
    public int id { get; set; }
    [Required(ErrorMessage = "bookId is required")]
    public int bookId { get; set; }

    [Required(ErrorMessage = "name is required")]
    public string? name { get; set; }
    public DateTime createdAt { get; set; } = DateTime.Now;
}
