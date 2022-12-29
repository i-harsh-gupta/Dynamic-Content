using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DynamicContent.ViewModels
{
    public class PageEditViewModel : PageCreateViewModel
    {
        public int Id { get; set; }
        public string? ExistingPhotoPath { get; set; }
    }
}
