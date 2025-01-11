namespace eBookStore.Models.ViewModels
{


    public class LandingPageViewModel
    {
        public List<BookViewModel> allBooks { get; set; } // List of all the books in the store - needed for carousel
        public List<string> listOfCategorys { get; set; }
        public List<GeneralFeedbackModel> allFeedbacks { get; set; }

    }
}
