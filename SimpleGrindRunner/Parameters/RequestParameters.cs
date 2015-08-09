using System.Collections.Generic;
using System.Linq;
using SimpleGrind.Extensions;

namespace SimpleGrind.Runner.Parameters
{
    public class RequestParameters
    {
        public RequestParameters(ParameterBuilder parameterBuilder)
        {
            parameterBuilder.MapByIndex<string>(0, val => Method = val);
            parameterBuilder.MapByIndex<string>(1, val => Url = val);
            parameterBuilder.MapByArg<string>("h", val => Headers = val.ToDictionary(';', '='));
            parameterBuilder.MapByArg<string>("c", val => Cookies = val.ToDictionary(';', '='));
            parameterBuilder.MapByArg<string>("j", val => Json = val);
            parameterBuilder.MapByArg<int>("t", val => TimeOut = val);
        }

        public string Method { get; private set; } = "get";
        public string Url { get; private set; } = "http://localhost";
        public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Cookies { get; private set; } = new Dictionary<string, string>();
        public string Json { get; private set; } = "";
        public int TimeOut { get; private set; } = 5;
    }
}
