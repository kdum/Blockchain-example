namespace Blockchain_example
{
    public static class DependencyManager
    {
        public static TransactionPool TransactionPool { get; private set; }
        public static BlockMiner BlockMiner { get; private set; }
        public static EmbedServer EmbedServer { get; private set; }

        public static void Fill(TransactionPool transactionPool, BlockMiner blockMiner, EmbedServer embedServer)
        {
            TransactionPool = transactionPool;
            BlockMiner = blockMiner;
            EmbedServer = embedServer;
        }
    }
}
