using System.Diagnostics;
using SimpleGrind.Loadtest;

namespace SimpleGrindRunner
{
	public static class Monitor
	{
		public static void Start(ILoadTest loadTest, int numberOfRuns, int increaseBy, int wait)
		{
			var stopWatch = new Stopwatch();

			GridConsole.WriteCells(new[] { "Run", "NoOfCalls", "Ok", "Failed", "TotalTime", "AvgTime" });
			var numberOfCalls = increaseBy == 0 ? 1 : 0;
			for (var run = 1; run <= numberOfRuns; run++)
			{
				numberOfCalls += increaseBy;

				GridConsole.WriteCell(run.ToString());
				GridConsole.WriteCell(numberOfCalls.ToString());
				stopWatch.Start();

				var result = loadTest.Run(numberOfCalls, wait, r => GridConsole.WriteNoPersistantCells(new[]
				{
					r.Ok.ToString(), 
					r.Failed.ToString()
				}));
				
				var totalSec = (int)stopWatch.ElapsedMilliseconds / 1000;
				GridConsole.WriteCells(new[]
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
