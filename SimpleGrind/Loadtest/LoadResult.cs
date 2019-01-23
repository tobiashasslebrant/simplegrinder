using System;
using System.Collections.Generic;

namespace SimpleGrind.Loadtest
{
	public class LoadResult
	{
		public int Ok;
		public int Failed;
		public IReadOnlyCollection<string> Errors;
	}
}