using System;
using SimpleGrind.Parameters;

namespace SimpleGrind.Loadtest
{
	public interface ILoadTest
	{
		LoadResult Run(int numberOfCalls, int wait, LogLevel logLevel);
	}
}