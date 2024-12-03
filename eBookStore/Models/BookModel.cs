using Microsoft.VisualBasic;

namespace eBookStore.Models;


public class BookModel
{
    public string? Name { get; set; }
    public string? ID { get; set; }
    public string? FileFormat { get; set; }
    public List<string>? Author { get; set; }
    public string? Title { get; set; }
    public DateTime YearOfPublish { get; set; }
    public string? BuyingPrice { get; set; }
    public string? BorrowPrice { get; set; }
    public List<string>? Publishers { get; set; }
    public bool? forSale { get; set; }
    public bool? forRent { get; set; }
}