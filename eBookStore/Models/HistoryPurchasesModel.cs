using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class HistoryPurchasesModel
{
	public int id { get; set; }
	[Required(ErrorMessage = "bookId is required")]
	public int bookId { get; set; }

	[Required(ErrorMessage = "Book price is required")]
	public int bookPrice { get; set; }

	[Required(ErrorMessage = "userId is required")]
	public int userId { get; set; }

	public DateTime purchaseDate { get; set; } = DateTime.Now;
	public DateTime createdAt { get; set; } = DateTime.Now;
}


