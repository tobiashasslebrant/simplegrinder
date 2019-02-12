using System.Linq;
using SimpleGrind.Model;
using Xunit;

namespace SimpleGrind.Tests.Given_ConditionHandler
{
    public class When_Validate_failed_largerthan : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "failed>0";

        public When_Validate_failed_largerthan()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{Failed = 1},
                    new RunResult{Failed = 0},
                    new RunResult{Failed = 2},
                }
            });

        [Fact]
        public void Should_have_two_failed_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
    
    public class When_Validate_failed_lowerthan : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "failed<2";

        public When_Validate_failed_lowerthan()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{Failed = 1},
                    new RunResult{Failed = 0},
                    new RunResult{Failed = 2},
                }
            });

        [Fact]
        public void Should_have_two_failed_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
    
    public class When_Validate_failed_percentage : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "failed%49";

        public When_Validate_failed_percentage()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{NumberOfCalls = 10, Failed = 5},
                    new RunResult{NumberOfCalls = 10, Failed = 4},
                    new RunResult{NumberOfCalls = 10, Failed = 6},
                }
            });

        [Fact]
        public void Should_have_two_failed_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
    public class When_Validate_failed_percentage_lower : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "failed#51";

        public When_Validate_failed_percentage_lower()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{NumberOfCalls = 10, Failed = 5},
                    new RunResult{NumberOfCalls = 10, Failed = 4},
                    new RunResult{NumberOfCalls = 10, Failed = 6},
                }
            });

        [Fact]
        public void Should_have_two_failed_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
}