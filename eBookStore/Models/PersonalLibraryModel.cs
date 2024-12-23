using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class PersonalLibraryModel
{
	

	[Required(ErrorMessage = "userId  is required")]
	public int userId { get; set; }

	[Required(ErrorMessage = "bookId is required")]
	public int bookId { get; set; }
	public DateTime createAt { get; set; } = DateTime.Now;
}
