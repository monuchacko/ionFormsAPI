using System.ComponentModel.DataAnnotations;

namespace ionForms.API.Models
{
    public class ColumnForCreationDto
    {
        [Required(ErrorMessage = "You should provide a column name.")]
        [MaxLength(80)]
        public string ColumnName { get; set; }

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
    }
}
