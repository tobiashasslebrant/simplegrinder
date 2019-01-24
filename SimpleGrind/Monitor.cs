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
			var maxConcurrentCalls = 
				(_runnerParameters.NumberOfRuns * _runnerParameters.IncreaseBy) 
				- _runnerParameters.IncreaseBy 
				+ _runnerParameters.NumberOfCalls;

			if (maxConcurrentCalls > _runnerParameters.ConnectionLimit)
			{
				_gridWriter.WriteLine($"Max concurrent calls {maxConcurrentCalls} are larger than connectionlimit {_runnerParameters.ConnectionLimit}.");
				return;
			}

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
 
			_gridWriter.WriteLine("====== Parameters ======");
			_gridWriter.WriteLine($" Executing {_runnerParameters.NumberOfRuns} runs against [{_requestParameters.Method.ToUpper()}]{_requestParameters.Url}");
            _gridWriter.WriteLine($" First run starts with {_runnerParameters.NumberOfCalls} calls and increasing by {_runnerParameters.IncreaseBy} calls between each run");
			_gridWriter.WriteLine($" Each call will have a timeout of {_requestParameters.TimeOut}s and will wait {_runnerParameters.Wait}ms between each call");
			_gridWriter.WriteLine("====== Result ======");
            _gridWriter.WriteHeaders(new[] { "Run", "Calls", "Ok", "Failed","Timed Out", "Total Time", "Average Time" });

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
					var runTime = stopWatchOne.ElapsedMilliseconds - (numberOfCalls * _runnerParameters.Wait);
					var avgTime = runTime / numberOfCalls;
					_gridWriter.WriteCells(new[]
					{
						result.Ok.ToString(),
						result.Failed.ToString(),
						result.TimedOut.ToString(),
						stopWatchOne.ElapsedMilliseconds < 1000
							? $"{runTime} ms"
							: $"{runTime / 1000D:F1} s",
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
					_gridWriter.WriteLine("error occured");
				}
				totalCalls += numberOfCalls;
				numberOfCalls += _runnerParameters.IncreaseBy;
				stopWatchOne.Reset();
			}

			_gridWriter.WriteLine($"====== Metrics ======");
			_gridWriter.WriteLine($" A total of {totalCalls} calls where made");
			_gridWriter.WriteLine($" Total time is {(stopWatchAll.ElapsedMilliseconds / 1000)} seconds {stopWatchAll.ElapsedMilliseconds % 1000} milliseconds");
			if(_runnerParameters.Wait > 0)
				_gridWriter.WriteLine($" Of total time waiting is {(totalCalls * _runnerParameters.Wait) / 1000} seconds {(totalCalls * _runnerParameters.Wait) % 1000D} milliseconds");
			_gridWriter.WriteLine($" Average time is {((stopWatchAll.ElapsedMilliseconds - (totalCalls * _runnerParameters.Wait))/ totalCalls)} milliseconds");
			
			if (errors.Any())
			{
				_gridWriter.WriteLine($"====== Errors ====== ");
				_gridWriter.WriteLine($" Total of {errors.Count()} errors");

				if (_runnerParameters.LogLevel == LogLevel.Verbose)
				{
					if (errors.Count > _runnerParameters.LogItems)
						_gridWriter.WriteLine($" Showing first {_runnerParameters.LogItems} errors");

					foreach (var (run, error) in errors.Take(_runnerParameters.LogItems))
					{
						_gridWriter.WriteLine($">>>  Error in run {run} <<<");
						_gridWriter.WriteLine($"{error}");
					}

					if (errors.Count > _runnerParameters.LogItems)
						_gridWriter.WriteLine($"... more errors ...");
				}
			}
        }
    }
}
