using eBookStore.Models;
using System.ComponentModel.DataAnnotations;
namespace eBookStore.Models.ViewModels;


public class PersonalLibraryViewModel
{
   public PersonalLibraryModel personalLibrary { get; set; }= new PersonalLibraryModel();
    
}