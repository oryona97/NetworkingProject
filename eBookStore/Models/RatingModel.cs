using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class RatingModel
{
	[Required(ErrorMessage = "id is required")]
	public int starRating { get; set; }

	[Required(ErrorMessage = "userId is required")]
	public int userId { get; set; }
	
	[Required(ErrorMessage = "bookId is required")]
	public int bookId { get; set; }

	[Required(ErrorMessage = "comment is required")]
	public DateTime createdAt { get; set; } = DateTime.Now;

}



