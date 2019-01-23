using SimpleGrind.Loadtest;
using SimpleGrind.Parameters;
using System;
using System.Linq;
using SimpleGrind.Net;
using System.Net;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleGrind.Runner
{
    public static class Program
    {
        static void Help(RequestParameters reqParams, RunnerParameters runParams)
        {
            Console.Write(
            "Measure webrequests in sync, async or parallel\r\n\r\n" +
            "SIMPLEGRIND method url [-j json] [-b behavior] [-n numberOfRuns]\r\n" +
            "                       [-i increaseBy] [-t timeout] [-w wait]\r\n" +
            "                       [-?]\r\n" +
            "\r\n" +
           $"  method               Method for request. [get|post|put|delete]. Current is {reqParams.Method}\r\n" +
           $"  url                  Url used for request. Current is {reqParams.Url}\r\n" +
            "  -h headers           Headers included in request.\r\n" +
            "                         Format \"header1=value1;header2=value2\"\r\n" +
            "  -c cookies           Cookies included in request.\r\n" +
            "                         Format \"cookie1=value1;cookie2=value2\"\r\n" +
            "  -j json              Json used by action put and post\r\n" +
           $"  -b behavior          Request behavior [sync|async|parallel]. Current is {runParams.Behavior}\r\n" +
           $"  -nr numberOfRuns      Number of runs before quitting. Current is {runParams.NumberOfRuns}\r\n" +
           $"  -nc numberOfCalls     Number of calls on each run. Current is {runParams.NumberOfCalls}\r\n" +
           $"  -i increaseByCalls   Increase number of calls between runs. Current is {runParams.IncreaseBy}\r\n" +
           $"  -w wait              Wait between requests in milliseconds. Current is {runParams.Wait}\r\n" +
           $"  -t timeout           Timeout for each request in seconds. Current is {reqParams.TimeOut}\r\n" +
           $"  -cl connectionLimit  Connection limit. Current is {runParams.ConnectionLimit}\r\n" +
           $"  -wu dateTime         Wait for datetime (yyyyMMdd hhmmss).\r\n" +
            "  -?                   Show this help\r\n");
        }
        public static void Main(string[] args)
        {
            var parameterBuilder = new ParameterBuilder(args, '-');
            var requestParams = new RequestParameters(parameterBuilder);
            var runnerParams = new RunnerParameters(parameterBuilder);

            if (args.Length < 2 || args.Any(a => a == "-?"))
            {
                Help(requestParams, runnerParams);
                return;
            }
        
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<ParameterBuilder>();
            serviceCollection.AddTransient<IGridWriter>(s => new GridConsole(Console.Out, 16,6));
            serviceCollection.AddTransient<IMonitor, Monitor>();
            serviceCollection.AddTransient<ILoadTestFactory,LoadTestFactory>();
            serviceCollection.AddSingleton<IRequestParameters>(_ => requestParams);
            serviceCollection.AddSingleton<IRunnerParameters>(_ => runnerParams);
            serviceCollection.AddSingleton<ISimpleWebClient,SimpleWebClient>();

            var serviceProvider = serviceCollection.BuildServiceProvider();
           
            ServicePointManager.DefaultConnectionLimit = runnerParams.ConnectionLimit;
            var monitor = serviceProvider.GetService<IMonitor>();
            monitor.Start();
        }
    }     
}