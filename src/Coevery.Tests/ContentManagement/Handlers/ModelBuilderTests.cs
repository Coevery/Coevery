using NUnit.Framework;
using Coevery.ContentManagement;
using Coevery.ContentManagement.Handlers;
using Coevery.ContentManagement.MetaData.Builders;
using Coevery.Tests.ContentManagement.Models;

namespace Coevery.Tests.ContentManagement.Handlers {
    [TestFixture]
    public class ModelBuilderTests {
        [Test]
        public void BuilderShouldReturnWorkingModelWithTypeAndId() {
            var builder = new ContentItemBuilder(new ContentTypeDefinitionBuilder().Named("foo").Build());
            var model = builder.Build();
            Assert.That(model.ContentType, Is.EqualTo("foo"));
        }

        [Test]
        public void IdShouldDefaultToZero() {
            var builder = new ContentItemBuilder(new ContentTypeDefinitionBuilder().Named("foo").Build());
            var model = builder.Build();
            Assert.That(model.Id, Is.EqualTo(0));
        }

        [Test]
        public void WeldShouldAddPartToModel() {
            var builder = new ContentItemBuilder(new ContentTypeDefinitionBuilder().Named("foo").Build());
            builder.Weld<AlphaPart>();
            var model = builder.Build();

            Assert.That(model.Is<AlphaPart>(), Is.True);
            Assert.That(model.As<AlphaPart>(), Is.Not.Null);
            Assert.That(model.Is<BetaPart>(), Is.False);
            Assert.That(model.As<BetaPart>(), Is.Null);
        }
    }
}

