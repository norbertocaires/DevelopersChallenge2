using Nibo.Models.ViewModels;
using System;

namespace Nibo.Models {
    public class Import {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Date { get; set; }
        public string FileImported { get; set; }
        public string FileDuplicate { get; set; }
        public int TotalTransactions { get; set; }
        public int TotalTransactionsDuplicates { get; set; }
        public int TotalTransactionsSaves { get; set; }

        public Import() {
        }
    }
}