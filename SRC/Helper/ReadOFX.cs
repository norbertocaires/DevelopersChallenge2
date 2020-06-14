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

namespace Nibo.Helper {
    public class ReadOFX {
        public List<TransactionViewModel> GetTransactionsPage(int page, int rows) {
            try {
                var masterTransactions = GetTransactions();
                var toSkip = (page - 1) * rows;
                return masterTransactions.Skip(toSkip).Take(rows).ToList();
            } catch (Exception ex) {
                throw ex;
            }
        }

        public List<TransactionViewModel> GetTransactions() {
            try {
                var repository = new AzureFile();
                return repository.GetMasterOFXFile();
            } catch (Exception ex) {
                throw ex;
            }
        }

        public void UnifyTransactions(List<TransactionViewModel> transactions, IHubContext<NotifyHub, ITypedHubClient> _hubContext) {
            _hubContext.Clients.All.UpdatePercent("Unifying transactions");
            var erros = new List<TransactionViewModel>();
            try {
                var repository = new AzureFile();
                var allTransactions = new ConcurrentDictionary<string, TransactionViewModel>();
                var masterTransactions = repository.GetMasterOFXFile();
                var total = transactions.Count() + masterTransactions.Count();
                var count = 1;

                masterTransactions.ForEach(t => {
                    var add = allTransactions.TryAdd(t.Hash, t);
                    if (add == false) {
                        erros.Add(t);
                    }
                    SendImportProgress(count++, total, "Unifying transactions", _hubContext);
                });

                transactions.ForEach(t => {
                    var add = allTransactions.TryAdd(t.Hash, t);
                    if (add == false) {
                        erros.Add(t);
                    }
                    SendImportProgress(count++, total, "Unifying transactions", _hubContext);
                });

                var transactionsTreated = allTransactions.Select(t => t.Value).ToList();
                repository.SaveMasterOFXFile(transactionsTreated);

            } catch (Exception ex) {
                _hubContext.Clients.All.Error("Error ao unificando transações");
                throw ex;
            }
            _hubContext.Clients.All.Sucess("");
        }


        public List<TransactionViewModel> ReadTransactions(IFormFile file, IHubContext<NotifyHub, ITypedHubClient> _hubContext) {
            var transactions = new List<TransactionViewModel>();
            _hubContext.Clients.All.UpdatePercent("Reading OFX file");
            try {
                var repository = new AzureFile();
                var urlPath = repository.SaveFile(file, "importedofx", new List<string> { ".OFX" });
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
                                var DTPOSTED = read.ReadLine().Split("<DTPOSTED>")[1].Trim();
                                SendImportProgress(count++, total, "Reading OFX file", _hubContext);
                                var TRNAMT = read.ReadLine().Split("<TRNAMT>")[1].Trim();
                                SendImportProgress(count++, total, "Reading OFX file", _hubContext);
                                var MEMO = read.ReadLine().Split("<MEMO>")[1].Trim();
                                SendImportProgress(count++, total, "Reading OFX file", _hubContext);

                                var transaction = new TransactionViewModel {
                                    Data = DTPOSTED,
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

        private static string GetHashTransactionImport(TransactionViewModel transaction) {
            UnicodeEncoding ue = new UnicodeEncoding();
            SHA1Managed shHash = new SHA1Managed();
            var transactionString = "";
            transactionString += transaction.Data.Replace(" ", string.Empty);
            transactionString += transaction.Memo.Replace(" ", string.Empty);
            transactionString += transaction.Type.Replace(" ", string.Empty);
            transactionString += transaction.Value.ToString();
            byte[] messageBytes = ue.GetBytes(transactionString);
            byte[] hashValue = shHash.ComputeHash(messageBytes);
            return BitConverter.ToString(hashValue);
        }

        public static void SendImportProgress(int count, int total, string msg, IHubContext<NotifyHub, ITypedHubClient> hubContext) {
            if (hubContext != null) {
                var percentage = String.Format("{0:0}", (double)count / total * 100);
                hubContext.Clients.All.UpdatePercent($"{msg} {percentage}%");
            }
        }
    }
}
