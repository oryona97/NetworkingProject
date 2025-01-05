namespace eBookStore.Models.ViewModels
{


    public class LandingPageViewModel
    {
        public List<BookViewModel> SpecialSales { get; set; } = new List<BookViewModel>(); // List of books on special sales
        public List<BookViewModel> allBooks { get; set; } = new List<BookViewModel>();// List of all the books in the store - needed for carousel
        public List<string> listOfCategorys { get; set; } = new List<string>();

    }
}
