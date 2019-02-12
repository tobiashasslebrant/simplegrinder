using System;
using System.Diagnostics;
using SimpleGrind.LoadTest;
using SimpleGrind.Model;
using SimpleGrind.Parameters;

namespace SimpleGrind
{
  
    public class LoadRunner
    {
        readonly ILoadTestFactory _loadTestFactory;
        readonly IRequestParameters _requestParameters;
        readonly IRunnerParameters _runnerParameters;

        public LoadRunner(ILoadTestFactory loadTestFactory, IRequestParameters requestParameters, IRunnerParameters runnerParameters)
        {
            _loadTestFactory = loadTestFactory;
            _requestParameters = requestParameters;
            _runnerParameters = runnerParameters;
        }

        public AggregatedRunResult Execute()
        {
            var runResults = new AggregatedRunResult();
            var loadTest = _loadTestFactory.Create(_runnerParameters.Behavior, _requestParameters.Method, _requestParameters.Url, _requestParameters.Json);
            var numberOfCalls = _runnerParameters.NumberOfCalls;
            var totalCalls = 0;
            var stopWatchOne = new Stopwatch();
            var stopWatchAll = new Stopwatch();
            stopWatchAll.Start();

            for (var run = 1; run <= _runnerParameters.NumberOfRuns; run++)
            {
                stopWatchOne.Start();
                var runResult = new RunResult();
                runResults.RunResults.Add(runResult);
                try
                {
                    var result = loadTest.Run(numberOfCalls, _runnerParameters.Wait, _runnerParameters.LogLevel);
                    var runTime = stopWatchOne.ElapsedMilliseconds - (numberOfCalls * _runnerParameters.Wait);
                    var avgTime = runTime / numberOfCalls;

                    runResult.Run = run;
                    runResult.NumberOfCalls = numberOfCalls;
                    runResult.Ok = result.Ok;
                    runResult.Failed = result.Failed;
                    runResult.TimedOut = result.TimedOut;
                    runResult.TotalTime = runTime;
                    runResult.AverageTime = avgTime;
                    runResult.Errors.AddRange(result.Errors);
                   
                }
                catch (Exception ex)
                {
                    runResult.Errors.Add(ex.ToString());
                }
                totalCalls += numberOfCalls;
                numberOfCalls += _runnerParameters.IncreaseBy;
                stopWatchOne.Reset();
            }

            var totalTime = stopWatchAll.ElapsedMilliseconds;
            runResults.TotalTime = totalTime;
            runResults.TotalCalls = totalCalls;
            runResults.TotalWaitingTime = totalCalls * _runnerParameters.Wait;
            runResults.AverageTime = (totalTime - totalCalls * _runnerParameters.Wait) / totalCalls;

            return runResults;
        }
    }
}