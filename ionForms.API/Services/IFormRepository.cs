using ionForms.API.Entities;
using System.Collections.Generic;

namespace ionForms.API.Services
{
    public interface IFormRepository
    {
        bool FormExists(int formId);
        IEnumerable<Form> GetForms();
        Form GetForm(int formId, bool includeColumn);
        IEnumerable<Column> GetColumnForForm(int formId);
        IEnumerable<Column> GetDistinctColumnForForm(int formId);
        Column GetColumnForForm(int formId, int columnId);
        void AddColumnForForm(int formId, Column column);
        void AddColumnsForForm(int formId, ICollection<Column> columns);
        void DeleteColumn(Column column);
        bool Save();
    }
}
