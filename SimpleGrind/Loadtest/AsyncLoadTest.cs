using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SimpleGrind.Loadtest
{
	public class AsyncLoadTest : ILoadTest
	{
		readonly Func<Task<HttpResponseMessage>> _action;
		readonly object _syncLock = new object();

		public AsyncLoadTest(Func<Task<HttpResponseMessage>> action)
		{
			_action = action;
		}

		public LoadResult Run(int numberOfCalls, int wait, Action<LoadResult> callback)
		{
			var result = RunAsync(numberOfCalls,wait,callback);
			result.Wait();
			return result.Result;
		}

		public async Task<LoadResult> RunAsync(int numberOfCalls, int wait, Action<LoadResult> callback)
		{
			var result = new LoadResult();
			for (var index = 0; index < numberOfCalls; index++)
			{
				var response = await _action();
				lock (_syncLock)
				{
					if (((int)response.StatusCode) < 400)
						result.Ok++;
					else
						result.Failed++;

					callback(result);
				}
				if (wait > 0)
					System.Threading.Thread.Sleep(wait);
			}
			return result;
		}

	}
}