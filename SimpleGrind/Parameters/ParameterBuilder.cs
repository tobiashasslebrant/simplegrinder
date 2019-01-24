using System;

namespace SimpleGrind.Parameters
{
    public class ParameterBuilder
    {
        private readonly string[] _args;
        private readonly char _seperator;

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
        public void MapByArg<T>(string parameter, Action<T> map) where T : struct
        {
            for (var index = 0; index < _args.Length; index++)
            {
                if (_seperator + parameter == _args[index])
                    try
                    {
                        if (typeof(T).IsEnum)
                        {
                            map((T) Enum.Parse<T>(_args[index + 1], true));
                        }
                        else
                        {
                            map((T) Convert.ChangeType(_args[index + 1], typeof(T)));
                        }
                       
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new Exception($"No value for option {parameter}");
                    }
            }
        }
    }
}
