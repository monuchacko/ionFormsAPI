using System.ComponentModel.DataAnnotations;

namespace ionForms.API.Models
{
    public class FormForUpdateDto
    {
        [Required(ErrorMessage = "You should provide title value.")]
        [MaxLength(150)]
        public string Title { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
    }
}
