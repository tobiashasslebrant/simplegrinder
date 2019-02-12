using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using SimpleGrind.LoadTest;
using SimpleGrind.Model;
using SimpleGrind.Parameters;

namespace SimpleGrind
{
    public interface IMonitor
    {
        int Start();
    }

    public class Monitor : IMonitor
    {
	    readonly IGridWriter _gridWriter;
	    readonly ILoadTestFactory _loadTestFactory;
	    readonly IRequestParameters _requestParameters;
	    readonly IRunnerParameters _runnerParameters;
	    private readonly LoadRunner _runner;
	    private readonly ErrorConditionHandler _errorConditionHandler;

	    public Monitor(IGridWriter gridWriter, ILoadTestFactory loadTestFactory, 
		    IRequestParameters requestParameters, IRunnerParameters runnerParameters, 
		    LoadRunner runner,
		    ErrorConditionHandler errorConditionHandler)
        {
            _gridWriter = gridWriter;
            _loadTestFactory = loadTestFactory;
            _requestParameters = requestParameters;
            _runnerParameters = runnerParameters;
	        _runner = runner;
	        _errorConditionHandler = errorConditionHandler;
        }

	    public int Start()
	    {
		    var maxConcurrentCalls = 
				(_runnerParameters.NumberOfRuns * _runnerParameters.IncreaseBy) 
				- _runnerParameters.IncreaseBy 
				+ _runnerParameters.NumberOfCalls;

			if (maxConcurrentCalls > _runnerParameters.ConnectionLimit)
			{
				WriteLine($"Max concurrent calls {maxConcurrentCalls} are larger than connectionlimit {_runnerParameters.ConnectionLimit}.", Context.Parameters);
				return 1;
			}

			if (_runnerParameters.WaitUntil != null)
			{
				var waitFor = DateTime.ParseExact(_runnerParameters.WaitUntil,"yyyyMMdd hhmmss",DateTimeFormatInfo.InvariantInfo);
				var waitTime = waitFor - DateTime.Now;

				if (waitTime.TotalMilliseconds > Int32.MaxValue)
				{
					WriteLine("Waiting until time is to far away, try a smaller date", Context.Parameters);
					return 1;
				}
				WriteLine($"Wait for {waitFor.ToString("F")} ({waitTime.Days} days {waitTime.Hours} hours {waitTime.Minutes} minutes {waitTime.Seconds} seconds)", Context.Parameters);

				System.Threading.Thread.Sleep(waitTime);
			}

 
			WriteLine("\r\n====== Parameters ======", Context.Parameters);
			WriteLine($" Executing {_runnerParameters.NumberOfRuns} runs against [{_requestParameters.Method.ToUpper()}]{_requestParameters.Url}", Context.Parameters);
            WriteLine($" First run starts with {_runnerParameters.NumberOfCalls} calls and increasing by {_runnerParameters.IncreaseBy} calls between each run", Context.Parameters);
			WriteLine($" Each call will have a timeout of {_requestParameters.TimeOut}s and will wait {_runnerParameters.Wait}ms between each call", Context.Parameters);
			WriteLine("\r\n====== Result ======", Context.Result);
         
		    WriteHeaders(new[] { "Run", "Calls", "Ok", "Failed","Timed Out", "Total Time", "Average Time" });
			var aggregatedResult = _runner.Execute();
			foreach (var runResult in aggregatedResult.RunResults)
		    {
			    WriteCell(runResult.Run.ToString());
			    WriteCell(runResult.NumberOfCalls.ToString());
			    WriteCells(new[]
			    {
				    runResult.Ok.ToString(),
				    runResult.Failed.ToString(),
				    runResult.TimedOut.ToString(),
				    runResult.TotalTime < 1000
					    ? $"{runResult.TotalTime} ms"
					    : $"{runResult.TotalTime / 1000D:F1} s",
				    runResult.AverageTime < 1000
					    ? $"{runResult.AverageTime} ms"
					    : $"{runResult.AverageTime / 1000D:F1} s"
			    });
		    }
		    
			
			WriteLine($"\r\n====== Summary ======", Context.Summary);
			WriteLine($" A total of {aggregatedResult.TotalCalls} calls where made", Context.Summary);
		    if (_runnerParameters.Wait > 0)
		    {
			    WriteLine($" Total time is {(aggregatedResult.TotalTime / 1000)} seconds {aggregatedResult.TotalTime % 1000} milliseconds (including waiting time)", Context.Summary);
			    WriteLine($" Total waiting is {(aggregatedResult.TotalWaitingTime) / 1000} seconds {(aggregatedResult.TotalWaitingTime) % 1000D} milliseconds", Context.Summary);
			    WriteLine($" Average time is {aggregatedResult.AverageTime} milliseconds (excluding waiting time)", Context.Summary);
		    }
		    else
		    {
			    WriteLine($" Total time is {(aggregatedResult.TotalTime / 1000)} seconds {aggregatedResult.TotalTime % 1000} milliseconds", Context.Summary);
			    WriteLine($" Average time is {aggregatedResult.AverageTime} milliseconds", Context.Summary);    
		    }
			
		    var errors = aggregatedResult.RunResults.SelectMany(c => c.Errors.Select(s => (c.Run, s))).ToArray();
			if (errors.Any())
			{
				WriteLine($" Total of {aggregatedResult.RunResults.SelectMany(c => c.Errors).Count()} errors", Context.Summary);

				WriteLine($"\r\n====== Errors ====== ", Context.Errors);
				if (errors.Count() > _runnerParameters.LogItems)
					WriteLine($" Showing first {_runnerParameters.LogItems} errors", Context.Errors);

				foreach (var (run, error) in errors.Take(_runnerParameters.LogItems))
				{
					WriteLine($"\r\n>>>  Error in run {run} <<<", Context.Errors);
					WriteLine($"{error}", Context.Errors);
				}

				if (errors.Count() > _runnerParameters.LogItems)
					WriteLine($"\r\n... more errors ...", Context.Errors);
			 }


		    if (_runnerParameters.ErrorCondition != string.Empty)
		    {
			    var (conditionRaised, conditions) = _errorConditionHandler.Validate(aggregatedResult);
			    if (conditionRaised)
			    {
				    WriteLine($"\r\n====== Conditions ====== ", Context.Conditions);
				    foreach (var condition in conditions)
						WriteLine(condition, Context.Conditions);

				    return 1;
			    }
		    }

		    return 0;
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
		    Errors,
		    Conditions
	    }
	    
    }
}
