namespace eBookStore.Models.ViewModels;
using eBookStore.Models;

public class BookViewModel
{
    public BookViewModel()
    {
        book = new BookModel();
        rating = new RatingModel();
        feedbackModel = new List<FeedbackModel>();
        publishers = new List<PublisherModel>();
        publishers.Add(new PublisherModel());
        coverModel = new CoverModel();
        genreModel = new GenreModel();
        authorModel = new AuthorModel();
    }
    public BookModel book { get; set; }
    public RatingModel rating { get; set; }
    public List<FeedbackModel> feedbackModel { get; set; }
    public List<PublisherModel> publishers { get; set; }
    public CoverModel coverModel { get; set; }
    public UserModel? userModel { get; set; } = null;
    public GenreModel genreModel { get; set; }
    public AuthorModel authorModel { get; set; }
    public List<int> ownerUserIds { get; set; } = new List<int>();
    public BookDiscountModel? bookDiscountModel {get; set;}

}
