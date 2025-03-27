using System.ComponentModel.DataAnnotations;

namespace BuddgetWeb.Areas.User.Models
{
    public class CreateFinancialSpaceViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(255, ErrorMessage = "Name cannot exceed 255 characters")]
        public string Name { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string Description { get; set; }

        [Display(Name = "Space Image")]
        public IFormFile? Image { get; set; }
    }
}