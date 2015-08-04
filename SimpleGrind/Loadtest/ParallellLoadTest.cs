using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleGrind.Loadtest
{
	public class ParallellLoadTest : ILoadTest
	{
		readonly Func<HttpResponseMessage> _action;
		readonly object _syncLock = new object();

		public ParallellLoadTest(Func<HttpResponseMessage> action)
		{
			_action = action;
		}
		public LoadResult Run(int numberOfCalls, int wait, Action<LoadResult> callback)
		{
			var result = new LoadResult();
			var res = Parallel.For(1, numberOfCalls, index =>
			{
				var t = _action();
				lock (_syncLock)
				{
					if (((int) t.StatusCode) < 400)
						result.Ok++;
					else
						result.Failed++;
				
					callback(result);
				}
				if(wait > 0)
					Thread.Sleep(wait);
			});

			while (!res.IsCompleted) { Thread.Sleep(100); }
			return result;
		}
	}
}