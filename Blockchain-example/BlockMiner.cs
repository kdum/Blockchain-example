using Blockchain_example.Helpers;
using Blockchain_example.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blockchain_example
{
    public class BlockMiner
    {
        private TransactionPool TransactionPool
        {
            get => DependencyManager.TransactionPool;
        }
        public Blockchain Blockchain
        {
            get => DependencyManager.Blockchain;
            set => DependencyManager.Blockchain = value;
        }
        private CancellationTokenSource cancellationToken;

        public void StartMining()
        {
            cancellationToken = new CancellationTokenSource();
            Task.Run(() => this.GenerateBlock(), cancellationToken.Token);
            Console.WriteLine("Started mining...");
        }

        public void StopMining()
        {
            cancellationToken.Cancel();
            Console.WriteLine("Stopped mining...");
        }

        private void GenerateBlock()
        {
            Console.WriteLine("GenerateBlock starting...");
            var lastBlock = Blockchain.Blocks.LastOrDefault();
            var block = new Block
            {
                Timestamp = DateTime.Now,
                Proof = 0,
                Transactions = this.TransactionPool.TakeAllUnprocessedTransactions(),
                Index = lastBlock?.Index + 1 ?? 0,
                PreviousHash = lastBlock?.Hash ?? string.Empty,
            };

            MineBlock(block);
            Blockchain.Blocks.Add(block);
            Console.WriteLine("GenerateBlock ending...");
        }

        private void MineBlock(Block block)
        {
            Console.WriteLine("Block mining starting...");
            long proof = -1;
            var hash = string.Empty;

            do
            {
                proof++;
                hash = HashCalculator.CalculateHash(block.PreviousHash, block.Timestamp, proof);
            }
            while (!Validators.IsProofValid(hash));

            block.Hash = hash;
            block.Proof = proof;
            Console.WriteLine("Block mining ending...");
        }

        public ResolveConflictsModel ResolveConflicts()
        {
            var allBlockchains = new List<Blockchain>();
            Blockchain newChain = null;
            var maxLength = this.Blockchain.Blocks.Count;

            foreach (var node in this.Blockchain.Nodes)
            {
                // Generate blockchain with 50/50 chance of being valid or invalid.
                var nodeBlockchain = FakeDataGenerator.GetRandomizedBlockchain();
                allBlockchains.Add(nodeBlockchain);
                var nodeBlockchainLength = nodeBlockchain.Blocks.Count;

                if (nodeBlockchainLength > maxLength &&
                    Validators.IsChainValid(nodeBlockchain))
                {
                    maxLength = nodeBlockchainLength;
                    newChain = nodeBlockchain;
                }
            }

            if (newChain != null)
            {
                this.Blockchain = newChain;

                return new ResolveConflictsModel
                {
                    Blockchains = allBlockchains,
                    Resolved = true
                };
            }

            return new ResolveConflictsModel
            {
                Blockchains = allBlockchains,
                Resolved = false
            };
        }
    }
}
