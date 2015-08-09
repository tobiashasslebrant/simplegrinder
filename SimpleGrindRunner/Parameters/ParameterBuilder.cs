using System;

namespace SimpleGrind.Runner.Parameters
{
    public class ParameterBuilder
    {
        private string[] _args;
        private char _seperator;

        public ParameterBuilder(string[] args, char seperator)
        {
            _args = args;
            _seperator = seperator;
        }

        public void MapByIndex<T>(int index, Action<T> map)
        {
            if(index < _args.Length)
                map((T)Convert.ChangeType(_args[index], typeof(T)));
        }
        public void MapByArg<T>(string parameter, Action<T> map)
        {
            for (var index = 0; index < _args.Length; index++)
            {
                if (index % 2 == 0)
                {
                    if (_seperator + parameter == _args[index])
                        if (_args.Length <= index)
                            map((T)Convert.ChangeType(_args[index + 1], typeof(T)));
                        else
                            throw new ArgumentException("No value for parameter " + parameter);
                }
            }
        }
    }
}
