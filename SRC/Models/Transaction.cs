namespace Nibo.Models {
    public class Transaction {
        public string Type { get; set; }
        public string Data { get; set; }
        public decimal Value { get; set; }
        public string Memo { get; set; }
        public string Hash { get; set; }

        public Transaction() {
        }
    }
}