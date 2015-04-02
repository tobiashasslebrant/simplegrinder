using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleGrindRunner
{
	class ArgumentHelper
	{
		string[] _args;
		public ArgumentHelper(string[] args)
		{
			_args = args;
		}

		public T GetArg<T>(string parameter, T @default)
		{
			for(var index = 0; index < _args.Length; index++)
			{
				if(index % 2 == 0)
				{
					if(parameter == _args[index])
						return (T)Convert.ChangeType(_args[index + 1], typeof(T));
				}
			}
			return @default;
		}
	}
}
