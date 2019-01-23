using System;
using SimpleGrind.Net;

namespace SimpleGrind.Loadtest
{
    public interface ILoadTestFactory
    {
        ILoadTest Create(string behavior, string method, string url, string json);
    }

    public class LoadTestFactory : ILoadTestFactory
    {
        private ISimpleWebClient _webClient;

        public LoadTestFactory(ISimpleWebClient simpleWebClient)
        {
            _webClient = simpleWebClient;
        }
        public ILoadTest Create(string behavior, string method, string url, string json)
        {
            if (behavior == "sync")
            {
                switch (method)
                {
                    case "get":
                        return new SyncLoadTest(() => _webClient.Get(url));
                    case "post":
                        return new SyncLoadTest(() => _webClient.PostJson(url, json));
                    case "put":
                        return new SyncLoadTest(() => _webClient.PutJson(url, json));
                    case "delete":
                        return new SyncLoadTest(() => _webClient.Delete(url));
                }
            }

            if (behavior == "parallel")
            {
                switch (method)
                {
                    case "get":
                        return new ParallellLoadTest(() => _webClient.Get(url));
                    case "post":
                        return new ParallellLoadTest(() => _webClient.PostJson(url, json));
                    case "put":
                        return new ParallellLoadTest(() => _webClient.PutJson(url, json));
                    case "delete":
                        return new SyncLoadTest(() => _webClient.Delete(url));
                }
            }
            if (behavior == "async")
            {
                switch (method)
                {
                    case "get":return new AsyncLoadTest(() => _webClient.GetAsync(url));
                    case "post":
                        return new AsyncLoadTest(() => _webClient.PostJsonAsync(url, json));
                    case "put":
                        return new AsyncLoadTest(() => _webClient.PutJsonAsync(url, json));
                    case "delete":
                        return new AsyncLoadTest(() => _webClient.DeleteAsync(url));
                }
            }
            throw new NotImplementedException($"No loadtest implemented for a combination of {behavior} and {method}");
        }
    }
}