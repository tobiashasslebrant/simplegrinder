using System.Diagnostics;
using SimpleGrind.Loadtest;
using SimpleGrind.Runner.Parameters;

namespace SimpleGrind.Runner
{
    public interface IMonitor
    {
        void Start(ILoadTest loadTest, RunnerParameters runnerParameters);
    }

    public class Monitor : IMonitor
    {
        private IGridWriter _gridWriter;

        public Monitor(IGridWriter gridWriter)
        {
            _gridWriter = gridWriter;
        }
		public void Start(ILoadTest loadTest, RunnerParameters runnerParameters)
		{
			var stopWatch = new Stopwatch();

            _gridWriter.WriteHeaders(new[] { "Run", "NoOfCalls", "Ok", "Failed", "TotalTime", "AvgTime" });
            var numberOfCalls = runnerParameters.IncreaseBy;
			for (var run = 1; run <= runnerParameters.NumberOfRuns; run++)
			{
				numberOfCalls += runnerParameters.IncreaseBy;

                _gridWriter.WriteCell(run.ToString());
                _gridWriter.WriteCell(numberOfCalls.ToString());
				stopWatch.Start();

				var result = loadTest.Run(numberOfCalls, runnerParameters.Wait);
				
				var totalSec = (int)stopWatch.ElapsedMilliseconds / 1000;
                _gridWriter.WriteCells(new[]
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
