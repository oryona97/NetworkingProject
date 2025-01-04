namespace eBookStore.Models;
using System.ComponentModel.DataAnnotations;

public class BookModel
{
	public int id { get; set; }
	public int publisherId { get; set; }
	
	[Required(ErrorMessage = "genreId is required")]
	public int genreId { get; set; }

	[Required(ErrorMessage = "amountOfCopies is required")]
	public int amountOfCopies { get; set; }

	[Required(ErrorMessage = "title is required")]
	public string? title { get; set; }

	[Required(ErrorMessage = "borrowPrice is required")]
	public float borrowPrice { get; set; }

	[Required(ErrorMessage = "buyingPrice is required")]
	public float buyingPrice { get; set; }

	[Required(ErrorMessage = "pubDate is required")]
	public DateTime pubDate { get; set; }

	[Required(ErrorMessage = "agelimit is required")]
	public int ageLimit { get; set; }

	[Required(ErrorMessage = "historyPrice is required")]
	public int priceHistory { get; set; }

	public bool onSale { get; set; }

	[Required(ErrorMessage = "canborrow is required")]
	public bool canBorrow { get; set; }

	[Required(ErrorMessage = "startRate is required")]
	public float starRate { get; set; }

	public DateTime createdAt { get; set; }= DateTime.Now;

}

