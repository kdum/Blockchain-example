using Blockchain_example.Models;

namespace Blockchain_example
{
    public static class DependencyManager
    {
        public static TransactionPool TransactionPool { get; private set; }

        public static Blockchain Blockchain { get; set; }
        public static BlockMiner BlockMiner { get; private set; }
        public static EmbedServer EmbedServer { get; private set; }

        public static void Fill(TransactionPool transactionPool, Blockchain blockchain, BlockMiner blockMiner, EmbedServer embedServer)
        {
            TransactionPool = transactionPool;
            Blockchain = blockchain;
            BlockMiner = blockMiner;
            EmbedServer = embedServer;
        }
    }
}
