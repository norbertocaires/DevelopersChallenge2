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

namespace Nibo.Controllers {
    [Route("[controller]/[action]")]
    public class OfxController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IHubContext<NotifyHub, ITypedHubClient> _hubContext;

        public OfxController(
                //ApplicationDbContext context,
                IHubContext<NotifyHub, ITypedHubClient> hubContext
        ) {
            //_context = context;
            _hubContext = hubContext;
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile file) {
            try {
                var helperOFX = new ReadOFX();
                var transactions = helperOFX.ReadTransactions(file, _hubContext);
                Task.Run(() => helperOFX.UnifyTransactions(transactions, _hubContext));
                return Ok();
            } catch (AggregateException) {
                return StatusCode(400);
            } catch (Exception ex) {
                return StatusCode(400, new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult ListTransactionsPaged(int page, int rows) {
            try {
                var helperOFX = new ReadOFX();
                var transactions = helperOFX.GetTransactionsPage(page, rows);
                return Ok(transactions);
            } catch (AggregateException ex) {
                return StatusCode(400, new { message = ex.Message });
            } catch (Exception ex) {
                return StatusCode(400, new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetTotalTransactions() {
            try {
                var helperOFX = new ReadOFX();
                return Ok(helperOFX.GetTransactions().Count());
            } catch (AggregateException ex) {
                return StatusCode(400, new { message = ex.Message });
            } catch (Exception ex) {
                return StatusCode(400, new { message = ex.Message });
            }
        }
    }
}
