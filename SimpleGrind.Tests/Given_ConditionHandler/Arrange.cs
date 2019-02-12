using CtorMock.Moq;
using SimpleGrind.Parameters;

namespace SimpleGrind.Tests.Given_ConditionHandler
{
    public abstract class Arrange : MockBase<ConditionHandler>
    {
        protected Arrange()
        {
            Mocker.MockOf<IRunnerParameters>()
                .Setup(s => s.ExitCondition)
                .Returns(ExitCondition);
        }

        protected virtual string ExitCondition { get; }
    }
}