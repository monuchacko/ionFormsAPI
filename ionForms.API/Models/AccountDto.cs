using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ionForms.API.Models
{
    public class AccountDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "You should provide a title.")]
        public string Title { get; set; }
        public string Description { get; set; }

        public int NumberOfForms
        {
            get
            {
                return Form.Count;
            }
        }

        public ICollection<FormDto> Form { get; set; }
        = new List<FormDto>();
    }
}