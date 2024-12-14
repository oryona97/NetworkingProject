using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class PersonalLibraryModel
{
	public int id { get; set; }

	[Required(ErrorMessage = "userId  is required")]
	public int userId { get; set; }

	[Required(ErrorMessage = "bookId is required")]
	public int bookId { get; set; }
	public DateTime createAt { get; set; } = DateTime.Now;
}
