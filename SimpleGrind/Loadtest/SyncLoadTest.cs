using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;

namespace SimpleGrind.Loadtest
{
    public class SyncLoadTest : ILoadTest
    {
        readonly Func<HttpResponseMessage> _action;

        public SyncLoadTest(Func<HttpResponseMessage> action)
        {
            _action = action;
        }
        public LoadResult Run(int numberOfCalls, int wait, string logLevel)
        {
            var result = new LoadResult();
            var errors = new List<string>();

            for (var index = 0; index < numberOfCalls; index++)
            {
                var t = _action();
                if (((int)t.StatusCode) < 400)
                    result.Ok++;
                else
                {
                    result.Failed++;
                    if(logLevel == "VERBOSE")
                     errors.Add(t.Content.ReadAsStringAsync().Result);
                }
                if (wait > 0)
                    Thread.Sleep(wait);
            };
            result.Errors = errors;
            return result;
        }
    }

}
