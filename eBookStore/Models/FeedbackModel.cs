namespace eBookStore.Models;

public class FeedbackModel

{
    public int id { get; set; }
    public int userId { get; set; }
    public int bookId { get; set; }
    public string? comment { get; set; }
    public DateTime createdAt { get; set; } = DateTime.Now;
    public UserModel? userModel { get; set; }
}

