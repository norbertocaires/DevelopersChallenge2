using CsvHelper.Configuration;

namespace Nibo.Models.ViewModels
{
	public class TransactionViewModel {

        public string Type { get; set; }
        public string Data { get; set; }
        public decimal Value { get; set; }
        public string Memo { get; set; }
        public string Hash { get; set; }

        public TransactionViewModel() { }

    }

    sealed class TransactionDefinitionMap : ClassMap<TransactionViewModel> {
        public TransactionDefinitionMap() {
            Map(e => e.Type).Name("Type");
            Map(e => e.Data).Name("Data");
            Map(e => e.Value).Name("Value");
            Map(e => e.Memo).Name("Memo");
            Map(e => e.Hash).Name("Hash");
        }
    }
}
