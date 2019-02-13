using ionForms.API.Helper;
using ionForms.API.Models;
using ionForms.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace ionForms.API.Controllers
{
    [Route("api/accounts")]
    public class FormActionsController : Controller
    {
        private ILogger<FormsController> _logger;
        private IMailService _mailService;
        private IAccountRepository _accountRepository;
        private IFormRepository _formRepository;

        public FormActionsController(ILogger<FormsController> logger, IMailService mailService, IAccountRepository accountRepository, IFormRepository formRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _accountRepository = accountRepository;
            _formRepository = formRepository;
        }

        [HttpPost("{accountId}/forms/{formId}/action")]
        public IActionResult CreateAction(int accountId, int formId,
           [FromBody] FormActionsDto formAction)
        {
            if (formAction == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_accountRepository.AccountExists(accountId))
            {
                return NotFound();
            }

            if (!_formRepository.FormExists(formId))
            {
                return NotFound();
            }

            var columnEntity = _formRepository.GetDistinctColumnForForm(formId);
            if (columnEntity == null)
            {
                return NotFound();
            }
            else
            {
                try
                {
                    var clientTablePrefix = Startup.Configuration["AppSettings:clientTablePrefix"];
                    var clientConnectionString = Startup.Configuration["ConnectionStrings:connFDClientData"];
                    ClientDBHelper clientDBHelper = new ClientDBHelper(accountId, formId, clientConnectionString, clientTablePrefix);

                    if (clientDBHelper.TableExists(clientDBHelper.GetClientTableName()))
                    {
                        clientDBHelper.UpdateTable(columnEntity);
                    }
                    else
                    {
                        clientDBHelper.CreateTable(columnEntity);
                    }
                }
                catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }
            }

            return Ok();
        }
    }
}