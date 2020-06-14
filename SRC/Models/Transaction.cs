using Nibo.Models.ViewModels;
using System;

namespace Nibo.Models {
    public class Transaction {
        public string Hash { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }
        public decimal Value { get; set; }
        public string Memo { get; set; }
        public Transaction() {
        }
    }
}