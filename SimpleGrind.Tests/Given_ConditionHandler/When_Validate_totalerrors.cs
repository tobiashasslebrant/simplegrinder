using System.Linq;
using SimpleGrind.Model;
using Xunit;

namespace SimpleGrind.Tests.Given_ConditionHandler
{
    public class When_Validate_totalerrors_largerthan : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "totalerrors>0";

        public When_Validate_totalerrors_largerthan()
        {
            var runResult = new RunResult();
            runResult.Errors.AddRange(Enumerable.Repeat("error", 1));
            _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults = {runResult}
            });
        }

        [Fact]
        public void Should_have_condition()
            => Assert.Single(_result.Item2);
    }
    
    public class When_Validate_totalerrors_lowerthan : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "totalerrors<5";

        public When_Validate_totalerrors_lowerthan()
        {
            var runResult = new RunResult();
            runResult.Errors.AddRange(Enumerable.Repeat("error", 1));
            _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults = {runResult}
            });
        }

        [Fact]
        public void Should_have_condition()
            => Assert.Single(_result.Item2);
    }
    public class When_Validate_totalerrors_percentage : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "totalerrors%49";

        public When_Validate_totalerrors_percentage()
        {
            var runResult = new RunResult();
            runResult.Errors.AddRange(Enumerable.Repeat("error", 5));
            _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults = {runResult},
                TotalCalls = 10
                
            });
        }

        [Fact]
        public void Should_have_condition()
            => Assert.Single(_result.Item2);
    }
    
    public class When_Validate_totalerrors_equal : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "totalerrors=5";

        public When_Validate_totalerrors_equal()
        {
            var runResult = new RunResult();
            runResult.Errors.AddRange(Enumerable.Repeat("error", 5));
            _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults = {runResult},
            });
        }

        [Fact]
        public void Should_have_condition()
            => Assert.Single(_result.Item2);
    }
    
    public class When_Validate_totalerrors_notequal : Arrange
    {
        private readonly (bool, string[]) _result;
        protected override string ExitCondition => "totalerrors!5";

        public When_Validate_totalerrors_notequal()
        {
            var runResult = new RunResult();
            runResult.Errors.AddRange(Enumerable.Repeat("error", 4));
            _result = Subject.Validate(new AggregatedRunResult
            {
                RunResults = {runResult},
            });
        }

        [Fact]
        public void Should_have_condition()
            => Assert.Single(_result.Item2);
    }
}