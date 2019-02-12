using SimpleGrind.LoadTest;
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
            "\r\ndescription: Measure webrequests in sync, async or parallel\r\n" +
            "\r\n" +
            "usage: method url [Parameters...]\r\n" +
            "\r\n" +
            "parameters:\r\n" +
           $"  method               Method for request. [get|post|put|delete]\r\n" +
           $"  url                  Url used for request\r\n" +
            "  -h headers           Headers included in request.\r\n" +
            "                         Format \"header1=value1;header2=value2\"\r\n" +
            "  -c cookies           Cookies included in request.\r\n" +
            "                         Format \"cookie1=value1;cookie2=value2\"\r\n" +
            "  -j json              Json used by action put and post\r\n" +
           $"  -b behavior          Request behavior [sync|async|parallel]. Default is {runParams.Behavior}\r\n" +
           $"  -w wait              Wait between requests in milliseconds. Default is {runParams.Wait}s\r\n" +
           $"  -t timeout           Timeout for each request in seconds. Default is {reqParams.TimeOut}\r\n" +
           $"  -nr numberOfRuns     Number of runs before quitting. Default is {runParams.NumberOfRuns}\r\n" +
           $"  -nc numberOfCalls    Number of calls for first run. Default is {runParams.NumberOfCalls}\r\n" +
           $"  -ic increaseByCalls  Increase number of calls between runs. Default is {runParams.IncreaseBy}\r\n" +
           $"  -cl connectionLimit  Connection limit. Default is {runParams.ConnectionLimit}\r\n" +
           $"  -wu dateTime         Wait to start until datetime (yyyyMMdd hhmmss).\r\n" +
           $"  -ll loglevel         Loglevel can be FRIENDLY, VERBOSE, REPORT or SUMMARY. Default is {runParams.LogLevel}\r\n" +
           $"                        FRIENDLY is reporting friendly messages with a result grid and summary\r\n" +
           $"                        VERBOSE is FRIENDLY but with detailed errors\r\n" +
           $"                        RESULT is only reporting the result grid. Useful when integrating with other tools\r\n" +
           $"                        SUMMARY is only reporting the summary. Useful when integrating with other tools\r\n" +
           $"  -li items            Number of log error items to show. Default is {runParams.LogItems}\r\n" +
           $"  -ec condition        Exit condition. Will exit 1 when fullfilled\r\n" +
           $"                        Syntax per run: [ok|failed|timedout|time|avg|totaltime|totalavg][%|#|=|<|>|!][value|percentage];[...]\r\n" +
           $"                         Fields ok, failed, timedout, time and avg will test against each run" +
           $"                         Fields totaltime and totalavg will compare the aggregated result for all runs" +
           $"                         A semicolon (;) will seperate multiple conditions. Each condition will be applies with OR." +
           $"                         When a % is used, comparing is doing by percentage against relevant field. " +
           $"                          Percentage must be greater than value. (# is the same as % but will compare with" +
           $"                          lower than value instead." +
           $"                         All time comparisions will be with milliseconds" +
           $"                         Example: failed%80 => any run, with failed compared to number of calls > 80 percent\r\n" +
           $"                         Example: ok#80 => any run, with ok compared to number of calls < 80 percent\r\n" +
           $"                         Example: totaltime>1000 => total time larger than 1000ms\r\n" +
            $"                        Example: failed!0 => any runs with failed not equal 0\r\n" +
           $"                       Show this help\r\n");
        }
        public static int Main(string[] args)
        {
            var parameterBuilder = new ParameterBuilder(args, '-');
            var requestParams = new RequestParameters(parameterBuilder);
            var runnerParams = new RunnerParameters(parameterBuilder);

            if (args.Length < 2 || args.Any(a => a == "-?"))
            {
                Help(requestParams, runnerParams);
                return 0;
            }
            
            
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient<ParameterBuilder>();
            serviceCollection.AddTransient<LoadRunner>();
            serviceCollection.AddTransient<ConditionHandler>();
            serviceCollection.AddTransient<IGridWriter>(s => new GridConsole(Console.Out, 16,7));
            serviceCollection.AddTransient<IMonitor, Monitor>();
            serviceCollection.AddTransient<ILoadTestFactory,LoadTestFactory>();
            serviceCollection.AddSingleton<IRequestParameters>(_ => requestParams);
            serviceCollection.AddSingleton<IRunnerParameters>(_ => runnerParams);
            serviceCollection.AddSingleton<ISimpleWebClient,SimpleWebClient>();
            
            var serviceProvider = serviceCollection.BuildServiceProvider();
           
            ServicePointManager.DefaultConnectionLimit = runnerParams.ConnectionLimit;
            var monitor = serviceProvider.GetService<IMonitor>();
            
            return monitor.Start();
        }
    }     
}