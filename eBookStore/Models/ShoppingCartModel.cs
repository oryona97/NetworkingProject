
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class ShoppingCartModel
{

	[Required(ErrorMessage = "userId is required")]
	public int userId { get; set; }

	[Required(ErrorMessage = "bookId is required")]
	public int bookId { get; set; }
	public DateTime createdAt { get; set; } = DateTime.Now;
}



