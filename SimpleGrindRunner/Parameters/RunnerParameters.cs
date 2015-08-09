﻿namespace SimpleGrind.Runner.Parameters
{
    public class RunnerParameters
    {
        public RunnerParameters(ParameterBuilder parameterBuilder)
        {
            parameterBuilder.MapByArg<string>("b", val => Behavior = val);
            parameterBuilder.MapByArg<int>("n", val => NumberOfRuns = val);
            parameterBuilder.MapByArg<int>("i", val => IncreaseBy = val == 0 ? 1 : val);
            parameterBuilder.MapByArg<int>("w", val => Wait = val);
            parameterBuilder.MapByArg<int>("cl", val => ConnectionLimit = val);
        }
        public string Behavior { get; private set; } = "async";
        public int NumberOfRuns { get; private set; } = 10;
        public int IncreaseBy { get; private set; } = 5;
        public int Wait { get; private set; } = 0;
        public int ConnectionLimit { get; private set; } = 1000;
    }
}