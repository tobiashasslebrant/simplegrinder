using System.Linq;
using SimpleGrind.Model;
using Xunit;

namespace SimpleGrind.Tests.Given_ConditionHandler
{
    public class When_Validate_avg_largerthan : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "avg>0";

        public When_Validate_avg_largerthan()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{AverageTime = 1},
                    new RunResult{AverageTime = 0},
                    new RunResult{AverageTime = 2},
                }
            });

        [Fact]
        public void Should_have_two_avg_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
    
    public class When_Validate_avg_lowerthan : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "avg<2";

        public When_Validate_avg_lowerthan()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{AverageTime = 1},
                    new RunResult{AverageTime = 0},
                    new RunResult{AverageTime = 2},
                }
            });

        [Fact]
        public void Should_have_two_avg_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
    
    public class When_Validate_avg_percentage : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "avg%49";

        public When_Validate_avg_percentage()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{AverageTime = 5},
                    new RunResult{AverageTime = 4},
                    new RunResult{AverageTime = 6},
                },
                AverageTime = 10
            });

        [Fact]
        public void Should_have_two_avg_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
    public class When_Validate_avg_percentage_lower : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "avg#51";

        public When_Validate_avg_percentage_lower()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{AverageTime= 5},
                    new RunResult{AverageTime = 4},
                    new RunResult{AverageTime = 6},
                },
                AverageTime = 10
            });

        [Fact]
        public void Should_have_two_avg_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
}