using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleGrind.Loadtest
{
	public class AsyncLoadTest : ILoadTest
	{
		readonly Func<Task<HttpResponseMessage>> _action;
		LoadResult _result = new LoadResult();
		
		public AsyncLoadTest(Func<Task<HttpResponseMessage>> action)
		{
			_action = action;
		}

		public LoadResult Run(int numberOfCalls, int wait)
		{
			_result = new LoadResult();
			RunAsync(numberOfCalls, wait)
                .Wait();
			return _result;
		}
		
		async Task RunAsync(int numberOfCalls, int wait)
		{
			var tasks = new Task[numberOfCalls];
			for (var index = 0; index < numberOfCalls; index++)
			{
				tasks[index] = RunAsync();
				if (wait > 0)
					Thread.Sleep(wait);
			}
			await Task.WhenAll(tasks);
		}

		async Task RunAsync()
		{
			try
			{
				var response = await _action();
				var ok = (((int) response.StatusCode) < 400);
				if (ok)
					_result.Ok++;
				else
					_result.Failed++;
			}
			catch(TaskCanceledException)
			{
				_result.Failed++;
			}
		}
	}
}