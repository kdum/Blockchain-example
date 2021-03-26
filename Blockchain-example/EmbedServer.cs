using Blockchain_example.Models;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using Newtonsoft.Json;
using System;
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
            public string GetAllBlocks() => JsonConvert.SerializeObject(
                DependencyManager.BlockMiner.Blockchain,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                });

            [Route(HttpVerbs.Get, "/blocks/index/{index?}")]
            public string GetAllBlocks(int index)
            {
                Block block = null;
                if (index < DependencyManager.BlockMiner.Blockchain.Count)
                    block = DependencyManager.BlockMiner.Blockchain[index];
                return JsonConvert.SerializeObject(
                    block,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.Indented
                    });
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
            public string GetLatestBlocks()
            {
                var block = DependencyManager.BlockMiner.Blockchain.LastOrDefault();
                return JsonConvert.SerializeObject(
                    block,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        Formatting = Formatting.Indented
                    });
            }

            [Route(HttpVerbs.Post, "/add")]
            public void AddTransaction()
            {
                var data = HttpContext.GetRequestDataAsync<Transaction>();
                if (data != null && data.Result != null)
                    DependencyManager.TransactionPool.AddTransaction(data.Result);
            }
        }
    }
}
