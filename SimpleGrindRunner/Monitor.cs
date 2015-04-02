using System;
using System.Diagnostics;
using SimpleGrind.Loadtest;

namespace SimpleGrindRunner
{
	public static class Monitor
	{
		public static void Start(ILoadTest loadTest, int numberOfRuns, int increaseBy, int wait)
		{
			var stopWatch = new Stopwatch();

			Cons.WriteLine(new[] { "Run", "NoOfCalls", "Ok", "Failed", "TotalTime", "AvgTime" });
			var numberOfCalls = increaseBy == 0 ? 1 : 0;
			for (var run = 1; run <= numberOfRuns; run++)
			{
				numberOfCalls += increaseBy;

				Cons.Write(run.ToString());
				Cons.Write(numberOfCalls.ToString());
				stopWatch.Start();

				var callback = new Action<LoadResult>(r => Cons.WriteBufferLine(new[] { r.Ok.ToString(), r.Failed.ToString() }));
				var result = loadTest.Run(numberOfCalls,callback, wait);
				var totalSec = (int)stopWatch.ElapsedMilliseconds / 1000;
				Cons.WriteLine(new[]
				{
					result.Ok.ToString(),
					result.Failed.ToString(),
					totalSec.ToString(),
					(totalSec / numberOfCalls).ToString()
				});
				stopWatch.Reset();

			}
		}
	}
}
