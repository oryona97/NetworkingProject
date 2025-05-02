using eBookStore.Models;
using System.ComponentModel.DataAnnotations;

namespace eBookStore.Models.ViewModels
{
    public class CreditCardViewModel
    {
        public UserModel userModel { get; set; }
        public CreditCardModel creditCard { get; set; }
    }
}
