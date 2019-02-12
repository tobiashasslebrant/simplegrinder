using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using SimpleGrind.Parameters;

namespace SimpleGrind.LoadTest
{
	public class AsyncLoadTest : ILoadTest
	{
		readonly Func<Task<HttpResponseMessage>> _action;
			
		public AsyncLoadTest(Func<Task<HttpResponseMessage>> action) 
			=> _action = action;

		public LoadResult Run(int numberOfCalls, int wait, LogLevel logLevel) 
			 => Task.Run(() => RunAsync(numberOfCalls, wait,logLevel)).Result;

		async Task<LoadResult> RunAsync(int numberOfCalls, int wait, LogLevel logLevel)
		{
			var tasks = new List<Task<HttpResponseMessage>>();
			for (var index = 0; index < numberOfCalls; index++)
			{
				tasks.Add(_action());
				if (wait > 0)
					Thread.Sleep(wait);
			}

			var errors = new List<string>();
			try
			{
				await Task.WhenAll(tasks);
			}
			catch (Exception ex)
			{
				errors.Add(ex.ToString());
			}

			var failed = tasks.Where(s => s.IsCompletedSuccessfully && !s.Result.IsSuccessStatusCode).ToArray();
			var successFull = tasks.Count(s => s.IsCompletedSuccessfully && s.Result.IsSuccessStatusCode);
			var timedOut = tasks.Count(s => s.IsCanceled);
			if (logLevel == LogLevel.Verbose && failed.Any())
				errors.AddRange(failed.Select(s => s.Result.Content.ReadAsStringAsync().Result));
			return new LoadResult
			{
				Ok =	successFull,
				Failed = numberOfCalls - successFull - timedOut,
				TimedOut = timedOut,
				Errors = errors
			};
		}
	}
}