using AutoMapper;
using ionForms.API.Helper;
using ionForms.API.Models;
using ionForms.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ionForms.API.Controllers
{
    [Route("api/client")]
    public class ClientColumnsController : Controller
    {
        private ILogger<ColumnsController> _logger;
        private IMailService _mailService;
        private IAccountRepository _accountRepository;
        private IFormRepository _formRepository;

        public ClientColumnsController(ILogger<ColumnsController> logger, IMailService mailService, IAccountRepository accountRepository, IFormRepository formRepository)
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
                _logger.Log(LogLevel.Information, "columns");
                _logger.Log(LogLevel.Critical, "columns test");
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

        //[HttpGet("GenerateCSV1")]
        //public HttpResponseMessage GenerateCSV1()
        //{
        //    var sb = new StringBuilder();
        //    sb.Append("cdsdfl dsljfdlskjfdls;kjf");

        //    HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

        //    result.Content = new StringContent(sb.ToString());
        //    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        //    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment"); //attachment will force download
        //    result.Content.Headers.ContentDisposition.FileName = "RecordExport.csv";


        //    //result = Request.CreateResponse(HttpStatusCode.OK);
        //    //result.Content = new StreamContent(new FileStream("localFilePath", FileMode.Open, FileAccess.Read));
        //    //result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
        //    //result.Content.Headers.ContentDisposition.FileName = "SampleImg";

        //    return result;
        //}

        //[HttpGet("GenerateCSV")]
        //public HttpResponseMessage GenerateCSV()
        //{
        //    var stream = GenerateStreamFromString("a,b \n c,d");
        //    //var stream = new MemoryStream();

        //    // processing the stream.

        //    var result = new HttpResponseMessage(HttpStatusCode.OK)
        //    {
        //        Content = new StringContent("sdsfsdf")
        //        //Content = new ByteArrayContent(stream.ToArray())
        //    };
        //    result.Content.Headers.ContentDisposition =
        //        new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
        //        {
        //            FileName = "CertificationCard.pdf"
        //        };
        //    result.Content.Headers.ContentType =
        //        new MediaTypeHeaderValue("application/octet-stream");

        //    //using (var stream1 = GenerateStreamFromString("a,b \n c,d"))
        //    //{
        //    //    // ... Do stuff to stream
        //    //    byte[] bdata = stream1.ToArray();

        //    //}

        //    return result;
        //}

        public MemoryStream GenerateStreamFromString(string sdata)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(sdata);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        [HttpGet("{accountId}/forms/{formId}/PyDataFrame")]
        public IActionResult PyDataFrame(int accountId, int formId)
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

                //var columnsForForm = _formRepository.GetDistinctColumnForForm(formId);
                //var columnsForFormResults = Mapper.Map<IEnumerable<ColumnDto>>(columnsForForm);

                var clientTablePrefix = Startup.Configuration["AppSettings:clientTablePrefix"];
                var clientConnectionString = Startup.Configuration["ConnectionStrings:connFDClientData"];
                ClientDBHelper clientDBHelper = new ClientDBHelper(accountId, formId, clientConnectionString, clientTablePrefix);

                var sqlQuery = clientDBHelper.GetClientDataReadQuery();
                //"SELECT [Id] ,[Col1] ,[Col1_] ,[Col1_1] ,[Col1_2] ,[Col1_3] ,[Col1_4] ,[Col2] FROM [dbo].[tblClient_3_5]";
                StringBuilder sb = clientDBHelper.GetClientDataPyDataFrameCSV(sqlQuery);

                var stream = GenerateStreamFromString(sb.ToString());
                return new FileContentResult(stream.ToArray(), "application/octet-stream");
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Exception while getting column for form with id {formId}.", ex);
                return StatusCode(500, "A problem happened while handling your request.");
            }
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
                return NotFound();
            }

            if (!_formRepository.FormExists(formId))
            {
                return NotFound();
            }

            //var finalColumn = Mapper.Map<Entities.Column>(column);
            var finalColumns = Mapper.Map<ICollection<Entities.Column>>(columns);

            var clientTablePrefix = Startup.Configuration["AppSettings:clientTablePrefix"];
            var clientConnectionString = Startup.Configuration["ConnectionStrings:connFDClientData"];
            ClientDBHelper clientDBHelper = new ClientDBHelper(accountId, formId, clientConnectionString, clientTablePrefix);

            clientDBHelper.InsertClientData(finalColumns);

            return Ok();
        }

    }
}
