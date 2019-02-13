using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ionForms.API.Entities
{
    public class Form
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        public string Title { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [ForeignKey("AccountId")]
        public Account Account { get; set; }

        public int AccountId { get; set; }

        public bool IsDeleted { get; set; }

        public ICollection<Column> Columns { get; set; }
               = new List<Column>();

    }
}