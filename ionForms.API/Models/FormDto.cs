using System.Collections.Generic;

namespace ionForms.API.Models
{
    public class FormDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public int NumberOfColumns
        {
            get
            {
                return Column.Count;
            }
        }

        public ICollection<ColumnDto> Column { get; set; }
        = new List<ColumnDto>();

    }
}
