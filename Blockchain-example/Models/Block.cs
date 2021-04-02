using System;
using System.Collections.Generic;

namespace Blockchain_example.Models
{
    public class Block
    {
        public long Index { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Hash { get; set; }
        public string PreviousHash { get; set; }
        public long Proof { get; set; }
        public IList<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
