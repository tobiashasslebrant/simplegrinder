﻿using System;
using System.Diagnostics;
using System.Globalization;
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
				var avgTime = stopWatchOne.ElapsedMilliseconds / numberOfCalls;
			     _gridWriter.WriteCells(new[]
				{
					result.Ok.ToString(),
					result.Failed.ToString(),
					stopWatchOne.ElapsedMilliseconds < 1000
						? $"{stopWatchOne.ElapsedMilliseconds} ms"
						: $"{stopWatchOne.ElapsedMilliseconds / 1000} s",
					avgTime < 1000
						? $"{avgTime} ms"
						: $"{avgTime / 1000} s"
				});
				stopWatchOne.Reset();
			}
            _gridWriter.WriteLine($"Total run time is { (stopWatchAll.ElapsedMilliseconds / 1000)} seconds {stopWatchAll.ElapsedMilliseconds % 1000} milliseconds.");
        }
    }
}
