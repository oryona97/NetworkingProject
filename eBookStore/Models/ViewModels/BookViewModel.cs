using eBookStore.Models;
using System.ComponentModel.DataAnnotations;
namespace eBookStore.Models.ViewModels;

public class BookViewModel
{
    public BookModel book { get; set;}
    public RatingModel rating { get; set;}
    public FeedbackModel feedbackModel { get; set;}
    public PublisherModel publisherModel { get; set;}
     
}