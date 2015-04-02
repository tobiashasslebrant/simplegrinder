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

		public LoadResult Run(int numberOfCalls, Action<LoadResult> callback, int wait)
		{
			var tasks = new Task[numberOfCalls];
			var result = new LoadResult();

			for (var index = 0; index < numberOfCalls; index++)
			{
				var task = _action();
				tasks[index] = task;
				task.ContinueWith((t, o) =>
				{
					lock (_syncLock)
					{
						if (((int) t.Result.StatusCode) < 400)
							result.Ok++;
						else
							result.Failed++;
					
						callback(result);
					}
				}, null);
				if (wait > 0)
					System.Threading.Thread.Sleep(wait);
			}

			Task.WaitAll(tasks);
			return result;
		}
	}
}