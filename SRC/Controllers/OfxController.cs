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
using Nibo.Models;

namespace Nibo.Controllers {
    [Route("[controller]/[action]")]
    public class OfxController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotifyHub, ITypedHubClient> _hubContext;
        private readonly IConfiguration _configuration;

        public OfxController(
                ApplicationDbContext context,
                IHubContext<NotifyHub, ITypedHubClient> hubContext,
                IConfiguration configuration
        ) {
            _context = context;
            _hubContext = hubContext;
            _configuration = configuration;
        }

        /// <summary>
        /// Upload file OFX, save transactions on db
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult UploadFile(IFormFile file) {
            try {
                if (file == null) {
                    return StatusCode(400, "Choise a File");
                }

                var helperOFX = new ReadOFX(_configuration);
                var transactions = helperOFX.ReadTransactions(file, _hubContext, out string urlPathOFX);
                if (transactions.Count() > 0) {
                    Task.Run(() => helperOFX.UnifyTransactions(transactions, _hubContext, urlPathOFX));
                }
                return Ok();
            } catch (AggregateException) {
                return StatusCode(400);
            } catch (Exception ex) {
                return StatusCode(400, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get page of Transactions to show frotend
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns>list of transactions</returns>
        [HttpGet]
        public IActionResult ListTransactionsPaged(int page, int rows) {
            try {
                var toReturn = new List<TransactionViewModel>();
                var toSkip = (page - 1) * rows;
                toReturn = _context.Transaction.OrderBy(t => t.Date).Skip(toSkip).Take(rows).ToList().ConvertAll(t => new TransactionViewModel(t));
                return Ok(toReturn);
            } catch (AggregateException ex) {
                return StatusCode(400, new { message = ex.Message });
            } catch (Exception ex) {
                return StatusCode(400, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get number of total transactions
        /// </summary>
        /// <returns>number toral transactions</returns>
        [HttpGet]
        public IActionResult GetTotalTransactions() {
            try {
                var total = _context.Transaction.Count();
                return Ok(total);
            } catch (AggregateException ex) {
                return StatusCode(400, new { message = ex.Message });
            } catch (Exception ex) {
                return StatusCode(400, new { message = ex.Message });
            }
        }
    }
}
