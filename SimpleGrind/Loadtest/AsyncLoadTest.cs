using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleGrind.Loadtest
{
	public class AsyncLoadTest : ILoadTest
	{
		readonly Func<Task<HttpResponseMessage>> _action;
			
		public AsyncLoadTest(Func<Task<HttpResponseMessage>> action) 
			=> _action = action;

		public LoadResult Run(int numberOfCalls, int wait) 
			 => Task.Run(() => RunAsync(numberOfCalls, wait)).Result;

		async Task<LoadResult> RunAsync(int numberOfCalls, int wait)
		{
			var tasks = new List<Task<HttpResponseMessage>>();
			for (var index = 0; index < numberOfCalls; index++)
			{
				tasks.Add(_action());
				if (wait > 0)
					Thread.Sleep(wait);
			}

			try
			{
				await Task.WhenAll(tasks);
			}
			catch (Exception)
			{
			
			}
			return new LoadResult
			{
				Ok =	tasks.Count(w => w.IsCompletedSuccessfully),
				Failed = tasks.Count(w => !w.IsCompletedSuccessfully),
			};

		}
	}
}