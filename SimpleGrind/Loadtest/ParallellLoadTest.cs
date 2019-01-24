using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SimpleGrind.Parameters;

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

		public LoadResult Run(int numberOfCalls, int wait, LogLevel logLevel)
		{
			var result = new LoadResult();
			var errors = new List<string>();
			var res = Parallel.For(0, numberOfCalls, index =>
			{
				try
				{
					var t = _action();

					lock (_syncLock)
					{
						if (((int) t.StatusCode) < 400)
							result.Ok++;
						else
						{
							result.Failed++;
							if (logLevel == LogLevel.Verbose)
								errors.Add(t.Content.ReadAsStringAsync().Result);
						}
					}

					if (wait > 0)
						Thread.Sleep(wait);

				}
				catch (AggregateException ex)
				{
					if (ex.InnerException is TaskCanceledException)
						result.TimedOut++;
					else
						throw;
				}
				catch (Exception e)
				{
					result.Failed++;
					if (logLevel == LogLevel.Verbose)
						errors.Add(e.ToString());
				}
			});

			while (!res.IsCompleted)
			{
			}

			result.Errors = errors;
			return result;
		}
	}
}