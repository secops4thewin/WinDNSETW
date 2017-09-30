using System;
using O365.Security.ETW;

namespace hiddentreasure_etw_demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var filter = new EventFilter(Filter
                .EventIdIs(3018)
                .Or(Filter.EventIdIs(3020)));

            filter.OnEvent += (IEventRecord r) => {
                var query = r.GetUnicodeString("QueryName");
                var result = r.GetUnicodeString("QueryResults");
                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                int secondsSinceEpoch = (int)t.TotalSeconds;
                Console.WriteLine($"{secondsSinceEpoch} | {r.Id} | {query} | {result}");
            };

            var provider = new Provider("Microsoft-Windows-DNS-Client");
            provider.AddFilter(filter);

            var trace = new UserTrace();
            trace.Enable(provider);

            Console.CancelKeyPress += (sender, eventArg) =>
            {
                if (trace != null)
                {
                    trace.Stop();
                }
            };
            trace.Start();

            
        }
    }
}