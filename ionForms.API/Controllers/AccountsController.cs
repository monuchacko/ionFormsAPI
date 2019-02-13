using AutoMapper;
using ionForms.API.Helper;
using ionForms.API.Models;
using ionForms.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace ionForms.API.Controllers
{
    [Route("api/accounts")]
    public class AccountsController : Controller
    {
        //private IAccountRepository _accountRepository;
        private ILogger<ColumnsController> _logger;
        private IMailService _mailService;
        private IAccountRepository _accountRepository;
        private IFormRepository _formRepository;

        public AccountsController(ILogger<ColumnsController> logger, IMailService mailService, IAccountRepository accountRepository, IFormRepository formRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _accountRepository = accountRepository;
            _formRepository = formRepository;
        }

        //public AccountsController(IAccountRepository accountRepository)
        //{
        //    _accountRepository = accountRepository;
        //}

        [HttpGet()]
        public IActionResult GetAccounts()
        {
            var accountEntities = _accountRepository.GetAccounts();
            var results = Mapper.Map<IEnumerable<AccountWithoutFormsDto>>(accountEntities);

            return Ok(results);
        }

        [HttpGet("{id}")]
        public IActionResult GetAccount(int id, bool includeForms = false)
        {
            var account = _accountRepository.GetAccount(id, includeForms);

            if (account == null)
            {
                return NotFound();
            }

            if (includeForms)
            {
                var accountResult = Mapper.Map<AccountDto>(account);
                return Ok(accountResult);
            }

            var accountWithoutFormsResult = Mapper.Map<AccountWithoutFormsDto>(account);
            return Ok(accountWithoutFormsResult);
        }

        [HttpPost()]
        public IActionResult CreateAccount(
           [FromBody] AccountForCreateDto account, bool includeForms = false)
        {
            if (account == null)
            {
                return BadRequest();
            }

            if (account.Title == "")
            {
                ModelState.AddModelError("Title", "Title cannot be empty.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var finalAccount = Mapper.Map<Entities.Account>(account);

            _accountRepository.AddAccount(finalAccount);

            if (!_accountRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return Ok(finalAccount);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAccount(int id, [FromQuery] string action, [FromQuery] string clientaction)
        {
            if (action == "perm")
            {
                if (!_accountRepository.AccountExists(id))
                {
                    return NotFound();
                }

                var accountEntity = _accountRepository.GetAccount(id, true);
                if (accountEntity == null)
                {
                    return NotFound();
                }

                var formEntity = _accountRepository.GetFormForAccount(id);
                if (formEntity != null)
                {
                    int formId;
                    foreach (Entities.Form form in formEntity)
                    {
                        formId = form.Id;
                        var columnEntity = _formRepository.GetColumnForForm(formId);
                        if (columnEntity != null)
                        {
                            foreach (Entities.Column column in columnEntity)
                            {
                                _formRepository.DeleteColumn(column);
                            }
                        }

                        _accountRepository.DeleteForm(form);

                        if (clientaction != null && clientaction == "cleanall")
                        {
                            var clientTablePrefix = Startup.Configuration["AppSettings:clientTablePrefix"];
                            var clientConnectionString = Startup.Configuration["ConnectionStrings:connFDClientData"];
                            ClientDBHelper clientDBHelper = new ClientDBHelper(id, formId, clientConnectionString, clientTablePrefix);

                            clientDBHelper.DropClientTable();
                        }
                    }
                }

                //Delete Acccount
                _accountRepository.DeleteAccount(accountEntity);

                if (!_accountRepository.Save())
                {
                    return StatusCode(500, "A problem happened while handling your request.");
                }

                //_mailService.Send("Form deleted.",
                //        $"Form {formEntity.Title} with id {formEntity.Id} was deleted.");

                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAccount(int id,
            [FromBody] AccountForUpdateDto account, bool includeForms = false)
        {
            if (account == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_accountRepository.AccountExists(id))
            {
                return NotFound();
            }

            var accountEntity = _accountRepository.GetAccount(id, includeForms);
            if (accountEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(account, accountEntity);

            if (!_accountRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return Ok(accountEntity);
        }

        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateAccount(int id,
            [FromBody] JsonPatchDocument<AccountForUpdateDto> patchDoc, bool includeForms = false)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!_accountRepository.AccountExists(id))
            {
                return NotFound();
            }

            var accountEntity = _accountRepository.GetAccount(id, includeForms);
            if (accountEntity == null)
            {
                return NotFound();
            }

            var accountToPatch = Mapper.Map<AccountForUpdateDto>(accountEntity);

            patchDoc.ApplyTo(accountToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //if (formToPatch.Description == formToPatch.Title)
            //{
            //    ModelState.AddModelError("Description", "The provided description should be different from the title.");
            //}

            TryValidateModel(accountToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(accountToPatch, accountEntity);

            if (!_accountRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return Ok(accountToPatch);
        }

        //[HttpPost("{createaccount}")]
        //public IActionResult CreateAccount()
        //{
        //    string strConfigConnectionString = configuration.GetConnectionString("connFDConfig");

        //    string sqlQuery = "INSERT INTO tblAccounts (AccountID, CreateDate, ModifiedDate) Values ('acct_1', getdate(), getdate())";
        //    //string strGetNewID = "Select @@Identity";

        //    using (SqlConnection conn = new SqlConnection(strConfigConnectionString))
        //    {
        //        conn.Open();
        //        SqlCommand command = new SqlCommand(sqlQuery, conn);
        //        command.ExecuteNonQuery();
        //        //command.CommandText = strGetNewID;
        //        //RecordID = command.ExecuteScalar().ToString();
        //    }

        //    return Ok(AccountsDataStore.Current.Accounts);
        //}

        //[HttpGet("{id}")]
        //public IActionResult GetAccount(int id)
        //{
        //    var accountToReturn = AccountsDataStore.Current.Accounts.FirstOrDefault(c => c.Id == id);
        //    if (accountToReturn == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(accountToReturn);
        //}

        //[HttpGet("{accountid}")]
        //public IActionResult GetAccount(string accountid)
        //{
        //    var accountToReturn = AccountsDataStore.Current.Accounts.FirstOrDefault(c => c.AccountID == accountid);
        //    if (accountToReturn == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(accountToReturn);
        //}
    }
}