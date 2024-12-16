using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models;



public class PublisherModel
{
	public int id { get; set; }
	public string name { get; set; }
	public DateTime createdAt { get; set; } = DateTime.Now;
}



