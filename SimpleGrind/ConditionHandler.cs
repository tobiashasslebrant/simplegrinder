using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SimpleGrind.Model;
using SimpleGrind.Parameters;

namespace SimpleGrind
{
    public class ConditionHandler
    {
        private readonly IRunnerParameters _runnerParameters;

        public ConditionHandler(IRunnerParameters runnerParameters)
        {
            _runnerParameters = runnerParameters;
        }

        public (bool,string[]) Validate(AggregatedRunResult aggregatedResult)
        {
            var errorCount = aggregatedResult.RunResults.SelectMany(s => s.Errors).Count();

            var cond = Regex.Match(_runnerParameters.ErrorCondition,"^(.+)([<>!=%#])(.+)$");
            var field = cond.Groups[1].Value;
            var condition = cond.Groups[2].Value;
            var val = cond.Groups[3].Value;
            var errors = new List<string>();
             
            switch (field)
            {
                case "any":
                    if(errorCount > 0)
                        errors.Add($"condition: any. Has {errorCount} errors.");
                    break;
                case "totaltime":
                    if(HasError(aggregatedResult.TotalTime, 0))
                        errors.Add($"condition: {_runnerParameters.ErrorCondition}. Totaltime: {aggregatedResult.TotalTime}");
                    break;
                case "totalavg":
                    if(HasError(aggregatedResult.AverageTime, 0))
                        errors.Add($"condition: {_runnerParameters.ErrorCondition}. Averagetime: {aggregatedResult.AverageTime}");
                    break;
                case "totalerrors":
                    if(HasError(errorCount, aggregatedResult.TotalCalls))
                        errors.Add($"condition: {_runnerParameters.ErrorCondition}. Totalerrors: {errorCount}, Totalcalls: {aggregatedResult.TotalCalls}");
                    break;
                case "time":
                    foreach (var runResult in aggregatedResult.RunResults)
                        if(HasError(runResult.TotalTime, runResult.NumberOfCalls))
                            errors.Add($"condition: {_runnerParameters.ErrorCondition}. TotalTime: {runResult.TotalTime}, NumberOfCalls: {runResult.NumberOfCalls}");
                    break;
                case "avg":
                    foreach (var runResult in aggregatedResult.RunResults)
                        if(HasError(runResult.AverageTime, runResult.NumberOfCalls))
                            errors.Add($"condition: {_runnerParameters.ErrorCondition}. AverageTime: {runResult.AverageTime}, NumberOfCalls: {runResult.NumberOfCalls}");
                    break;
                case "ok":
                    foreach (var runResult in aggregatedResult.RunResults)
                        if(HasError(runResult.Ok, runResult.NumberOfCalls))
                            errors.Add($"condition: {_runnerParameters.ErrorCondition}. Ok: {runResult.Ok}, NumberOfCalls: {runResult.NumberOfCalls}");
                    break;
                case "failed":
                    foreach (var runResult in aggregatedResult.RunResults)
                        if(HasError(runResult.Failed, runResult.NumberOfCalls))
                            errors.Add($"condition: {_runnerParameters.ErrorCondition}. Failed: {runResult.Failed}, NumberOfCalls: {runResult.NumberOfCalls}");
                    break;
                case "timedout":
                    foreach (var runResult in aggregatedResult.RunResults)
                        if(HasError(runResult.TimedOut, runResult.NumberOfCalls))
                            errors.Add($"condition: {_runnerParameters.ErrorCondition}. Timedout: {runResult.TimedOut}, NumberOfCalls: {runResult.NumberOfCalls}");
                    break;
            }
            return (errors.Any(), errors.ToArray());

            bool HasError<T>(T fieldValue, int percentageCompare)
            {
                switch (condition)
                {
                    case "=": return (Equals(fieldValue, (T) Convert.ChangeType(val, typeof(T))));
                    case "!": return !(Equals(fieldValue, (T) Convert.ChangeType(val, typeof(T))));
                    case ">":
                        return (int) Convert.ChangeType(fieldValue, typeof(int)) >
                               (int) Convert.ChangeType(val, typeof(int));
                    case "<":
                        return (int) Convert.ChangeType(fieldValue, typeof(int)) <
                               (int) Convert.ChangeType(val, typeof(int));
                    case "%":
                        return (int) Convert.ChangeType(fieldValue, typeof(int)) * 100 / percentageCompare >
                               (int) Convert.ChangeType(val, typeof(int));
                    case "#":
                        return (int) Convert.ChangeType(fieldValue, typeof(int)) * 100 / percentageCompare <
                               (int) Convert.ChangeType(val, typeof(int));
                    default: return false;
                }
            }
        }
    }
}