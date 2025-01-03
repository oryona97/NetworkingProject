using eBookStore.Models;

namespace eBookStore.Models.ViewModels;

public class GalleryPageViewModel
{
    public List<BookViewModel> allBooks {get; set; }
    public List<string> listOfCategorys {get; set;}

}