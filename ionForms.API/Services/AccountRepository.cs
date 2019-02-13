using ionForms.API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ionForms.API.Services
{
    public class AccountRepository : IAccountRepository
    {
        private FDDataContext _context;

        public AccountRepository(FDDataContext context)
        {
            _context = context;
        }

        public void AddAccount(Account account)
        {
            var sdsd = _context.Accounts.Add(account);
            //_context.Accounts.Add(account);
            string strint = "";
        }

        public void AddFormForAccount(int accountId, Form form)
        {
            var account = GetAccount(accountId, false);
            account.Forms.Add(form);
        }

        public bool AccountExists(int accountId)
        {
            return _context.Accounts.Any(c => c.Id == accountId);
        }

        public IEnumerable<Account> GetAccounts()
        {
            return _context.Accounts.OrderBy(c => c.Title).ToList();
        }

        public Account GetAccount(int accountId, bool includeForm)
        {
            if (includeForm)
            {
                return _context.Accounts.Include(c => c.Forms)
                    .Where(c => c.Id == accountId).FirstOrDefault();
            }

            return _context.Accounts.Where(c => c.Id == accountId).FirstOrDefault();
        }

        public Form GetFormForAccount(int accountId, int formId)
        {
            return _context.Forms
               .Where(p => p.AccountId == accountId && p.Id == formId).FirstOrDefault();
        }

        public IEnumerable<Form> GetFormForAccount(int accountId)
        {
            return _context.Forms
                           .Where(p => p.AccountId == accountId).ToList();
        }

        public void DeleteForm(Form form)
        {
            _context.Forms.Remove(form);
        }

        public void DeleteAccount(Account account)
        {
            _context.Accounts.Remove(account);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}