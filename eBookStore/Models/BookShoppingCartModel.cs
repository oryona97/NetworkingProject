
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class BookShoppingCartModel
{
	[Required(ErrorMessage = "bookId is required")]
	public int bookId { get; set; }

	[Required(ErrorMessage = "userId is required")]
	public int userId { get; set; }

	[Required(ErrorMessage = "format is required")]
	public string? format { get; set; }

	public DateTime createdAt { get; set; } = DateTime.Now;
}
