using NUnit.Framework;
using Coevery.Localization;
using Coevery.UI.Notify;

namespace Coevery.Tests.UI.Notify {
    [TestFixture]
    public class NotifierTests {

        [Test]
        public void MessageServiceCanAccumulateWarningsAndErrorsToReturn() {
            INotifier notifier = new Notifier();
            Localizer T = NullLocalizer.Instance;

            notifier.Warning(T("Hello world"));
            notifier.Information(T("More Info"));
            notifier.Error(T("Boom"));

            Assert.That(notifier.List(), Has.Count.EqualTo(3));
            Assert.That(notifier.List(), Has.Some.Property("Message").EqualTo(T("Hello world")));
            Assert.That(notifier.List(), Has.Some.Property("Message").EqualTo(T("More Info")));
            Assert.That(notifier.List(), Has.Some.Property("Message").EqualTo(T("Boom")));
        }
    }
}