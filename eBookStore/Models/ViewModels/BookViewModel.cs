using eBookStore.Models;
using System.ComponentModel.DataAnnotations;
namespace eBookStore.Models.ViewModels;

public class BookViewModel
{
    public BookModel book { get; set;}
    public RatingModel rating { get; set;}
    public List<FeedbackModel> feedbackModel { get; set;}
    public PublisherModel publisherModel { get; set;}
    public CoverModel coverModel { get; set;}
    public UserModel userModel { get; set;}
    public GenreModel genreModel { get; set;}
    public AuthorModel authorModel { get; set;}

}