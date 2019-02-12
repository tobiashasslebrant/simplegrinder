using System;
using SimpleGrind.Parameters;

namespace SimpleGrind.LoadTest
{
	public interface ILoadTest
	{
		LoadResult Run(int numberOfCalls, int wait, LogLevel logLevel);
	}
}