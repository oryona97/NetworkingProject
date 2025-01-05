namespace eBookStore.Models.ViewModels;

public class GalleryPageViewModel
{
    public List<BookViewModel> allBooks { get; set; } = new List<BookViewModel>();
    public List<string> listOfCategorys { get; set; } = new List<string>();

}
