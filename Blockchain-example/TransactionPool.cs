using Blockchain_example.Models;
using System.Collections.Generic;
using System.Linq;

namespace Blockchain_example
{
    public class TransactionPool
    {
        private IList<Transaction> Transactions = new List<Transaction>();
        private readonly object lockTransactions = new object();

        /// <summary>
        /// Add transcation into unprocessed transactions pool.
        /// </summary>
        public void AddTransaction(Transaction transaction)
        {
            lock (this.lockTransactions)
            {
                this.Transactions.Add(transaction);
            }
        }

        /// <summary>
        /// Create transaction object from given properties and add to unprocessed transactions pool.
        /// </summary>
        public void AddTransaction(string from, string to, int amount)
        {
            var transaction = new Transaction
            {
                To = to,
                From = from,
                Amount = amount
            };

            this.AddTransaction(transaction);
        }

        /// <summary>
        /// Take all unprocessed transactions and clear transactions pool.
        /// </summary>
        public IList<Transaction> TakeAllUnprocessedTransactions()
        {
            lock (this.lockTransactions)
            {
                var transactionsCopy = this.Transactions.ToList();
                this.Transactions.Clear();

                return transactionsCopy;
            }
        }

    }
}
