using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DynamicContent.ViewModels
{
    public class PageCreateViewModel
    {
        [Required]
        [Display(Name = "Page Title")]
        [MaxLength(50, ErrorMessage = "Page title should not exceed more than 50 characters.")]
        public string PageTitle { get; set; }

        [Required]
        [Display(Name = "Page Url")]
        [MaxLength(25, ErrorMessage = "Page url should not exceed more than 25 characters.")]
        public string PageUrl { get; set; }

        [Required, Display(Name = "Page Content")]
        public string PageContent { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
