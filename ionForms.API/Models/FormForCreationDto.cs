using System.ComponentModel.DataAnnotations;

namespace ionForms.API.Models
{
    public class FormForCreationDto
    {
        [Required(ErrorMessage = "You should provide a title value.")]
        [MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
    }
}
