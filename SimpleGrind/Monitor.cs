using System.Diagnostics;
using SimpleGrind.Loadtest;
using SimpleGrind.Parameters;

namespace SimpleGrind
{
    public interface IMonitor
    {
        void Start();
    }

    public class Monitor : IMonitor
    {
        IGridWriter _gridWriter;
        ILoadTestFactory _loadTestFactory;
        IRequestParameters _requestParameters;
        IRunnerParameters _runnerParameters;

        public Monitor(IGridWriter gridWriter, ILoadTestFactory loadTestFactory, IRequestParameters requestParameters, IRunnerParameters runnerParameters)
        {
            _gridWriter = gridWriter;
            _loadTestFactory = loadTestFactory;
            _requestParameters = requestParameters;
            _runnerParameters = runnerParameters;
        }
		public void Start()
		{
            var loadTest = _loadTestFactory.Create(_runnerParameters.Behavior, _requestParameters.Method, _requestParameters.Url, _requestParameters.Json);
            var numberOfCalls = _runnerParameters.IncreaseBy;
            var stopWatchOne = new Stopwatch();
            var stopWatchAll = new Stopwatch();

            _gridWriter.WriteLine($"Starting {_runnerParameters.NumberOfRuns} runs with {_requestParameters.Method} against {_requestParameters.Url}.");
            _gridWriter.WriteLine($"Increase each run by {_runnerParameters.IncreaseBy} requests.");
            _gridWriter.WriteHeaders(new[] { "Run", "NoOfCalls", "Ok", "Failed", "TotTime", "AvgTime" });

			stopWatchAll.Start();
            for (var run = 1; run <= _runnerParameters.NumberOfRuns; run++)
			{
				numberOfCalls += _runnerParameters.IncreaseBy;
                _gridWriter.WriteCell(run.ToString());
                _gridWriter.WriteCell(numberOfCalls.ToString());
				stopWatchOne.Start();
				var result = loadTest.Run(numberOfCalls, _runnerParameters.Wait);
				var totalSec = (int)stopWatchOne.ElapsedMilliseconds / 1000;
                _gridWriter.WriteCells(new[]
				{
					result.Ok.ToString(),
					result.Failed.ToString(),
					$"{totalSec}s",
					$"{(totalSec / numberOfCalls)}s"
				});
				stopWatchOne.Reset();
			}
            _gridWriter.WriteLine("Total run time is {0} seconds.", (stopWatchAll.ElapsedMilliseconds / 1000));
        }
    }
}
