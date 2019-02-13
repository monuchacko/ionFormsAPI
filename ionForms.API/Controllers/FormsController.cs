using AutoMapper;
using ionForms.API.Models;
using ionForms.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace ionForms.API.Controllers
{
    [Route("api/accounts")]
    public class FormsController : Controller
    {
        private ILogger<FormsController> _logger;
        private IMailService _mailService;
        private IAccountRepository _accountRepository;

        public FormsController(ILogger<FormsController> logger, IMailService mailService, IAccountRepository accountRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _accountRepository = accountRepository;
        }

        [HttpGet("{accountId}/forms")]
        public IActionResult GetForms(int accountId)
        {
            try
            {
                if (!_accountRepository.AccountExists(accountId))
                {
                    _logger.LogInformation($"Account with id {accountId} wasn't found when accessing forms.");
                    return NotFound();
                }

                var formsForAccount = _accountRepository.GetFormForAccount(accountId);
                var formsForAccountResults =
                                   Mapper.Map<IEnumerable<FormDto>>(formsForAccount);

                return Ok(formsForAccountResults);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting form for account with id {accountId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        //private List<FormsDto> GetFormsData(string accountid)
        //{
        //    List<FormsDto> forms = new List<FormsDto>() { };
        //    string strConfigConnectionString = configuration.GetConnectionString("connFDConfig");

        //    using (SqlConnection conn = new SqlConnection(strConfigConnectionString))
        //    {
        //        using (SqlCommand SqlCmd = new SqlCommand())
        //        {
        //            SqlCmd.CommandText = "Select ID, AccountID, FormID, Title from tblForms where accountid='" + accountid + "'";
        //            SqlCmd.CommandType = CommandType.Text;
        //            SqlCmd.Connection = conn;
        //            SqlCmd.CommandTimeout = 0;

        //            conn.Open();

        //            SqlDataReader sqlDataReader = SqlCmd.ExecuteReader();
        //            if (sqlDataReader.HasRows)
        //            {
        //                while (sqlDataReader.Read())
        //                {
        //                    int dataId = (sqlDataReader.IsDBNull(0) ? 0 : sqlDataReader.GetInt32(0));
        //                    string dataAccountID = (sqlDataReader.IsDBNull(1) ? "" : sqlDataReader.GetString(1));
        //                    string dataFormID = (sqlDataReader.IsDBNull(2) ? "" : sqlDataReader.GetString(2));
        //                    string dataTitle = (sqlDataReader.IsDBNull(3) ? "" : sqlDataReader.GetString(3));

        //                    forms.Add(new FormsDto
        //                    {
        //                        Id = dataId,
        //                        AccountID = dataAccountID,
        //                        FormID = dataFormID,
        //                        Title = dataTitle
        //                    });
        //                }
        //            }
        //        }
        //    }

        //    return forms;
        //}

        [HttpGet("{accountId}/forms/{id}", Name = "GetForm")]
        public IActionResult GetForm(int accountId, int id)
        {
            if (!_accountRepository.AccountExists(accountId))
            {
                return NotFound();
            }

            var form = _accountRepository.GetFormForAccount(accountId, id);

            if (form == null)
            {
                return NotFound();
            }

            var formResult = Mapper.Map<FormDto>(form);
            return Ok(formResult);
        }

        [HttpPost("{accountId}/forms")]
        public IActionResult CreateForm(int accountId,
           [FromBody] FormForCreationDto form)
        {
            if (form == null)
            {
                return BadRequest();
            }

            if (form.Description == form.Title)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the title.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_accountRepository.AccountExists(accountId))
            {
                return NotFound();
            }

            var finalForm = Mapper.Map<Entities.Form>(form);

            _accountRepository.AddFormForAccount(accountId, finalForm);

            if (!_accountRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            var createdFormToReturn = Mapper.Map<Models.FormDto>(finalForm);

            return CreatedAtRoute("GetForm", new
            { accountId = accountId, id = createdFormToReturn.Id }, createdFormToReturn);
        }

        [HttpPut("{accountId}/forms/{id}")]
        public IActionResult UpdateForm(int accountId, int id,
            [FromBody] FormForUpdateDto form)
        {
            if (form == null)
            {
                return BadRequest();
            }

            if (form.Description == form.Title)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the title.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_accountRepository.AccountExists(accountId))
            {
                return NotFound();
            }

            var formEntity = _accountRepository.GetFormForAccount(accountId, id);
            if (formEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(form, formEntity);

            if (!_accountRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }


        [HttpPatch("{accountId}/forms/{id}")]
        public IActionResult PartiallyUpdateForm(int accountId, int id,
            [FromBody] JsonPatchDocument<FormForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!_accountRepository.AccountExists(accountId))
            {
                return NotFound();
            }

            var formEntity = _accountRepository.GetFormForAccount(accountId, id);
            if (formEntity == null)
            {
                return NotFound();
            }

            var formToPatch = Mapper.Map<FormForUpdateDto>(formEntity);

            patchDoc.ApplyTo(formToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (formToPatch.Description == formToPatch.Title)
            {
                ModelState.AddModelError("Description", "The provided description should be different from the title.");
            }

            TryValidateModel(formToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(formToPatch, formEntity);

            if (!_accountRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpDelete("{accountId}/forms/{id}")]
        public IActionResult DeleteForm(int accountId, int id)
        {
            if (!_accountRepository.AccountExists(accountId))
            {
                return NotFound();
            }

            var formEntity = _accountRepository.GetFormForAccount(accountId, id);
            if (formEntity == null)
            {
                return NotFound();
            }

            _accountRepository.DeleteForm(formEntity);

            if (!_accountRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            //_mailService.Send("Form deleted.",
            //        $"Form {formEntity.Title} with id {formEntity.Id} was deleted.");

            return NoContent();
        }
    }
}