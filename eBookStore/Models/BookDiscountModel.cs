namespace eBookStore.Models;
using System.ComponentModel.DataAnnotations;

public class BookDiscountModel
{   
    [Required(ErrorMessage = "discountId is required")]
    public int bookId { get; set; }

    [Required(ErrorMessage = "discountPrecentage is required")]
    public float discountPrecentage { get; set; }

    public DateTime saleStartDate  { get; set; } = DateTime.Now;
    public DateTime saleEndDate  { get; set; } 
}