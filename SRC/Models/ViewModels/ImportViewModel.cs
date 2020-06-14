using CsvHelper.Configuration;
using System;

namespace Nibo.Models.ViewModels {
    public class ImportViewModel {
        public string Date { get; set; }
        public string FileImported { get; set; }
        public string FileDuplicate { get; set; }
        public int TotalTransactions { get; set; }
        public int TotalTransactionsDuplicates { get; set; }
        public int TotalTransactionsSaves { get; set; }
        public ImportViewModel() { }

        public ImportViewModel(Import import) {
            Date = import.Date.ToString("dd/MM/yyyy HH:mm:ss");
            FileImported = import.FileImported;
            FileDuplicate = import.FileDuplicate;
            TotalTransactions = import.TotalTransactions;
            TotalTransactionsDuplicates = import.TotalTransactionsDuplicates;
            TotalTransactionsSaves = import.TotalTransactionsSaves;
        }

    }
}
