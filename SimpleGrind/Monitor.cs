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
				WriteLine($"Max concurrent calls {maxConcurrentCalls} are larger than connectionlimit {_runnerParameters.ConnectionLimit}.", Context.Parameters);
				return;
			}

			if (_runnerParameters.WaitUntil != null)
			{
				var waitFor = DateTime.ParseExact(_runnerParameters.WaitUntil,"yyyyMMdd hhmmss",DateTimeFormatInfo.InvariantInfo);
				var waitTime = waitFor - DateTime.Now;

				if (waitTime.TotalMilliseconds > Int32.MaxValue)
				{
					WriteLine("Waiting until time is to far away, try a smaller date", Context.Parameters);
					return;
				}
				WriteLine($"Wait for {waitFor.ToString("F")} ({waitTime.Days} days {waitTime.Hours} hours {waitTime.Minutes} minutes {waitTime.Seconds} seconds)", Context.Parameters);

				System.Threading.Thread.Sleep(waitTime);
			}

		    var loadTest = _loadTestFactory.Create(_runnerParameters.Behavior, _requestParameters.Method, _requestParameters.Url, _requestParameters.Json);
            var numberOfCalls = _runnerParameters.NumberOfCalls;
			var totalCalls = 0;
            var stopWatchOne = new Stopwatch();
            var stopWatchAll = new Stopwatch();
 
			WriteLine("\r\n====== Parameters ======", Context.Parameters);
			WriteLine($" Executing {_runnerParameters.NumberOfRuns} runs against [{_requestParameters.Method.ToUpper()}]{_requestParameters.Url}", Context.Parameters);
            WriteLine($" First run starts with {_runnerParameters.NumberOfCalls} calls and increasing by {_runnerParameters.IncreaseBy} calls between each run", Context.Parameters);
			WriteLine($" Each call will have a timeout of {_requestParameters.TimeOut}s and will wait {_runnerParameters.Wait}ms between each call", Context.Parameters);
			WriteLine("\r\n====== Result ======", Context.Result);
            WriteHeaders(new[] { "Run", "Calls", "Ok", "Failed","Timed Out", "Total Time", "Average Time" });

			stopWatchAll.Start();
			var errors = new List<(int,string)>();
            for (var run = 1; run <= _runnerParameters.NumberOfRuns; run++)
			{
				WriteCell(run.ToString());
				WriteCell(numberOfCalls.ToString());
				stopWatchOne.Start();
				try
				{
					var result = loadTest.Run(numberOfCalls, _runnerParameters.Wait, _runnerParameters.LogLevel);
					var runTime = stopWatchOne.ElapsedMilliseconds - (numberOfCalls * _runnerParameters.Wait);
					var avgTime = runTime / numberOfCalls;
					WriteCells(new[]
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
					WriteLine("error occured", Context.Result);
				}
				totalCalls += numberOfCalls;
				numberOfCalls += _runnerParameters.IncreaseBy;
				stopWatchOne.Reset();
			}

			WriteLine($"\r\n====== Summary ======", Context.Summary);
			WriteLine($" A total of {totalCalls} calls where made", Context.Summary);
			WriteLine($" Total time is {(stopWatchAll.ElapsedMilliseconds / 1000)} seconds {stopWatchAll.ElapsedMilliseconds % 1000} milliseconds", Context.Summary);
			if(_runnerParameters.Wait > 0)
				WriteLine($" Of total time waiting is {(totalCalls * _runnerParameters.Wait) / 1000} seconds {(totalCalls * _runnerParameters.Wait) % 1000D} milliseconds", Context.Summary);
			WriteLine($" Average time is {((stopWatchAll.ElapsedMilliseconds - (totalCalls * _runnerParameters.Wait))/ totalCalls)} milliseconds", Context.Summary);
			
			if (errors.Any())
			{
				WriteLine($" Total of {errors.Count()} errors", Context.Summary);

				WriteLine($"\r\n====== Errors ====== ", Context.Errors);
				if (errors.Count > _runnerParameters.LogItems)
					WriteLine($" Showing first {_runnerParameters.LogItems} errors", Context.Errors);

				foreach (var (run, error) in errors.Take(_runnerParameters.LogItems))
				{
					WriteLine($"\r\n>>>  Error in run {run} <<<", Context.Errors);
					WriteLine($"{error}", Context.Errors);
				}

				if (errors.Count > _runnerParameters.LogItems)
					WriteLine($"\r\n... more errors ...", Context.Errors);
			}
        }

	    void WriteLine(string line, Context context)
	    {
		    switch (_runnerParameters.LogLevel)
		    {
			    case LogLevel.Friendly:
				    if(context != Context.Errors)
						_gridWriter.WriteLine(line);
				    break;
			    case LogLevel.Verbose:
				    _gridWriter.WriteLine(line);
				    break;
			    case LogLevel.Report:
				    if(context == Context.Result)
					    _gridWriter.WriteLine(line);
				    break;
			    case LogLevel.Summary:
				    if(context == Context.Summary)
					    _gridWriter.WriteLine(line);
				    break;
			    default:
				    throw new ArgumentOutOfRangeException();
		    }
	    }
	    
	    void WriteHeaders(string[] headers)
	    {
		    if(_runnerParameters.LogLevel == LogLevel.Summary)
			    return;
		    _gridWriter.WriteHeaders(headers);
	    }
	    void WriteCells(string[] cells)
	    {
		    if(_runnerParameters.LogLevel == LogLevel.Summary)
			    return;
		    _gridWriter.WriteCells(cells);
	    }

	    void WriteCell(string cell)
	    {
		    if(_runnerParameters.LogLevel == LogLevel.Summary)
			    return;
		    _gridWriter.WriteCell(cell);
	    }

	    enum Context
	    {
		    Parameters,
		    Result,
		    Summary,
		    Errors
	    }
	    
    }
}
