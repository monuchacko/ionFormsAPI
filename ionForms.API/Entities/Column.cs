using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ionForms.API.Entities
{
    public class Column
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        public string ColumnName { get; set; }

        [Required]
        [MaxLength(100)]
        public string ColumnType { get; set; }

        [MaxLength(500)]
        public string DisplayName { get; set; }

        [MaxLength(5000)]
        public string Description { get; set; }

        [MaxLength(20)]
        public string ColumnLength { get; set; }

        public string ColumnValue { get; set; }

        public string DefaultValue { get; set; }

        public bool IsRequired { get; set; }

        public bool IsDeleted { get; set; }

        [ForeignKey("FormId")]
        public Form Form { get; set; }

        public int FormId { get; set; }
    }
}
