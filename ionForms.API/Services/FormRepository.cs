using ionForms.API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ionForms.API.Services
{
    public class FormRepository : IFormRepository
    {
        private FDDataContext _context;

        public FormRepository(FDDataContext context)
        {
            _context = context;
        }

        public void AddColumnForForm(int formId, Column column)
        {
            var form = GetForm(formId, false);
            form.Columns.Add(column);
        }

        public void AddColumnsForForm(int formId, ICollection<Column> columns)
        {
            var form = GetForm(formId, false);
            form.Columns = columns;
        }

        public bool FormExists(int formId)
        {
            return _context.Forms.Any(c => c.Id == formId);
        }

        public IEnumerable<Form> GetForms()
        {
            return _context.Forms.OrderBy(c => c.Title).ToList();
        }

        public Form GetForm(int formId, bool includeColumn)
        {
            if (includeColumn)
            {
                return _context.Forms.Include(c => c.Columns)
                    .Where(c => c.Id == formId).FirstOrDefault();
            }

            return _context.Forms.Where(c => c.Id == formId).FirstOrDefault();
        }

        public Column GetColumnForForm(int formId, int columnId)
        {
            return _context.Columns
               .Where(p => p.FormId == formId && p.Id == columnId).FirstOrDefault();
        }

        public IEnumerable<Column> GetColumnForForm(int formId)
        {
            return _context.Columns
                           .Where(p => p.FormId == formId).ToList();
        }

        public IEnumerable<Column> GetDistinctColumnForForm(int formId)
        {
            return _context.Columns
                .Where(p => p.FormId == formId)
                .GroupBy(x => x.ColumnName).Select(x => x.FirstOrDefault());

            //return _context.Columns
            //    .Where(p => p.FormId == formId)
            //    .Distinct().ToList();

        }

        public void DeleteColumn(Column column)
        {
            _context.Columns.Remove(column);
        }
        
        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
