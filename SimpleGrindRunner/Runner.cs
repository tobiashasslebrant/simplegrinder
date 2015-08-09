using System;
using System.Diagnostics;
using System.Net;
using SimpleGrind.Loadtest;
using SimpleGrind.Net;
using SimpleGrind.Runner.Parameters;

namespace SimpleGrind.Runner
{
    public class Runner
    {
        private IGridWriter _gridWriter;
        private IMonitor _monitor;

        public Runner(IGridWriter gridWriter, IMonitor monitor)
        {
            _gridWriter = gridWriter;
            _monitor = monitor;
        }

        public void Run(RequestParameters requestParameters, RunnerParameters runnerParameters)
        {
            ServicePointManager.DefaultConnectionLimit = runnerParameters.ConnectionLimit;

            var stopWatch = new Stopwatch();
            _gridWriter.WriteLine($"Starting {runnerParameters.NumberOfRuns} runs with {requestParameters.Method} against {requestParameters.Url}.");
            _gridWriter.WriteLine($"Increase each run by {runnerParameters.IncreaseBy} requests.");
            stopWatch.Start();
            _monitor.Start(Create(requestParameters,runnerParameters.Behavior), runnerParameters);
            stopWatch.Stop();
            _gridWriter.WriteLine("Total run time is {0} seconds.", (stopWatch.ElapsedMilliseconds / 1000));
        }

        ILoadTest Create(RequestParameters requestParameters, string behavior)
        {
            var client = new SimpleWebClient(requestParameters.TimeOut, requestParameters.Headers, requestParameters.Cookies);

            if (behavior == "parallel")
            {
                switch (requestParameters.Method.ToLower())
                {
                    case "get":
                        return new ParallellLoadTest(() => client.Get(requestParameters.Url));
                    case "post":
                        return new ParallellLoadTest(() => client.PostJson(requestParameters.Url, requestParameters.Json));
                    case "put":
                        return new ParallellLoadTest(() => client.PutJson(requestParameters.Url, requestParameters.Json));
                }
            }
            if (behavior == "async")
            {
                switch (requestParameters.Method.ToLower())
                {
                    case "get":
                        return new AsyncLoadTest(() => client.GetAsync(requestParameters.Url));
                    case "post":
                        return new AsyncLoadTest(() => client.PostJsonAsync(requestParameters.Url, requestParameters.Json));
                    case "put":
                        return new AsyncLoadTest(() => client.PutJsonAsync(requestParameters.Url, requestParameters.Json));
                }
            }
            throw new ArgumentException("Not a valid argument");
        }

    }
}
