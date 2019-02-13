using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ionForms.API.Entities
{
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        public string Title { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Form> Forms { get; set; }
               = new List<Form>();
    }
}
