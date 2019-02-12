using System.Linq;
using SimpleGrind.Model;
using Xunit;

namespace SimpleGrind.Tests.Given_ConditionHandler
{
    public class When_Validate_time_largerthan : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "time>0";

        public When_Validate_time_largerthan()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{TotalTime = 1},
                    new RunResult{TotalTime = 0},
                    new RunResult{TotalTime = 2},
                }
            });

        [Fact]
        public void Should_have_two_time_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
    
    public class When_Validate_time_lowerthan : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "time<2";

        public When_Validate_time_lowerthan()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{TotalTime = 1},
                    new RunResult{TotalTime = 0},
                    new RunResult{TotalTime = 2},
                }
            });

        [Fact]
        public void Should_have_two_time_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
    
    public class When_Validate_time_percentage : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "time%49";

        public When_Validate_time_percentage()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{TotalTime = 5},
                    new RunResult{TotalTime = 4},
                    new RunResult{TotalTime = 6},
                },
                TotalTime = 10
            });

        [Fact]
        public void Should_have_two_time_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
    public class When_Validate_time_percentage_lower : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "time#51";

        public When_Validate_time_percentage_lower()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{TotalTime= 5},
                    new RunResult{TotalTime = 4},
                    new RunResult{TotalTime = 6},
                },
                TotalTime = 10
            });

        [Fact]
        public void Should_have_two_time_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
}