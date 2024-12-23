using eBookStore.Models;
using System.ComponentModel.DataAnnotations;
namespace eBookStore.Models.ViewModels;

public class ShoppingCartViewModel
{
   public ShoppingCartModel shoppingCart { get; set; } = new ShoppingCartModel();
}