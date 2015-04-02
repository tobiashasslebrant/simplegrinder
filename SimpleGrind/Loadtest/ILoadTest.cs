using System;

namespace SimpleGrind.Loadtest
{
	public interface ILoadTest
	{
		LoadResult Run(int numberOfCalls, Action<LoadResult> callback, int wait);
	}
}