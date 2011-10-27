using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RunAsService;
using UnitTests.Utilities;
using log4net;
using System.Threading;
using log4net.Core;
using UnitTests.Application.Strings;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class ChildProcessWrapperTests
    {
        public TestContext TestContext { get; set; }
        [Test]
        public void TestInitialize()
        {
            LoggerUtil.ClearEvents();
        }

        [Test]
        public void TestTerminatingNormally()
        {
            var events = RunTestApplication("TestTerminatingNormally");
            var fullLog = BuildFullLog(events);

            Assert.AreEqual(Common.ServiceMessage, events[2].RenderedMessage, fullLog);
            Assert.AreEqual(Common.ShutDown, events[3].RenderedMessage, fullLog);
            Assert.IsTrue(events[4].RenderedMessage.Contains("exited with exit code"), fullLog);

            Assert.AreEqual(5, events.Count, "Expected 5 log messages: {0}", fullLog);
        }

        [Test]
        public void TestExceptionTerminating()
        {
            var events = RunTestApplication("TestExceptionTerminating");
            var fullLog = BuildFullLog(events);

            Assert.AreEqual(Common.ServiceMessage, events[2].RenderedMessage, fullLog);
            Assert.AreEqual(Common.ExceptionMessage, events[3].RenderedMessage, fullLog);
        }

        private string BuildFullLog(IList<LoggingEvent> events)
        {
            return String.Concat(Environment.NewLine, String.Join(Environment.NewLine, events.Select(e => e.RenderedMessage)));
        }

        private IList<LoggingEvent> RunTestApplication(string testMethod)
        {
            var log = LogManager.GetLogger("ChildProcess");
            var settings = new ChildProcessSettings
            {
                FileName = @"TestFiles\TestApplication.exe",
                WorkingDirectory = @".\",
                Arguments = testMethod,
            };
            var wrapper = new ChildProcessWrapper(log, settings);

            Assert.IsTrue(wrapper.Start(), "Unable to start TestApplication.exe");
            Assert.IsTrue(wrapper.WaitForExit(1000), "Test process did not terminate within 1s");
            wrapper.Terminate();

            var events = LoggerUtil.GetEvents();
            CheckApplicationStart(events);

            return events;
        }

        private void CheckApplicationStart(IList<LoggingEvent> events)
        {
            var errorMessage = String.Format("Log message mismatch:{0}", BuildFullLog(events));

            Assert.IsTrue(events[0].RenderedMessage.Contains("TestApplication.exe"), errorMessage);
            Assert.IsTrue(events[0].RenderedMessage.Contains("spawned"), errorMessage);

            Assert.AreEqual(Common.StartUp, events[1].RenderedMessage, errorMessage);
        }
    }
}
