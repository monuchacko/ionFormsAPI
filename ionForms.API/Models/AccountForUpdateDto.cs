using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ionForms.API.Models
{
    public class AccountForUpdateDto
    {
        [Required(ErrorMessage = "You should provide a title.")]
        public string Title { get; set; }
        public string Description { get; set; }

    }
}
