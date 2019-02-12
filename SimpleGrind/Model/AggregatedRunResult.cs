using System.Collections.Generic;

namespace SimpleGrind.Model
{
    public class AggregatedRunResult
    {
        public List<RunResult> RunResults { get; } = new List<RunResult>();
        public int TotalCalls { get; set; }
        public long TotalTime { get; set; }
        public long TotalWaitingTime { get; set; }
        public long AverageTime { get; set; }
    }
}