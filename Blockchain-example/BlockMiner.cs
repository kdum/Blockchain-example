using Blockchain_example.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blockchain_example
{
    public class BlockMiner
    {
        private readonly int MINING_PERIOD = 10000; // 1min
        private TransactionPool TransactionPool { get => DependencyManager.TransactionPool; }
        public IList<Block> Blockchain { get; private set; } = new List<Block>();
        private CancellationTokenSource cancellationToken;

        public void StartMining()
        {
            cancellationToken = new CancellationTokenSource();
            Task.Run(() => this.DoGenerateBlock(cancellationToken.Token));
            Console.WriteLine("Started mining...");
        }

        public void StopMining()
        {
            cancellationToken.Cancel();
            Console.WriteLine("Stopped mining...");
        }

        private void DoGenerateBlock(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Console.WriteLine("Looping...");
                var startTime = DateTime.Now.Millisecond;
                this.GenerateBlock();
                var endTime = DateTime.Now.Millisecond;
                var remainingTime = MINING_PERIOD - (endTime - startTime);
                Thread.Sleep(remainingTime < 0 ? 0 : remainingTime);
            }

            Console.WriteLine("Done looping...");
        }

        private void GenerateBlock()
        {
            Console.WriteLine("GenerateBlock starting...");
            var lastBlock = Blockchain.LastOrDefault();
            var block = new Block
            {
                TimeStamp = DateTime.Now,
                Nonce = 0,
                Transactions = this.TransactionPool.TakeAllUnprocessedTransactions(),
                Index = lastBlock?.Index + 1 ?? 0,
                PreviousHash = lastBlock?.Hash ?? string.Empty,
            };

            MineBlock(block);
            Blockchain.Add(block);
            Console.WriteLine("GenerateBlock ending...");
        }

        private void MineBlock(Block block)
        {
            Console.WriteLine("Block mining starting...");
            var merkleRootHash = FindMerkleRootHash(block.Transactions);
            long nonce = -1;
            var hash = string.Empty;

            do
            {
                nonce++;
                var blockData = block.Index + block.PreviousHash + block.TimeStamp.ToString() + nonce + merkleRootHash;
                hash = CalculateHash(blockData);
            }
            while (!hash.StartsWith("0000"));

            block.Hash = hash;
            block.Nonce = nonce;
            Console.WriteLine("Block mining ending...");
        }

        private string FindMerkleRootHash(IList<Transaction> transactions)
        {
            var merkleLeaves = transactions.Select(t =>
            {
                var data = t.From + t.To + t.Amount;

                return this.CalculateHash(data);
            })
            .ToList();

            return this.BuildMerkleRootHash(merkleLeaves);
        }

        private string BuildMerkleRootHash(IList<string> merkleLeaves)
        {
            if (merkleLeaves == null || !merkleLeaves.Any())
                return string.Empty;

            if (merkleLeaves.Count() == 1)
                return merkleLeaves.First();

            if (merkleLeaves.Count() % 2 > 0)
                merkleLeaves.Add(merkleLeaves.Last());

            var merkleBranches = new List<string>();

            for (int i = 0; i < merkleLeaves.Count(); i += 2)
            {
                var leafPair = string.Concat(merkleLeaves[i], merkleLeaves[i + 1]);
                merkleBranches.Add(CalculateHash(leafPair));
            }

            return BuildMerkleRootHash(merkleBranches);
        }

        private string CalculateHash(string data)
        {
            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(data));
            StringBuilder builder = new StringBuilder();

            foreach(var _byte in bytes)
            {
                builder.Append(_byte.ToString("x2")); // x2 -> format in hexadecimal
            }

            return builder.ToString();
        }
    }
}
