using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using log4net;
using log4net.Core;

namespace UnitTests.Utilities
{
    public static class LoggerUtil
    {
        private static ListAppender _listAppender;

        public static void InitializeLog4Net()
        {
            // make sure that the log4net.config file is deployed
            log4net.Config.XmlConfigurator.Configure(new FileInfo("TestFiles/log4net.config"));

            // the configuration should have exactly one appender of type ListAppender
            var appenders = LogManager.GetRepository().GetAppenders();
            Assert.AreEqual(1, appenders.Length);
            Assert.IsInstanceOf<ListAppender>(appenders[0]);

            _listAppender = appenders[0] as ListAppender;
        }

        public static IList<LoggingEvent> GetEvents()
        {
            return _listAppender.GetEvents();
        }

        public static void ClearEvents()
        {
            _listAppender.Clear();
        }
    }
}
