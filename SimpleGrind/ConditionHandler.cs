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
            => _runnerParameters = runnerParameters;

        public (bool, string[]) Validate(AggregatedRunResult aggregatedResult)
        {
            var errorCount = aggregatedResult.RunResults.SelectMany(s => s.Errors).Count();
            var conditions = _runnerParameters.ExitCondition.Split(';');
            var errors = new List<string>();

            foreach (var condition in conditions)
                Validate(condition);
            
            return (errors.Any(), errors.ToArray());

            void Validate(string condition)
            {
                var cond = Regex.Match(condition, "^(.+)([<>!=%#])(.+)$");
                var field = cond.Groups[1].Value;
                var comparer = cond.Groups[2].Value;
                var val = long.Parse(cond.Groups[3].Value);
               
                switch (field.ToLower())
                {
                    case "ok":
                        foreach (var runResult in aggregatedResult.RunResults)
                            if (HasError(runResult.Ok, runResult.NumberOfCalls))
                                errors.Add(
                                    $"{_runnerParameters.ExitCondition}. Ok: {runResult.Ok}, NumberOfCalls: {runResult.NumberOfCalls}");
                        break;
                    case "failed":
                        foreach (var runResult in aggregatedResult.RunResults)
                            if (HasError(runResult.Failed, runResult.NumberOfCalls))
                                errors.Add(
                                    $"{_runnerParameters.ExitCondition}. Failed: {runResult.Failed}, NumberOfCalls: {runResult.NumberOfCalls}");
                        break;
                    case "timedout":
                        foreach (var runResult in aggregatedResult.RunResults)
                            if (HasError(runResult.TimedOut, runResult.NumberOfCalls))
                                errors.Add(
                                    $"{_runnerParameters.ExitCondition}. Timedout: {runResult.TimedOut}, NumberOfCalls: {runResult.NumberOfCalls}");
                        break;
                    case "time":
                        foreach (var runResult in aggregatedResult.RunResults)
                            if (HasError(runResult.TotalTime, aggregatedResult.TotalTime))
                                errors.Add(
                                    $"{_runnerParameters.ExitCondition}. Time: {runResult.TotalTime}, TotTime: {aggregatedResult.TotalTime}");
                        break;
                    case "avg":
                        foreach (var runResult in aggregatedResult.RunResults)
                            if (HasError(runResult.AverageTime, aggregatedResult.AverageTime))
                                errors.Add(
                                    $"{_runnerParameters.ExitCondition}. AverageTime: {runResult.AverageTime}, TotAverageTime: {aggregatedResult.AverageTime}");
                        break;
                    case "totaltime":
                        if (HasError(aggregatedResult.TotalTime))
                            errors.Add(
                                $"{_runnerParameters.ExitCondition}. Totaltime: {aggregatedResult.TotalTime}");
                        break;
                    case "totalavg":
                        if (HasError(aggregatedResult.AverageTime))
                            errors.Add(
                                $"{_runnerParameters.ExitCondition}. Averagetime: {aggregatedResult.AverageTime}");
                        break;
                    case "totalerrors":
                        if (HasError(errorCount, aggregatedResult.TotalCalls))
                            errors.Add(
                                $"{_runnerParameters.ExitCondition}. Totalerrors: {errorCount}, Totalcalls: {aggregatedResult.TotalCalls}");
                        break;

                }

                bool HasError(long fieldValue, long percentageCalcValue = 0)
                {
                    switch (comparer)
                    {
                        case "=": return fieldValue == val;
                        case "!": return fieldValue != val;
                        case ">": return fieldValue > val;
                        case "<": return fieldValue < val;
                        case "%": return fieldValue * 100 / percentageCalcValue > val;
                        case "#": return fieldValue * 100 / percentageCalcValue < val;
                        default: return false;
                    }
                }
            }
        }
    }
}