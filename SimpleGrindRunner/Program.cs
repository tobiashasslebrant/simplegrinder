using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Net;
using SimpleGrind.Loadtest;
using SimpleGrind.Net;

namespace SimpleGrindRunner
{
	public class Program
	{
		const string DefaultBehavior = "async";
		const int DefaultNumberOfRuns = 10;
		const int DefaultIncreaseBy = 5;
		const int DefaultWait = 0;
		const int DefaultTimeOut = 5;
		private const int DefaultConnectionLimit = 1000;

		static void Help()
		{
			Console.Write(
			"Measure webrequests in parallel or async\r\n\r\n" +
			"SIMPLEGRIND method url [-j json] [-b behavior] [-n numberOfRuns]\r\n" +
			"                       [-i increaseBy] [-t timeout] [-w wait]\r\n" +
			"                       [-?]\r\n" +
			"\r\n" +
			"  method               Method for request. [get|post|put]\r\n" +
			"  url                  Url used for request\r\n" +
			"  -h headers           Headers included in request.\r\n" +
			"                         Format \"header1=value1;header2=value2\"\r\n" +
			"  -c cookies           Cookies included in request.\r\n" +
			"                         Format \"cookie1=value1;cookie2=value2\"\r\n" +
			"  -j json              Json used by action put and post\r\n" +
			"  -b behavior          Request behavior [async|parallel]. Default is {0}\r\n" +
			"  -n numberOfRuns      Number of runs before quitting. Default is {1}\r\n" +
			"  -i increaseByCalls   Increase number of requests between runs. Default is {2}\r\n" +
			"  -w wait              Wait between requests in milliseconds. Default is {3}\r\n" +
			"  -t timeout           Timeout for each request in seconds. Default is {4}\r\n" +
			"  -cl connectionLimit  Connection limit. Default is {5}\r\n" +
			"  -?                   Show this help\r\n",
			DefaultBehavior, DefaultNumberOfRuns, DefaultIncreaseBy, DefaultWait, DefaultTimeOut, DefaultConnectionLimit);
		}
			
		public static void Main(string[] args)
		{
			try
			{
				if (args.Length < 2 || args.Any(a => a == "-?"))
				{
					Help();
					return;
				}

				var ah = new ArgumentHelper(args.Skip(2).ToArray());
				var method = args[0];
				var url = args[1];
				var headers = ToDictionary(ah.GetArg("h", ""));
				var cookies = ToDictionary(ah.GetArg("c", ""));
				var json = ah.GetArg("j", "");
				var behavior = ah.GetArg("b", DefaultBehavior);
				var numberOfRuns = ah.GetArg("n", DefaultNumberOfRuns);
				var increaseBy = ah.GetArg("i", DefaultIncreaseBy);
				var wait = ah.GetArg("w", DefaultWait);
				var timeOut = ah.GetArg("t", DefaultTimeOut);
				var connectionLimit = ah.GetArg("cl", DefaultConnectionLimit);

				ServicePointManager.DefaultConnectionLimit = connectionLimit;

				var stopWatch = new Stopwatch();
			
				GridConsole.WriteLine("Starting {0} {1} runs with {2} against {3}.", numberOfRuns, behavior, method, url, increaseBy);
				GridConsole.WriteLine("Increase each run by {4} requests.", numberOfRuns, behavior, method, url, increaseBy);
				stopWatch.Start();
				Monitor.Start(Create(behavior, method, url, json, timeOut, headers, cookies), numberOfRuns, increaseBy, wait);
				stopWatch.Stop();
				GridConsole.WriteLine("Total run time is {0} seconds.", (stopWatch.ElapsedMilliseconds / 1000));
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			
		}

		static Dictionary<string, string> ToDictionary(string arg)
		{
			var dictionary = new Dictionary<string, string>();
			if (string.IsNullOrEmpty(arg)) return dictionary;
			foreach (var keyValuePair in arg.Split(';'))
			{
				var keyValue = keyValuePair.Split('=');
				dictionary.Add(keyValue[0],keyValue[1]);
			}
			return dictionary;
		} 

		static ILoadTest Create(string behavior, string method, string url, string json, int timeOut, Dictionary<string, string> defaultRequestHeaders, Dictionary<string, string> cookies)
		{
			var client = new SimpleWebClient(timeOut, defaultRequestHeaders, cookies);

			if (behavior == "parallel")
			{
				switch (method.ToLower())
				{
					case "get":
						return new ParallellLoadTest(() => client.Get(url));
					case "post":
						return new ParallellLoadTest(() =>  client.PostJson(url, json));
					case "put":
						return new ParallellLoadTest(() => client.PutJson(url, json));
				}
			}
			if (behavior == "async")
			{
				switch (method.ToLower())
				{
					case "get":
						return new AsyncLoadTest(async () => await client.GetAsync(url));
					case "post":
						return new AsyncLoadTest(async () => await client.PostJsonAsync(url, json));
					case "put":
						return new AsyncLoadTest(async () => await client.PutJsonAsync(url, json));
				}
			}
			throw new ArgumentException("Not a valid argument");
		}
	}
}