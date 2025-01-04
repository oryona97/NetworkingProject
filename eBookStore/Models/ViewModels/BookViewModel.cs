using eBookStore.Models;
using System.ComponentModel.DataAnnotations;
namespace eBookStore.Models.ViewModels;

public class BookViewModel
{
    public BookViewModel()
    {
        book = new BookModel();
        rating = new RatingModel();
        feedbackModel = new List<FeedbackModel>();
        publisherModel = new PublisherModel();
        coverModel = new CoverModel();
        genreModel = new GenreModel();
        authorModel = new AuthorModel();
    }
    public BookModel book { get; set;}
    public RatingModel rating { get; set;}
    public List<FeedbackModel> feedbackModel { get; set;}
    public PublisherModel publisherModel { get; set;}
    public CoverModel coverModel { get; set;}
    public UserModel? userModel { get; set;} = null;
    public GenreModel genreModel { get; set;}
    public AuthorModel authorModel { get; set;}
    public List<int> ownerUserIds { get; set;} =new List<int>();
}
