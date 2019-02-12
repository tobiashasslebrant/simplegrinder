using System.Linq;
using SimpleGrind.Model;
using Xunit;

namespace SimpleGrind.Tests.Given_ConditionHandler
{
    public class When_Validate_ok_largerthan : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "ok>0";

        public When_Validate_ok_largerthan()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{Ok = 1},
                    new RunResult{Ok = 0},
                    new RunResult{Ok = 2},
                }
            });

        [Fact]
        public void Should_have_two_ok_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
    
    public class When_Validate_ok_lowerthan : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "ok<2";

        public When_Validate_ok_lowerthan()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{Ok = 1},
                    new RunResult{Ok = 0},
                    new RunResult{Ok = 2},
                }
            });

        [Fact]
        public void Should_have_two_ok_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
    
    public class When_Validate_ok_percentage : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "ok%49";

        public When_Validate_ok_percentage()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{NumberOfCalls = 10, Ok = 5},
                    new RunResult{NumberOfCalls = 10, Ok = 4},
                    new RunResult{NumberOfCalls = 10, Ok = 6},
                }
            });

        [Fact]
        public void Should_have_two_ok_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
    public class When_Validate_ok_percentage_lower : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "ok#51";

        public When_Validate_ok_percentage_lower()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults =
                {
                    new RunResult{NumberOfCalls = 10, Ok = 5},
                    new RunResult{NumberOfCalls = 10, Ok = 4},
                    new RunResult{NumberOfCalls = 10, Ok = 6},
                }
            });

        [Fact]
        public void Should_have_two_ok_conditions()
            => Assert.Equal(2,_result.Item2.Count());
    }
}