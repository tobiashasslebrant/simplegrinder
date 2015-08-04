using System;
using System.Linq;
using System.Diagnostics;
using System.Net;
using SimpleGrind.Loadtest;
using SimpleGrind.Net;

namespace SimpleGrindRunner
{
	public class Program
	{
		static void Help()
		{
			Console.Write(
			"Measure webrequests in parallel or async\r\n\r\n" +
			"SIMPLEGRIND method url [/b behavior] [/n numberOfRuns] [/i increaseBy] [/j json] [/t seconds] [/w milliseconds] [/?]\r\n" +
			"	Parameters\r\n" +
			"		method			get|post|put\r\n" +
			"		url				Url used for request\r\n" +
			"		/n numberOfRuns	Number of runs before quitting. Default is 10\r\n" +
			"		/i increaseBy	Increase number of calls between runs. Default is 5\r\n" +
			"		/b behavior		async|parallel, default is async\r\n" +
			"		/j json			Json used by action put and post" + 
			"		/t seconds		Timeout for each request in seconds. Default is 5" +
			"		/w milliseconds	Time to wait between requests. Default is 0" +
			"		/?				Show this help");

		}

		public static void Main(string[] args)
		{
			if (args.Length < 2 || args.Any(a => a == "/?"))
			{
				Help();
				return;
			}
		
			var url = args[1];
			var method = args[0];

			var ah = new ArgumentHelper(args.Skip(2).ToArray());

			var behavior = ah.GetArg<string>("/b","async");
			var numberOfRuns = ah.GetArg<int>("/n", 10);
			var increaseBy = ah.GetArg<int>("/i", 5);
			var json = ah.GetArg<string>("/j", "");
			var timeOut = ah.GetArg<int>("/t", 5);
			var wait = ah.GetArg<int>("/w", 0);
			
			ServicePointManager.DefaultConnectionLimit = 1000;
			
			var stopWatch = new Stopwatch();

			stopWatch.Start();
			Monitor.Start(Create(behavior, method, url, json, timeOut), numberOfRuns, increaseBy, wait);
			stopWatch.Stop();

			Console.WriteLine("Total run time is {0} seconds", stopWatch.ElapsedMilliseconds/1000);
		}

		static ILoadTest Create(string behavior, string method,string url, string json, int timeOut)
		{
			var client = new SimpleWebClient(timeOut);

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
						return new AsyncLoadTest(() => client.GetAsync(url));
					case "post":
						return new AsyncLoadTest(() => client.PostJsonAsync(url, json));
					case "put":
						return new AsyncLoadTest(() => client.PutJsonAsync(url, json));
				}
			}
			throw new ArgumentException("Not a valid argument");
		}
	}
}