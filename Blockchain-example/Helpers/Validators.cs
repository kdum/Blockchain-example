using Blockchain_example.Models;
using System.Linq;

namespace Blockchain_example.Helpers
{
    public static class Validators
    {
        private static readonly int Complexity = 4;

        public static bool IsProofValid(string hash)
        {
            return hash.StartsWith(new string('0', Complexity));
        }

        public static bool IsChainValid(Blockchain blockchain)
        {
            var previousBlock = blockchain.Blocks.FirstOrDefault();
            var currentIndex = 1;

            while (currentIndex < blockchain.Blocks.Count)
            {
                var block = blockchain.Blocks[currentIndex];

                if (block.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }

                var hash = HashCalculator.CalculateHash(block.PreviousHash, block.Timestamp, block.Proof);
                if (!IsProofValid(hash))
                {
                    return false;
                }

                previousBlock = block;
                currentIndex++;
            }

            return true;
        }
    }
}
