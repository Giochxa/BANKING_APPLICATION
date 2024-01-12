using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BANKING_APPLICATION
{
    public class Transaction
    {
        public DateTime transactionDate { get; set; }
        public string transactionType { get; set; }
        public decimal amount { get; set; }
        public decimal amountGEL { get; set; }
        public decimal amountUSD { get; set; }
        public decimal amountEUR { get; set; }
    }
}
