using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ionForms.API.Models
{
    public class ColumnForUpdateDto
    {
        [Required(ErrorMessage = "You should provide column name.")]
        [MaxLength(80)]
        public string ColumnName { get; set; }

        [Required(ErrorMessage = "You should provide column type.")]
        [MaxLength(100)]
        public string ColumnType { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        [MaxLength(20)]
        public string ColumnLength { get; set; }

        public string ColumnValue { get; set; }

        public string DefaultValue { get; set; }

        public bool IsRequired { get; set; }
    }
}