using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class RatingModel
{
	public int starRating { get; set; }

	public int userId { get; set; }

	public int bookId { get; set; }
	public DateTime createdAt { get; set; } = DateTime.Now;

}



