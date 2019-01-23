using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
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
	    readonly IGridWriter _gridWriter;
	    readonly ILoadTestFactory _loadTestFactory;
	    readonly IRequestParameters _requestParameters;
	    readonly IRunnerParameters _runnerParameters;

        public Monitor(IGridWriter gridWriter, ILoadTestFactory loadTestFactory, IRequestParameters requestParameters, IRunnerParameters runnerParameters)
        {
            _gridWriter = gridWriter;
            _loadTestFactory = loadTestFactory;
            _requestParameters = requestParameters;
            _runnerParameters = runnerParameters;
        }
		public void Start()
		{
			if (_runnerParameters.WaitUntil != null)
			{
				var waitFor = DateTime.ParseExact(_runnerParameters.WaitUntil,"yyyyMMdd hhmmss",DateTimeFormatInfo.InvariantInfo);
				var waitTime = waitFor - DateTime.Now;

				if (waitTime.TotalMilliseconds > Int32.MaxValue)
				{
					_gridWriter.WriteLine("Waiting until time is to far away, try a smaller date");
					return;
				}
				_gridWriter.WriteLine($"Wait for {waitFor.ToString("F")} ({waitTime.Days} days {waitTime.Hours} hours {waitTime.Minutes} minutes {waitTime.Seconds} seconds)");

				System.Threading.Thread.Sleep(waitTime);
			}
			
            var loadTest = _loadTestFactory.Create(_runnerParameters.Behavior, _requestParameters.Method, _requestParameters.Url, _requestParameters.Json);
            var numberOfCalls = _runnerParameters.NumberOfCalls;
			var totalCalls = 0;
            var stopWatchOne = new Stopwatch();
            var stopWatchAll = new Stopwatch();

            _gridWriter.WriteLine($"Starting {_runnerParameters.NumberOfRuns} runs against {_requestParameters.Method}:{_requestParameters.Url}");
            _gridWriter.WriteLine($"Run starts with {_runnerParameters.NumberOfCalls} calls and increasing by {_runnerParameters.IncreaseBy} calls between runs.");
            _gridWriter.WriteHeaders(new[] { "Run", "NoOfCalls", "Ok", "Failed", "TotTime", "AvgTime" });

			stopWatchAll.Start();
			var errors = new List<(int,string)>();
            for (var run = 1; run <= _runnerParameters.NumberOfRuns; run++)
			{
				
					_gridWriter.WriteCell(run.ToString());
					_gridWriter.WriteCell(numberOfCalls.ToString());
					stopWatchOne.Start();
				try
				{
					var result = loadTest.Run(numberOfCalls, _runnerParameters.Wait, _runnerParameters.LogLevel);
					var avgTime = stopWatchOne.ElapsedMilliseconds / numberOfCalls;
					_gridWriter.WriteCells(new[]
					{
						result.Ok.ToString(),
						result.Failed.ToString(),
						stopWatchOne.ElapsedMilliseconds < 1000
							? $"{stopWatchOne.ElapsedMilliseconds} ms"
							: $"{stopWatchOne.ElapsedMilliseconds / 1000D:F1} s",
						avgTime < 1000
							? $"{avgTime} ms"
							: $"{avgTime / 1000D:F1} s"
					});
					if(result.Errors.Any())
						foreach (var error in result.Errors)
							errors.Add((run,error));
				}
				catch (Exception ex)
				{
					errors.Add((run,ex.ToString()));
					_gridWriter.WriteCells(new[] {"error","","",""});
				}
				totalCalls += numberOfCalls;
				numberOfCalls += _runnerParameters.IncreaseBy;
				stopWatchOne.Reset();
			}
            _gridWriter.WriteLine($"Total run time is { (stopWatchAll.ElapsedMilliseconds / 1000)} seconds {stopWatchAll.ElapsedMilliseconds % 1000} milliseconds for {totalCalls} calls. Average time is { (stopWatchAll.ElapsedMilliseconds / totalCalls)} milliseconds");
			if (errors.Any())
			{
				_gridWriter.WriteLine($"Total of {errors.Count()} errors");

				if (_runnerParameters.LogLevel == "VERBOSE")
				{
					if (errors.Count > _runnerParameters.LogItems)
						_gridWriter.WriteLine($"Showing first {_runnerParameters.LogItems} errors");

					foreach (var (run, error) in errors.Take(_runnerParameters.LogItems))
						_gridWriter.WriteLine($"Error in run {run} MESSAGE: {error}");

					if (errors.Count > _runnerParameters.LogItems)
						_gridWriter.WriteLine($"... more errors ...");

				}

			}
        }
    }
}
