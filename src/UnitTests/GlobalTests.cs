using System.IO;
using log4net;
using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTests.Utilities;

namespace UnitTests
{
    [TestClass]
    public class GlobalTests
    {
        private const string TestMessage1 = "TestMessage1";
        private const string TestMessage2 = "TestMessage2";

        public TestContext TestContext { get; set; }

        private static ListAppender _appender;

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            // make sure that the log4net.config file is deployed
            log4net.Config.XmlConfigurator.Configure(new FileInfo("log4net.config"));

            // the configuration should have exactly one appender of type ListAppender
            var appenders = LogManager.GetRepository().GetAppenders();
            Assert.AreEqual(1, appenders.Length);
            Assert.IsInstanceOfType(appenders[0], typeof(ListAppender));

            _appender = appenders[0] as ListAppender;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            _appender.Clear();
        }

        [TestMethod]
        public void TestLogging()
        {
            var log = LogManager.GetLogger("SomeLogger");

            log.Info(TestMessage1);

            var events = _appender.GetEvents();
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(TestMessage1, events[0].MessageObject);
            Assert.AreEqual(Level.Info, events[0].Level);

            log.Warn(TestMessage2);

            events = _appender.GetEvents();
            Assert.AreEqual(2, events.Count);
            Assert.AreEqual(TestMessage2, events[1].MessageObject);
            Assert.AreEqual(Level.Warn, events[1].Level);

            _appender.Clear();
            events = _appender.GetEvents();
            Assert.AreEqual(0, events.Count);
        }
    }
}
