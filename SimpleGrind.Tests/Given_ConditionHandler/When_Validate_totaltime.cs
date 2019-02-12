using System.Linq;
using SimpleGrind.Model;
using Xunit;

namespace SimpleGrind.Tests.Given_ConditionHandler
{
    public class When_Validate_totaltime_largerthan : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "totaltime>0";

        public When_Validate_totaltime_largerthan()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                TotalTime = 10
            });

        [Fact]
        public void Should_have_condition()
            => Assert.Single(_result.Item2);
    }
    
    public class When_Validate_totaltime_lowerthan : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "totaltime<10";

        public When_Validate_totaltime_lowerthan()
            => _result = Subject.Validate(new AggregatedRunResult
            {
                TotalTime = 4
            });

        [Fact]
        public void Should_have_condition()
            => Assert.Single(_result.Item2);
    }

    
    
}