using System;

namespace Blockchain_example
{
    class Program
    {
        static void Main(string[] args)
        {
            DependencyManager.Fill(new TransactionPool(), new BlockMiner(), new EmbedServer("5656"));
            DependencyManager.BlockMiner.StartMining();
            DependencyManager.EmbedServer.StartServer();
            Console.ReadKey();
            DependencyManager.BlockMiner.StopMining();
            DependencyManager.EmbedServer.StopServer();
        }
    }
}
