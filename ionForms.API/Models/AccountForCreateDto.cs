using System.ComponentModel.DataAnnotations;

namespace ionForms.API.Models
{
    public class AccountForCreateDto
    {
        [Required(ErrorMessage = "You should provide a title.")]
        public string Title { get; set; }
        public string Description { get; set; }

    }
}
