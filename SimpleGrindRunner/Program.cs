using SimpleGrind.Loadtest;
using SimpleGrind.Parameters;
using System;
using System.Linq;
using Ninject;
using SimpleGrind.Net;
using System.Net;

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
           $"  method               Method for request. [get|post|put]. Current is {reqParams.Method}\r\n" +
           $"  url                  Url used for request. Current is {reqParams.Url}\r\n" +
            "  -h headers           Headers included in request.\r\n" +
            "                         Format \"header1=value1;header2=value2\"\r\n" +
            "  -c cookies           Cookies included in request.\r\n" +
            "                         Format \"cookie1=value1;cookie2=value2\"\r\n" +
            "  -j json              Json used by action put and post\r\n" +
           $"  -b behavior          Request behavior [sync|async|parallel]. Current is {runParams.Behavior}\r\n" +
           $"  -n numberOfRuns      Number of runs before quitting. Current is {runParams.NumberOfRuns}\r\n" +
           $"  -i increaseByCalls   Increase number of requests between runs. Current is {runParams.IncreaseBy}\r\n" +
           $"  -w wait              Wait between requests in milliseconds. Current is {runParams.Wait}\r\n" +
           $"  -t timeout           Timeout for each request in seconds. Current is {reqParams.TimeOut}\r\n" +
           $"  -cl connectionLimit  Connection limit. Current is {runParams.ConnectionLimit}\r\n" +
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
        
            var kernel = new StandardKernel();
            kernel.Bind<IGridWriter>().To<GridConsole>()
                .WithConstructorArgument("writer", Console.Out)
                .WithConstructorArgument("columnWidth", 12)
                .WithConstructorArgument("noOfColumns", 6);
            kernel.Bind<IMonitor>().To<Monitor>();
            kernel.Bind<ILoadTestFactory>().To<LoadTestFactory>();
            kernel.Bind<IRequestParameters>().ToMethod(c => requestParams).InSingletonScope(); ;
            kernel.Bind<IRunnerParameters>().ToMethod(c => runnerParams).InSingletonScope();
            kernel.Bind<ISimpleWebClient>().To<SimpleWebClient>();

            ServicePointManager.DefaultConnectionLimit = runnerParams.ConnectionLimit;
            var monitor = kernel.Get<IMonitor>();
            monitor.Start();
        }
    }     
}