namespace eBookStore.Models.ViewModels;
public class AdminDashViewModel
{
    public UserModel? userModel { get; set; }
    public BookViewModel? bookViewModel { get; set; }
    public List<PublisherModel> publishersList { get; set; } = new List<PublisherModel>();
    public List<string> genreList { get; set; } = new List<string>();
}
