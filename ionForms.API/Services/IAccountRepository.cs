using ionForms.API.Entities;
using System.Collections.Generic;

namespace ionForms.API.Services
{
    public interface IAccountRepository
    {
        bool AccountExists(int accountId);
        IEnumerable<Account> GetAccounts();
        Account GetAccount(int accountId, bool includeForm);
        void DeleteAccount(Account account);
        IEnumerable<Form> GetFormForAccount(int accountId);
        void AddAccount(Account account);
        Form GetFormForAccount(int accountId, int formId);
        void AddFormForAccount(int accountId, Form form);
        void DeleteForm(Form form);
        bool Save();
    }
}
