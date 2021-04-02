using System.Collections.Generic;
using System.Linq;

namespace Blockchain_example.Models
{
    public class Blockchain
    {
        public IList<Block> Blocks { get; set; } = new List<Block>();
        public HashSet<string> Nodes = new HashSet<string>();

        public void AddNodes(List<string> nodes)
        {
            foreach (var node in nodes)
            {
                Nodes.Add(node);
            }
        }

        public Block LatestBlock()
        {
            return Blocks.LastOrDefault();
        }
    }
}