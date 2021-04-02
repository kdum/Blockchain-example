using System.Collections.Generic;

namespace Blockchain_example.Models
{
    public class ResolveConflictsModel
    {
        public List<Blockchain> Blockchains { get; set; } = new List<Blockchain>();
        public bool Resolved { get; set; }
    }
}
