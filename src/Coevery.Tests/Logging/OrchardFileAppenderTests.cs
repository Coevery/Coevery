using System;
using log4net.Util;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Coevery.Logging;

namespace Coevery.Tests.Logging {
    [TestFixture]
    public class CoeveryFileAppenderTests {
        [Test]
        public void AddSuffixTest() {
            const string filename = "Coevery-debug";
            const string filenameAlternative1 = "Coevery-debug-1";
            const string filenameAlternative2 = "Coevery-debug-2";

            string filenameUsed = string.Empty;

            // Set logging to quiet mode
            LogLog.QuietMode = true;

            Mock<StubCoeveryFileAppender> firstCoeveryFileAppenderMock = new Mock<StubCoeveryFileAppender>();
            StubCoeveryFileAppender firstCoeveryFileAppender = firstCoeveryFileAppenderMock.Object;

            // Regular filename should be used if nothing is locked
            firstCoeveryFileAppenderMock.Protected().Setup("BaseOpenFile", ItExpr.Is<string>(file => file.Equals(filename)), ItExpr.IsAny<bool>()).Callback<string, bool>(
                    (file, append) => filenameUsed = file);

            firstCoeveryFileAppender.OpenFileStub(filename, true);

            Assert.That(filenameUsed, Is.EqualTo(filename));

            // Alternative 1 should be used if regular filename is locked

            firstCoeveryFileAppenderMock.Protected().Setup("BaseOpenFile", ItExpr.Is<string>(file => file.Equals(filename)), ItExpr.IsAny<bool>()).Throws(new Exception());
            firstCoeveryFileAppenderMock.Protected().Setup("BaseOpenFile", ItExpr.Is<string>(file => file.Equals(filenameAlternative1)), ItExpr.IsAny<bool>()).Callback<string, bool>(
                    (file, append) => filenameUsed = file);

            firstCoeveryFileAppender.OpenFileStub(filename, true);

            Assert.That(filenameUsed, Is.EqualTo(filenameAlternative1));

            // make alternative 1 also throw exception to make sure alternative 2 is used.
            firstCoeveryFileAppenderMock.Protected().Setup("BaseOpenFile", ItExpr.Is<string>(file => file.Equals(filenameAlternative1)), ItExpr.IsAny<bool>()).Throws(new Exception());
            firstCoeveryFileAppenderMock.Protected().Setup("BaseOpenFile", ItExpr.Is<string>(file => file.Equals(filenameAlternative2)), ItExpr.IsAny<bool>()).Callback<string, bool>(
                    (file, append) => filenameUsed = file);

            firstCoeveryFileAppender.OpenFileStub(filename, true);

            Assert.That(filenameUsed, Is.EqualTo(filenameAlternative2));
        }

        public class StubCoeveryFileAppender : CoeveryFileAppender {
            public void OpenFileStub(string fileName, bool append) {
                base.OpenFile(fileName, append);
            }
        }
    }
}
