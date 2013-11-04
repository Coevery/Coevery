using NUnit.Framework;
using Coevery.Environment.ShellBuilders.Models;
using Coevery.Tests.Records;

namespace Coevery.Tests.Data.Builders {
    [TestFixture]
    public class SessionFactoryBuilderTests {
        [Test]
        public void SqlCeSchemaShouldBeGeneratedAndUsable() {

            var recordDescriptors = new[] {
                                              new RecordBlueprint {TableName = "Hello", Type = typeof (FooRecord)}
                                          };

            ProviderUtilities.RunWithSqlCe(recordDescriptors,
                sessionFactory => {
                    var session = sessionFactory.OpenSession();
                    var foo = new FooRecord { Name = "hi there" };
                    session.Save(foo);
                    session.Flush();
                    session.Close();

                    Assert.That(foo, Is.Not.EqualTo(0));

                });

        }

        [Test]
        public void SqlServerSchemaShouldBeGeneratedAndUsable() {
            var recordDescriptors = new[] {
                                              new RecordBlueprint {TableName = "Hello", Type = typeof (FooRecord)}
                                          };

            ProviderUtilities.RunWithSqlServer(recordDescriptors,
                sessionFactory => {
                    var session = sessionFactory.OpenSession();
                    var foo = new FooRecord { Name = "hi there" };
                    session.Save(foo);
                    session.Flush();
                    session.Close();

                    Assert.That(foo, Is.Not.EqualTo(0));

                });
        }
    }
}