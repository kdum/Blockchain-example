using Blockchain_example.Helpers;
using Blockchain_example.Models;
using Bogus;
using System;
using System.Linq;

namespace Blockchain_example
{
    public static class FakeDataGenerator
    {
        private static int BlockIndex { get; set; } = 0;
        private static string Hash { get; set; } = string.Empty;
        private static long Proof { get; set; } = 0;
        private static DateTime Timestamp { get; set; } = new DateTime(2020, 01, 01, 10, 00, 00);

        public static Blockchain GetRandomizedBlockchain()
        {
            ResetData();
            var random = new Random();

            if (random.Next(1, 3) % 2 == 0)
            {
                return GenerateValidBlockchain();
            }

            return GenerateInvalidBlockchain();
        }

        private static Blockchain GenerateValidBlockchain()
        {
            Console.WriteLine("Generating valid blockchain.");
            return FakeValidBlockchain().Generate();
        }

        private static Blockchain GenerateInvalidBlockchain()
        {
            Console.WriteLine("Generating invalid blockchain.");
            return FakeInvalidBlockchain().Generate();
        }

        private static Faker<Transaction> FakeTransaction => new Faker<Transaction>()
            .RuleFor(x => x.From, f => f.Random.AlphaNumeric(10))
            .RuleFor(x => x.To, f => f.Random.AlphaNumeric(10))
            .RuleFor(x => x.Amount, f => f.Random.Number(10, 1000));

        private static Faker<Block> FakeInvalidBlock => new Faker<Block>()
            .RuleFor(x => x.Timestamp, f => Timestamp.AddDays(BlockIndex))
            .RuleFor(x => x.PreviousHash, f => BlockIndex == 0 ? string.Empty : "0000" + f.Random.AlphaNumeric(60))
            .RuleFor(x => x.Hash, f => "0000" + f.Random.AlphaNumeric(60))
            .RuleFor(x => x.Proof, f => f.Random.Long(0, 10000))
            .RuleFor(x => x.Index, f => BlockIndex++)
            .RuleFor(x => x.Transactions, f => FakeTransaction.Generate(f.Random.Number(0, 3)));

        private static Faker<Blockchain> FakeInvalidBlockchain() => new Faker<Blockchain>()
            // .RuleFor(x => x.Nodes, f => Enumerable.Range(0, f.Random.Int(0, 4)).Select(node => "127.0.0.10" + f.Random.Number(1, 9)).ToHashSet())
            .RuleFor(x => x.Blocks, f => FakeInvalidBlock.Generate(f.Random.Number(2, 5)));

        private static Faker<Blockchain> FakeValidBlockchain() => new Faker<Blockchain>()
            // .RuleFor(x => x.Nodes, f => Enumerable.Range(0, f.Random.Int(0, 4)).Select(node => "127.0.0.10" + f.Random.Number(1, 9)).ToHashSet())
            .RuleFor(x => x.Blocks, f => Enumerable.Range(2, f.Random.Int(2, 4)).Select(block => FakeValidBlock()).ToList());

        private static Faker<Block> FakeValidBlockBase => new Faker<Block>()
            .RuleFor(x => x.Timestamp, f => Timestamp.AddDays(BlockIndex))
            .RuleFor(x => x.PreviousHash, f => BlockIndex == 0 ? string.Empty : Hash)
            .RuleFor(x => x.Hash, string.Empty)
            .RuleFor(x => x.Proof, 0)
            .RuleFor(x => x.Index, f => BlockIndex++)
            .RuleFor(x => x.Transactions, f => FakeTransaction.Generate(f.Random.Number(0, 3)));

        private static Block FakeValidBlock()
        {
            var block = FakeValidBlockBase.Generate();
            (string hash, long proof) = GetHashAndProof(block.Timestamp);

            return new Block
            {
                Index = block.Index,
                Timestamp = block.Timestamp,
                Transactions = block.Transactions,
                PreviousHash = block.PreviousHash,
                Hash = hash,
                Proof = proof
            };
        }

        private static (string hash, long proof) GetHashAndProof(DateTime timestamp)
        {
            long proof = -1;
            var hash = string.Empty;

            do
            {
                proof++;
                hash = HashCalculator.CalculateHash(Hash, timestamp, proof);
            }
            while (!Validators.IsProofValid(hash));

            Hash = hash;
            Proof = proof;
            return (hash, proof);
        }
        
        private static void ResetData()
        {
            BlockIndex = 0;
            Hash = string.Empty;
            Proof = 0;
            Timestamp = new DateTime(2020, 01, 01, 10, 00, 00);
        }
    }
}
