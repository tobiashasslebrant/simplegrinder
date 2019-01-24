﻿using System;

namespace SimpleGrind.Parameters
{
    public interface IRunnerParameters
    {
        string Behavior { get; }
        int NumberOfRuns { get; }
        int NumberOfCalls { get; }
        int IncreaseBy { get; }
        int Wait { get; }
        int ConnectionLimit { get; }
        string WaitUntil { get; }
        LogLevel LogLevel { get; set; }
        int LogItems { get; set; }
    }
    public class RunnerParameters : IRunnerParameters
    {
        public RunnerParameters(ParameterBuilder parameterBuilder)
        {
            parameterBuilder.MapByArg<string>("b", val => Behavior = val);
            parameterBuilder.MapByArg<int>("nr", val => NumberOfRuns = val);
            parameterBuilder.MapByArg<int>("nc", val => NumberOfCalls = val);
            parameterBuilder.MapByArg<int>("ic", val => IncreaseBy = val);
            parameterBuilder.MapByArg<int>("w", val => Wait = val);
            parameterBuilder.MapByArg<int>("cl", val => ConnectionLimit = val);
            parameterBuilder.MapByArg<string>("wu", val => WaitUntil = val);
            parameterBuilder.MapByArg<string>("ll", val => LogLevel = Enum.Parse<LogLevel>(val,true));
            parameterBuilder.MapByArg<int>("li", val => LogItems = val);
        }
        public string Behavior { get; private set; } = "async";
        public int NumberOfRuns { get; private set; } = 10;
        public int NumberOfCalls { get; private set; } = 10;
        public int IncreaseBy { get; private set; } = 5;
        public int Wait { get; private set; } = 0;
        public int ConnectionLimit { get; private set; } = 1000;
        public string WaitUntil { get; private set; } = null;
        public LogLevel LogLevel { get; set; } = LogLevel.Friendly;
        public int LogItems { get; set; } = 3;
    }

    public enum LogLevel
    {
        Friendly,
        Verbose,
        Report
    }
}
