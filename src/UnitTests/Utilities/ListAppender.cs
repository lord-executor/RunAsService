using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Appender;
using log4net.Core;

namespace UnitTests.Utilities
{
    class ListAppender : AppenderSkeleton
    {
        private readonly List<LoggingEvent> _events;

        public ListAppender()
        {
            _events = new List<LoggingEvent>();
        }
 
        protected override void Append(LoggingEvent loggingEvent)
        {
            lock (_events)
            {
                _events.Add(loggingEvent);
            }
        }

        public void Clear()
        {
            lock (_events)
            {
                _events.Clear();
            }
        }

        public IList<LoggingEvent> GetEvents()
        {
            lock (_events)
            {
                return new List<LoggingEvent>(_events);
            }
        }
    }
}
