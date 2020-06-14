using CsvHelper.Configuration;
using System;

namespace Nibo.Models.ViewModels
{
	public class TransactionViewModel {

        public string Type { get; set; }
        public string Date { get; set; }
        public decimal Value { get; set; }
        public string Memo { get; set; }
        public string Hash { get; set; }
        public TransactionViewModel() { }

        public TransactionViewModel(Transaction transaction) {
            Type = transaction.Type;
            Date = transaction.Date.ToString("dd/MM/yyyy HH:mm:ss");
            Memo = transaction.Memo;
            Hash = transaction.Hash;
            Value = transaction.Value;
        }

    }

    sealed class TransactionDefinitionMap : ClassMap<TransactionViewModel> {
        public TransactionDefinitionMap() {
            Map(e => e.Type).Name("Type");
            Map(e => e.Date).Name("Date");
            Map(e => e.Value).Name("Value");
            Map(e => e.Memo).Name("Memo");
            Map(e => e.Hash).Name("Hash");
        }
    }
}
