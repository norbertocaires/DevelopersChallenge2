using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Nibo.Models;
using Nibo.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nibo.Services.Interfaces;
using Nibo.Services.Blob;
using System.Collections.Concurrent;
using System.Text;
using System.Security.Cryptography;
using Nibo.Models.ViewModels;
using Nibo.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using EFCore.BulkExtensions;

namespace Nibo.Helper {
    public class ReadOFX {


        private readonly IConfiguration _configuration;

        public ReadOFX(
            IConfiguration configuration
            ) {
            _configuration = configuration;
        }

        /// <summary>
        /// Unify and save transaction on db
        /// </summary>
        /// <param name="transactions"></param>
        /// <param name="_hubContext"></param>
        /// <param name="urlFileImported"></param>
        public void UnifyTransactions(List<Transaction> transactions, IHubContext<NotifyHub, ITypedHubClient> _hubContext, string urlFileImported) {
            _hubContext.Clients.All.UpdatePercent("Unifying transactions");
            var dc = getNewDataContext();
            var duplication = new List<Transaction>();
            try {
                var repository = new AzureFile();
                var allTransactions = new ConcurrentDictionary<string, Transaction>();
                var transactionDB = dc.Transaction.ToList();
                var total = transactions.Count() + transactionDB.Count();
                var count = 1;

                transactionDB.ForEach(t => {
                    var add = allTransactions.TryAdd(t.Hash, t);
                    if (add == false) {
                        duplication.Add(t);
                    }
                    SendImportProgress(count++, total, "Unifying transactions", _hubContext);
                });

                transactions.ForEach(t => {
                    var add = allTransactions.TryAdd(t.Hash, t);
                    if (add == false) {
                        duplication.Add(t);
                    }
                    SendImportProgress(count++, total, "Unifying transactions", _hubContext);
                });

                var transactionsTreated = allTransactions.Select(t => t.Value).ToList();

                var urlDuplicates = repository.SaveCSV(duplication.ConvertAll(d => new TransactionViewModel(d)));
                var import = new Import() {
                    Date = DateTime.Now,
                    FileImported = urlFileImported,
                    FileDuplicate = urlDuplicates,
                    TotalTransactions = transactions.Count(),
                    TotalTransactionsDuplicates = duplication.Count(),
                    TotalTransactionsSaves = transactions.Count() - duplication.Count()
                };

                dc.BulkInsertOrUpdate(transactionsTreated, progress: delegate (decimal s) { sendProgress(s, "Save transactions", _hubContext); });
                dc.Add(import);
                dc.SaveChanges();

                _hubContext.Clients.All.Sucess(new ImportViewModel(import));
            } catch (Exception ex) {
                _hubContext.Clients.All.Error("Error unity transactions");
                throw ex;
            }
        }


        /// <summary>
        /// Read file OFX
        /// </summary>
        /// <param name="file"></param>
        /// <param name="_hubContext"></param>
        /// <param name="urlPath"></param>
        /// <returns>list of transactions</returns>
        public List<Transaction> ReadTransactions(IFormFile file, IHubContext<NotifyHub, ITypedHubClient> _hubContext, out string urlPath) {
            var transactions = new List<Transaction>();
            _hubContext.Clients.All.UpdatePercent("Reading OFX file");
            try {
                var format = "yyyyMMddHHmmss";
                CultureInfo provider = CultureInfo.InvariantCulture;
                var repository = new AzureFile();
                urlPath = repository.SaveFile(file, "importedofx", new List<string> { ".OFX" });
                if (string.IsNullOrWhiteSpace(urlPath)) {
                    return transactions;
                }

                using (Stream stream = file.OpenReadStream()) {

                    using (StreamReader read = new StreamReader(stream)) {
                        var lines = read.ReadToEnd().Split(new char[] { '\n' });
                        var total = lines.Count();
                        stream.Position = 0;
                        var line = "";
                        int count = 1;
                        while ((line = read.ReadLine()) != null) {
                            if (line.Contains("<TRNTYPE>")) {
                                var TRNTYPE = line.Split("<TRNTYPE>")[1].Trim();
                                SendImportProgress(count++, total, "Reading OFX file", _hubContext);
                                var DTPOSTED = read.ReadLine().Split("<DTPOSTED>")[1].Split('[')[0].Trim();
                                SendImportProgress(count++, total, "Reading OFX file", _hubContext);
                                var TRNAMT = read.ReadLine().Split("<TRNAMT>")[1].Trim();
                                SendImportProgress(count++, total, "Reading OFX file", _hubContext);
                                var MEMO = read.ReadLine().Split("<MEMO>")[1].Trim();
                                SendImportProgress(count++, total, "Reading OFX file", _hubContext);

                                var transaction = new Transaction {
                                    Date = DateTime.ParseExact(DTPOSTED, format, provider),
                                    Memo = MEMO,
                                    Type = TRNTYPE,
                                    Value = Convert.ToDecimal(TRNAMT.Replace('.', ',')),
                                };
                                transaction.Hash = GetHashTransactionImport(transaction);
                                transactions.Add(transaction);
                            } else {
                                SendImportProgress(count++, total, "Reading OFX file", _hubContext);
                            }
                        }
                        read.Close();
                    }
                    stream.Close();
                }
            } catch (Exception ex) {
                _hubContext.Clients.All.Error("Error ao ler arquivo");
                throw ex;
            }
            return transactions;
        }

        /// <summary>
        /// Create hash for transaction
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>string of hash</returns>
        private static string GetHashTransactionImport(Transaction transaction) {
            UnicodeEncoding ue = new UnicodeEncoding();
            SHA1Managed shHash = new SHA1Managed();
            var transactionString = "";
            transactionString += transaction.Date.ToString();
            transactionString += transaction.Memo.Replace(" ", string.Empty);
            transactionString += transaction.Type.Replace(" ", string.Empty);
            transactionString += transaction.Value.ToString();
            byte[] messageBytes = ue.GetBytes(transactionString);
            byte[] hashValue = shHash.ComputeHash(messageBytes);
            return BitConverter.ToString(hashValue);
        }

        /// <summary>
        /// send progress to frontend
        /// </summary>
        /// <param name="count"></param>
        /// <param name="total"></param>
        /// <param name="msg"></param>
        /// <param name="hubContext"></param>
        public static void SendImportProgress(int count, int total, string msg, IHubContext<NotifyHub, ITypedHubClient> hubContext) {
            if (hubContext != null) {
                var percentage = String.Format("{0:0}", (double)count / total * 100);
                hubContext.Clients.All.UpdatePercent($"{msg} {percentage}%");
            }
        }

        /// <summary>
        /// send progress to frontend BulkInsertOrUpdate
        /// </summary>
        /// <param name="count"></param>
        /// <param name="msg"></param>
        /// <param name="hubContext"></param>
        private void sendProgress(decimal count, string msg, IHubContext<NotifyHub, ITypedHubClient> hubContext) {
            count *= 100;
            hubContext.Clients.All.UpdatePercent($"{msg} {(int)count}%");
        }

        /// <summary>
        /// Gerenate new context
        /// </summary>
        /// <returns>context</returns>
        private ApplicationDbContext getNewDataContext() {
            var dbOptionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_configuration.GetConnectionString("DefaultConnectionString"));
            return new ApplicationDbContext(dbOptionBuilder.Options);
        }
    }
}
