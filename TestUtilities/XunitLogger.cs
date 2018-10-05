using Microsoft.Extensions.Logging;
using System;
using Xunit.Abstractions;

namespace Webcrm.ErpIntegrations.Test
{
    public class XunitLogger : ILogger, IDisposable
    {
        public XunitLogger(ITestOutputHelper output)
        {
            Output = output;
        }

        private ITestOutputHelper Output { get; }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Output.WriteLine(state.ToString());
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        { }
    }
}