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
        private readonly int Complexity = 4;
        private TransactionPool TransactionPool { get => DependencyManager.TransactionPool; }
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
                TimeStamp = DateTime.Now,
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
                hash = CalculateHash(block.PreviousHash, proof);
            }
            while (!IsProofValid(hash));

            block.Hash = hash;
            block.Proof = proof;
            Console.WriteLine("Block mining ending...");
        }

        private bool IsProofValid(string hash)
        {
            return hash.StartsWith(new string('0', Complexity));
        }

        public bool IsChainValid(Blockchain blockchain)
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

                var hash = CalculateHash(block.PreviousHash, block.Proof);
                if (!IsProofValid(hash))
                {
                    return false;
                }

                previousBlock = block;
                currentIndex++;
            }

            return true;
        }

        public bool ResolveConflicts()
        {
            Blockchain newChain = null;
            var maxLength = this.Blockchain.Blocks.Count;

            foreach (var node in this.Blockchain.Nodes)
            {
                var nodeBlockchain = new Blockchain
                {
                    Blocks = new List<Block>
                    {
                        new Block
                        {
                            Index = 0,
                            TimeStamp = new DateTime(2021, 4, 2, 14, 7, 2),
                            Hash = "0000a456e7b5a5eb059e721fb431436883143101275c4077f83fe70298f5623d",
                            PreviousHash = string.Empty,
                            Proof = 88484,
                            Transactions = new List<Transaction>()
                        },
                        new Block
                        {
                            Index = 1,
                            TimeStamp = new DateTime(2021, 4, 2, 14, 7, 5),
                            Hash = "0000c13b5d7c6b636942c8e62f5ab023bcce895b5907237e3f4ff548e138ccc3",
                            PreviousHash = "0000a456e7b5a5eb059e721fb431436883143101275c4077f83fe70298f5623d",
                            Proof = 158818,
                            Transactions = new List<Transaction>()
                        },
                        new Block
                        {
                            Index = 2,
                            TimeStamp = new DateTime(2021, 4, 2, 14, 7, 6),
                            Hash = "00009284241f5d3a42047c70b2655f06c3f48ba8739a13f4eb8eade7ae2d96bf",
                            PreviousHash = "0000c13b5d7c6b636942c8e62f5ab023bcce895b5907237e3f4ff548e138ccc3",
                            Proof = 10904,
                            Transactions = new List<Transaction>()
                        },
                        new Block
                        {
                            Index = 3,
                            TimeStamp = new DateTime(2021, 4, 2, 14, 7, 7),
                            Hash = "00002fe5cccea734e66e7194d778b3c82d0034baf5c88de3318e58a6af3add9c",
                            PreviousHash = "00009284241f5d3a42047c70b2655f06c3f48ba8739a13f4eb8eade7ae2d96bf",
                            Proof = 19496,
                            Transactions = new List<Transaction>()
                        },
                        new Block
                        {
                            Index = 4,
                            TimeStamp = new DateTime(2021, 4, 2, 14, 7, 9),
                            Hash = "0000b5bf5235d1741b8f73426c7cc27daee4fc7a71ee39f4d7a46fd33ccea51c",
                            PreviousHash = "00002fe5cccea734e66e7194d778b3c82d0034baf5c88de3318e58a6af3add9c",
                            Proof = 140382,
                            Transactions = new List<Transaction>()
                        },
                    },
                    Nodes = new HashSet<string> { "127.0.0.100", "127.0.0.101", "127.0.0.102" }
                }; // mocking blokchain for node.

                var nodeBlockchainLength = nodeBlockchain.Blocks.Count;
                
                if (nodeBlockchainLength > maxLength &&
                    IsChainValid(nodeBlockchain))
                {
                    maxLength = nodeBlockchainLength;
                    newChain = nodeBlockchain;
                }
            }

            if (newChain != null)
            {
                this.Blockchain = newChain;
                return true;
            }

            return false;
        }

        private string CalculateHash(string previousHash, long proof)
        {
            var blockData = previousHash + proof;

            using SHA256 sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(blockData));
            StringBuilder builder = new StringBuilder();

            foreach(var _byte in bytes)
            {
                builder.Append(_byte.ToString("x2")); // x2 -> format in hexadecimal
            }

            return builder.ToString();
        }
    }
}
