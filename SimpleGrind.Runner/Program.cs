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
            "SIMPLEGRIND method url [Parameters]\r\n" +
           $"  method               Method for request. [get|post|put|delete]\r\n" +
           $"  url                  Url used for request\r\n" +
            "  -h headers           Headers included in request.\r\n" +
            "                         Format \"header1=value1;header2=value2\"\r\n" +
            "  -c cookies           Cookies included in request.\r\n" +
            "                         Format \"cookie1=value1;cookie2=value2\"\r\n" +
            "  -j json              Json used by action put and post\r\n" +
           $"  -b behavior          Request behavior [sync|async|parallel]. Default is {runParams.Behavior}\r\n" +
           $"  -w wait              Wait between requests in milliseconds. Default is {runParams.Wait}\r\n" +
           $"  -t timeout           Timeout for each request in seconds. Default is {reqParams.TimeOut}\r\n" +
           $"  -nr numberOfRuns     Number of runs before quitting. Default is {runParams.NumberOfRuns}\r\n" +
           $"  -nc numberOfCalls    Number of calls for first run. Default is {runParams.NumberOfCalls}\r\n" +
           $"  -ic increaseByCalls  Increase number of calls between runs. Default is {runParams.IncreaseBy}\r\n" +
           $"  -cl connectionLimit  Connection limit. Default is {runParams.ConnectionLimit}\r\n" +
           $"  -wu dateTime         Wait to start until datetime (yyyyMMdd hhmmss).\r\n" +
           $"  -ll loglevel         Loglevel can be FRIENDLY, VERBOSE or RESULT. Default is {runParams.LogLevel}\r\n" +
           $"                        FRIENDLY is reporting user friendly messages when running" +
           $"                        VERBOSE is reporting user friendly messages with detailed errors when running" +
           $"                        RESULT is only reporting the result grid. Useful when integrating with other tools" +
           $"  -li logItems         Number of log items to show. Default is {runParams.LogItems}\r\n" +
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
            serviceCollection.AddTransient<IGridWriter>(s => new GridConsole(Console.Out, 16,7,runnerParams.LogLevel));
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