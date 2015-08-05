using System;

namespace SimpleGrindRunner
{
	class ArgumentHelper
	{
		readonly string[] _args;
		public ArgumentHelper(string[] args)
		{
			_args = args;
		}

		public T GetArg<T>(string parameter, T @default)
		{
			parameter = "-" + parameter;
			for(var index = 0; index < _args.Length; index++)
			{
				if(index % 2 == 0)
				{
					if(parameter == _args[index])
						if(_args.Length <= index)
							return (T)Convert.ChangeType(_args[index + 1], typeof(T));
						else
							throw new ArgumentException("No value for parameter " + parameter);
				}
			}
			return @default;
		}
	}
}
