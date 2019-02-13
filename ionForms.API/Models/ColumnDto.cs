namespace ionForms.API.Models
{
    public class ColumnDto
    {
        public int Id { get; set; }

        public string ColumnName { get; set; }

        public string ColumnType { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string ColumnLength{ get; set; }

        public string ColumnValue { get; set; }

        public string DefaultValue { get; set; }

        public bool IsRequired { get; set; }
    }
}
