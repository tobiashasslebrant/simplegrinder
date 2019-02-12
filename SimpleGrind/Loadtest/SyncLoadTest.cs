using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SimpleGrind.Parameters;

namespace SimpleGrind.LoadTest
{
    public class SyncLoadTest : ILoadTest
    {
        readonly Func<HttpResponseMessage> _action;

        public SyncLoadTest(Func<HttpResponseMessage> action)
        {
            _action = action;
        }
        public LoadResult Run(int numberOfCalls, int wait, LogLevel logLevel)
        {
            var result = new LoadResult();
            var errors = new List<string>();

            for (var index = 0; index < numberOfCalls; index++)
            {
                try
                {
                    var t = _action();
                    if (((int) t.StatusCode) < 400)
                        result.Ok++;
                    else
                    {
                        result.Failed++;
                        if (logLevel == LogLevel.Verbose)
                            errors.Add(t.Content.ReadAsStringAsync().Result);
                    }

                    if (wait > 0)
                        Thread.Sleep(wait);
                }
                catch (AggregateException ex)
                {
                    if(ex.InnerException is TaskCanceledException)
                        result.TimedOut++;
                    else
                        throw;
                }
                catch (Exception e)
                {
                    result.Failed++;
                    if (logLevel == LogLevel.Verbose)
                        errors.Add(e.ToString());
                }
               
            };
            result.Errors = errors;
            return result;
        }
    }

}
