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
    public class ColumnsController : Controller
    {
        private ILogger<ColumnsController> _logger;
        private IMailService _mailService;
        private IAccountRepository _accountRepository;
        private IFormRepository _formRepository;

        public ColumnsController(ILogger<ColumnsController> logger, IMailService mailService, IAccountRepository accountRepository, IFormRepository formRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _accountRepository = accountRepository;
            _formRepository = formRepository;
        }

        [HttpGet("{accountId}/forms/{formId}/columns")]
        public IActionResult GetColumns(int accountId, int formId)
        {
            try
            {
                if (!_accountRepository.AccountExists(accountId))
                {
                    _logger.LogInformation($"Account with id {accountId} wasn't found when accessing columns.");
                    return NotFound();
                }

                if (!_formRepository.FormExists(formId))
                {
                    _logger.LogInformation($"Form with id {formId} wasn't found when accessing forms.");
                    return NotFound();
                }

                var columnsForForm = _formRepository.GetColumnForForm(formId);
                var columnsForFormResults =
                                   Mapper.Map<IEnumerable<ColumnDto>>(columnsForForm);

                return Ok(columnsForFormResults);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting column for form with id {formId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
        }

        [HttpGet("{accountId}/forms/{formId}/columns/{id}", Name = "GetColumn")]
        public IActionResult GetColumn(int accountId, int formId, int id)
        {
            if (!_accountRepository.AccountExists(accountId))
            {
                return NotFound();
            }

            if (!_formRepository.FormExists(formId))
            {
                return NotFound();
            }

            var column = _formRepository.GetColumnForForm(formId, id);

            if (column == null)
            {
                return NotFound();
            }

            var columnResult = Mapper.Map<ColumnDto>(column);
            return Ok(columnResult);
        }

        [HttpPost("{accountId}/forms/{formId}/columns")]
        public IActionResult CreateColumns(int accountId, int formId,
           [FromBody] ICollection<ColumnForCreationDto> columns)
        {
            if (columns == null)
            {
                return BadRequest();
            }

            //if (column..Description == form.Title)
            //{
            //    ModelState.AddModelError("Description", "The provided description should be different from the title.");
            //}

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!_accountRepository.AccountExists(accountId))
            {
                return NotFound("Account Id[" + accountId +"] does not exist");
            }

            if (!_formRepository.FormExists(formId))
            {
                return NotFound("Form Id[" + formId + "] does not exist");
            }

            //var finalColumn = Mapper.Map<Entities.Column>(column);
            var finalColumns = Mapper.Map<ICollection<Entities.Column>>(columns);

            _formRepository.AddColumnsForForm(formId, finalColumns);

            //Entities.Column s = new Entities.Column();
            //s.Id

            //if (context.ObjectStateManager.GetObjectStateEntry(myEntity).State == EntityState.Detached)
            //{
            //    context.MyEntities.AddObject(myEntity);
            //}

            //// Attached object tracks modifications automatically

            //_formRepository.SaveChanges();

            try
            {
                if (!_formRepository.Save())
                {
                    return StatusCode(500, "A problem happened while handling your request.");
                }
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateException dbEx)
            {
                return BadRequest("Error saving. Please make sure there are no duplicate values.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return Ok();
        }

        [HttpPost("{accountId}/forms/{formId}/column")]
        public IActionResult CreateColumn(int accountId, int formId,
           [FromBody] ColumnForCreationDto column)
        {
            if (column == null)
            {
                return BadRequest();
            }

            //if (column..Description == form.Title)
            //{
            //    ModelState.AddModelError("Description", "The provided description should be different from the title.");
            //}

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

            var finalColumn = Mapper.Map<Entities.Column>(column);

            _formRepository.AddColumnForForm(formId, finalColumn);

            if (!_formRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            var createdColumnToReturn = Mapper.Map<Models.ColumnDto>(finalColumn);

            return CreatedAtRoute("GetColumn", new
            { accountId = accountId, formId = formId, id = createdColumnToReturn.Id }, createdColumnToReturn);
        }

        [HttpPut("{accountId}/forms/{formId}/columns/{id}")]
        public IActionResult UpdateForm(int accountId, int formId, int id,
            [FromBody] ColumnForUpdateDto column)
        {
            if (column == null)
            {
                return BadRequest();
            }

            //if (column.Description == column.Title)
            //{
            //    ModelState.AddModelError("Description", "The provided description should be different from the title.");
            //}

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

            var columnEntity = _formRepository.GetColumnForForm(formId, id);
            if (columnEntity == null)
            {
                return NotFound();
            }

            Mapper.Map(column, columnEntity);

            if (!_formRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpPatch("{accountId}/forms/{formId}/columns/{id}")]
        public IActionResult PartiallyUpdateColumn(int accountId, int formId, int id,
            [FromBody] JsonPatchDocument<ColumnForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!_accountRepository.AccountExists(accountId))
            {
                return NotFound();
            }

            if (!_formRepository.FormExists(formId))
            {
                return NotFound();
            }

            var columnEntity = _formRepository.GetColumnForForm(formId, id);
            if (columnEntity == null)
            {
                return NotFound();
            }

            var columnToPatch = Mapper.Map<ColumnForUpdateDto>(columnEntity);

            patchDoc.ApplyTo(columnToPatch, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //if (columnToPatch.Description == columnToPatch.Title)
            //{
            //    ModelState.AddModelError("Description", "The provided description should be different from the title.");
            //}

            TryValidateModel(columnToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(columnToPatch, columnEntity);

            if (!_formRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            return NoContent();
        }

        [HttpDelete("{accountId}/forms/{formId}/columns/{id}")]
        public IActionResult DeleteColumn(int accountId, int formId, int id)
        {
            if (!_accountRepository.AccountExists(accountId))
            {
                return NotFound();
            }

            if (!_formRepository.FormExists(formId))
            {
                return NotFound();
            }

            var columnEntity = _formRepository.GetColumnForForm(formId, id);
            if (columnEntity == null)
            {
                return NotFound();
            }

            _formRepository.DeleteColumn(columnEntity);

            if (!_formRepository.Save())
            {
                return StatusCode(500, "A problem happened while handling your request.");
            }

            //_mailService.Send("Form deleted.",
            //        $"Form {formEntity.Title} with id {formEntity.Id} was deleted.");

            return NoContent();
        }
    }
}