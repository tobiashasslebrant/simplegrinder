using System.Linq;
using SimpleGrind.Model;
using Xunit;

namespace SimpleGrind.Tests.Given_ConditionHandler
{
    public class When_Validate_timedout_largerthan : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "timedout>0";

        public When_Validate_timedout_largerthan()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{TimedOut = 1},
                    new RunResult{TimedOut = 0},
                    new RunResult{TimedOut = 2},
                }
            });

        [Fact]
        public void Should_have_two_timedout_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
    
    public class When_Validate_timedout_lowerthan : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "timedout<2";

        public When_Validate_timedout_lowerthan()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{TimedOut = 1},
                    new RunResult{TimedOut = 0},
                    new RunResult{TimedOut = 2},
                }
            });

        [Fact]
        public void Should_have_two_timedout_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
    
    public class When_Validate_timedout_percentage : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "timedout%49";

        public When_Validate_timedout_percentage()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{NumberOfCalls = 10, TimedOut = 5},
                    new RunResult{NumberOfCalls = 10, TimedOut = 4},
                    new RunResult{NumberOfCalls = 10, TimedOut = 6},
                }
            });

        [Fact]
        public void Should_have_two_timedout_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
    public class When_Validate_timedout_percentage_lower : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "timedout#51";

        public When_Validate_timedout_percentage_lower()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{NumberOfCalls = 10, TimedOut = 5},
                    new RunResult{NumberOfCalls = 10, TimedOut = 4},
                    new RunResult{NumberOfCalls = 10, TimedOut = 6},
                }
            });

        [Fact]
        public void Should_have_two_timedout_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
}