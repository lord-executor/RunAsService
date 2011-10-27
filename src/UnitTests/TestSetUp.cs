using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnitTests.Utilities;

namespace UnitTests
{
    [SetUpFixture]
    public class TestSetUp
    {
        [SetUp]
        public void Initialize()
        {
            LoggerUtil.InitializeLog4Net();
        }

        [TearDown]
        public void Shutdown()
        {
        }
    }
}
