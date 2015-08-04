using System;

namespace SimpleGrind.Loadtest
{
	public interface ILoadTest
	{
		LoadResult Run(int numberOfCalls, int wait, Action<LoadResult> callback);
	}
}