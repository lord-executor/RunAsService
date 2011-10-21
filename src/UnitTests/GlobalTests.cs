﻿using System.IO;
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

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            LoggerUtil.InitializeLog4Net();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            LoggerUtil.ClearEvents();
        }

        [TestMethod]
        public void TestLogging()
        {
            var log = LogManager.GetLogger("SomeLogger");

            log.Info(TestMessage1);

            var events = LoggerUtil.GetEvents();
            Assert.AreEqual(1, events.Count);
            Assert.AreEqual(TestMessage1, events[0].MessageObject);
            Assert.AreEqual(Level.Info, events[0].Level);

            log.Warn(TestMessage2);

            events = LoggerUtil.GetEvents();
            Assert.AreEqual(2, events.Count);
            Assert.AreEqual(TestMessage2, events[1].MessageObject);
            Assert.AreEqual(Level.Warn, events[1].Level);

            LoggerUtil.ClearEvents();
            events = LoggerUtil.GetEvents();
            Assert.AreEqual(0, events.Count);
        }
    }
}
