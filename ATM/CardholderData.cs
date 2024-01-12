using NLog;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading;

namespace BANKING_APPLICATION
{
    public class CardholderData
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public CardDetails cardDetails { get; set; }
        public string pinCode { get; set; }
        public Transaction[] transactionHistory { get; set; }
    }
}