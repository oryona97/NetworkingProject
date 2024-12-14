using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;
namespace eBookStore.Models;


public class BookModel
{
    public int id { get; set; }
    public int publisherId { get; set; }
    public int genreId { get; set; }
    public int amountOfCopies { get; set; }
    public string? title { get; set; }
    public float borrowPrice { get; set; }
    public float buyingPrice { get; set; }
    public DateTime pubDate { get; set; }
    public int ageLimit { get; set; }
    public int priceHistory { get; set; }
    public bool onSale { get; set; }
    public bool canBorrow { get; set; }
    public float starRate { get; set; }
    public DateTime createdAt { get; set; }
    
} 
  