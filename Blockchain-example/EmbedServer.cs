using Blockchain_example.Models;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blockchain_example
{
    public class EmbedServer
    {
        private readonly WebServer Server;
        private readonly string Url;

        public EmbedServer(string port)
        {
            this.Url = $"http://localhost:{port}/";
            this.Server = CreateWebServer(this.Url);
        }
        
        public void StartServer()
        {
            this.Server.RunAsync();
            Console.WriteLine($"HTTP server available at {this.Url}...");
        }

        public void StopServer()
        {
            this.Server.Dispose();
            Console.WriteLine($"HTTP server stopped...");
        }

        private WebServer CreateWebServer(string url)
        {
            var server = new WebServer(wso =>
                wso.WithUrlPrefix(url)
                   .WithMode(HttpListenerMode.EmbedIO))
                   .WithLocalSessionManager()
                   .WithWebApi("/api", wam => wam.WithController<Controller>())
                   .WithModule(new ActionModule(
                       "/",
                       HttpVerbs.Any,
                       ctx =>
                            ctx.SendDataAsync(new { Message = "Error" })));

            return server;
        }

        public sealed class Controller : WebApiController
        {
            [Route(HttpVerbs.Get, "/blocks")]
            public IList<Block> GetAllBlocks() => DependencyManager.BlockMiner.Blockchain.Blocks;

        [Route(HttpVerbs.Get, "/blocks/index/{index?}")]
            public Block GetAllBlocks(int index)
            {
                Block block = null;
                if (index < DependencyManager.BlockMiner.Blockchain.Blocks.Count)
                    block = DependencyManager.BlockMiner.Blockchain.Blocks[index];
                return block;
            }

            [Route(HttpVerbs.Post, "/mine")]
            public void Mine() {
                DependencyManager.BlockMiner.StartMining();
            }

            [Route(HttpVerbs.Post, "/mine/stop")]
            public void StopMine()
            {
                DependencyManager.BlockMiner.StopMining();
            }

            [Route(HttpVerbs.Get, "/blocks/latest")]
            public Block GetLatestBlocks() => DependencyManager.BlockMiner.Blockchain.LatestBlock();

            [Route(HttpVerbs.Post, "/transactions/add")]
            public void AddTransaction()
            {
                var data = HttpContext.GetRequestDataAsync<Transaction>();
                if (data != null && data.Result != null)
                    DependencyManager.TransactionPool.AddTransaction(data.Result);
            }

            [Route(HttpVerbs.Post, "/blockchain/nodes/add")]
            public void AddNodeToBlockchain()
            {
                var data = HttpContext.GetRequestDataAsync<List<string>>();
                if (data != null && data.Result != null)
                    DependencyManager.BlockMiner.Blockchain.AddNodes(data.Result);
            }

            [Route(HttpVerbs.Get, "/blockchain/nodes")]
            public HashSet<string> GetBlockchainNodes() => DependencyManager.BlockMiner.Blockchain.Nodes;

            [Route(HttpVerbs.Get, "/blockchain/nodes/resolve")]
            public ResolveConflictsModel ResolveBlockchainNodesConflict() => DependencyManager.BlockMiner.ResolveConflicts();
        }
    }
}
