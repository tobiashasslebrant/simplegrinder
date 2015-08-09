using System;
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
        public LoadResult Run(int numberOfCalls, int wait)
        {
            var result = new LoadResult();
            for (var index = 0; index < numberOfCalls; index++)
            {
                var t = _action();
                if (((int)t.StatusCode) < 400)
                    result.Ok++;
                else
                    result.Failed++;
                if (wait > 0)
                    Thread.Sleep(wait);
            };
            return result;
        }
    }

}
