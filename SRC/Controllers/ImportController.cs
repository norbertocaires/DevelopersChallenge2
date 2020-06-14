using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nibo.Helper;
using Nibo.Services;
using Microsoft.AspNetCore.SignalR;
using Nibo.Services.Interfaces;
using Nibo.Models.ViewModels;
using System.Linq;
using Nibo.Data;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using System.Text;
using Nibo.Services.Blob;

namespace Nibo.Controllers {
    [Route("[controller]/[action]")]
    public class ImportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotifyHub, ITypedHubClient> _hubContext;
        private readonly IConfiguration _configuration;

        public ImportController(
                ApplicationDbContext context,
                IHubContext<NotifyHub, ITypedHubClient> hubContext,
                IConfiguration configuration
        ) {
            _context = context;
            _hubContext = hubContext;
            _configuration = configuration;
        }

        /// <summary>
        /// Get page of Imports to show frotend
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns>list imports</returns>
        [HttpGet]
        public IActionResult ListImportsPaged(int page, int rows) {
            try {
                var toReturn = new List<ImportViewModel>();
                var toSkip = (page - 1) * rows;
                toReturn = _context.Import.OrderBy(t => t.Date).Skip(toSkip).Take(rows).ToList().ConvertAll(i => new ImportViewModel(i));
                return Ok(toReturn);
            } catch (AggregateException ex) {
                return StatusCode(400, new { message = ex.Message });
            } catch (Exception ex) {
                return StatusCode(400, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get number of total imports
        /// </summary>
        /// <returns>Number total imports</returns>
        [HttpGet]
        public IActionResult GetTotalImports() {
            try {
                var total = _context.Import.Count();
                return Ok(total);
            } catch (AggregateException ex) {
                return StatusCode(400, new { message = ex.Message });
            } catch (Exception ex) {
                return StatusCode(400, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Download file of duplicates
        /// </summary>
        /// <param name="fileName">
        /// Name file to download from BLOB
        /// </param>
        /// <returns>File</returns>
        [HttpGet]
        public IActionResult DownloadFileDuplicate(string fileName) {
            try {
                if (fileName.IsNullOrEmptyOrBlank()) {
                    return StatusCode(404, "Erro downloading file");
                }
                var repository = new AzureFile();
                var csv = repository.GetFile(fileName, "duplicates");
                var file = File(new UTF8Encoding().GetBytes(csv), "text/csv", fileName);
                return file;
            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Download file OFX
        /// </summary>
        /// <param name="fileName">
        /// Name file to download from BLOB
        /// </param>
        /// <returns>File</returns>
        [HttpGet]
        public IActionResult DownloadFile(string fileName) {
            try {
                if (fileName.IsNullOrEmptyOrBlank()) {
                    return StatusCode(404, "Erro downloading file");
                }
                var repository = new AzureFile();
                var csv = repository.GetFile(fileName, "importedofx");
                var file = File(new UTF8Encoding().GetBytes(csv), "text/ofx", fileName);
                return file;
            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}
