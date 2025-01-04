namespace eBookStore.Models
{
    public class UserNotificationModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}