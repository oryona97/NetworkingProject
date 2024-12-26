using eBookStore.Models;
using System.ComponentModel.DataAnnotations;
namespace eBookStore.Models.ViewModels;

public class BookViewModel
{
	public int id { get; set; }
	public BookModel book { get; set; }
	public RatingModel rating { get; set; }
	public List<FeedbackModel> feedbackModel { get; set; } = new List<FeedbackModel>();
	public PublisherModel publisherModel { get; set; }
	public CoverModel coverModel { get; set; }
	public UserModel userModel { get; set; }
	public GenreModel genreModel { get; set; }
	public List<int> ownerUserIds { get; set; } = new List<int>();
}
