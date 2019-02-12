using System.Collections.Generic;

namespace SimpleGrind.Model
{
    public class RunResult
    {
        public int Run{ get; set; }
        public int NumberOfCalls { get; set; }
        
        public int Ok{ get; set; }
        public int Failed{ get; set; }
        public int TimedOut{ get; set; }
        public List<string> Errors { get;  } = new List<string>();
        public long TotalTime { get; set; }
        public long AverageTime { get; set; }
    }
}