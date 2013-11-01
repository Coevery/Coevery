using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using NUnit.Framework;
using Coevery.Caching;
using Coevery.Environment.Extensions.Models;
using Coevery.Roles.Models;
using Coevery.Roles.Services;
using Coevery.Security.Permissions;
using Coevery.Tests.Stubs;

namespace Coevery.Tests.Modules.Roles.Services {
    [TestFixture]
    public class RoleServiceTests : DatabaseEnabledTestsBase {
        public override void Register(ContainerBuilder builder) {
            builder.RegisterType<RoleService>().As<IRoleService>();
            builder.RegisterType<StubCacheManager>().As<ICacheManager>();
            builder.RegisterType<Signals>().As<ISignals>();
            builder.RegisterType<TestPermissionProvider>().As<IPermissionProvider>();
        }

        public class TestPermissionProvider : IPermissionProvider {
            public Feature Feature {
                get { return new Feature { Descriptor = new FeatureDescriptor { Id = "RoleServiceTests" } }; }
            }

            public IEnumerable<Permission> GetPermissions() {
                return "alpha,beta,gamma,delta".Split(',').Select(name => new Permission { Name = name });
            }

            public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
                return Enumerable.Empty<PermissionStereotype>();
            }
        }

        protected override IEnumerable<Type> DatabaseTypes {
            get {
                return new[] { typeof(RoleRecord), typeof(PermissionRecord), typeof(RolesPermissionsRecord) };
            }
        }

        [Test]
        public void CreateRoleShouldAddToList() {
            var service = _container.Resolve<IRoleService>();
            service.CreateRole("one");
            service.CreateRole("two");
            service.CreateRole("three");

            ClearSession();

            var roles = service.GetRoles();
            Assert.That(roles.Count(), Is.EqualTo(3));
            Assert.That(roles, Has.Some.Property("Name").EqualTo("one"));
            Assert.That(roles, Has.Some.Property("Name").EqualTo("two"));
            Assert.That(roles, Has.Some.Property("Name").EqualTo("three"));
        }

        [Test]
        public void PermissionChangesShouldBeVisibleImmediately() {

            var service = _container.Resolve<IRoleService>();

            ClearSession();
            {
                service.CreateRole("test");
                var roleId = service.GetRoleByName("test").Id;
                service.UpdateRole(roleId, "test", new[] { "alpha", "beta", "gamma" });
            }

            ClearSession();
            {
                var result = service.GetPermissionsForRoleByName("test");
                Assert.That(result.Count(), Is.EqualTo(3));
            }

            ClearSession();
            {
                var roleId = service.GetRoleByName("test").Id;
                service.UpdateRole(roleId, "test", new[] { "alpha", "beta", "gamma", "delta" });
            }

            ClearSession();
            {
                var result = service.GetPermissionsForRoleByName("test");
                Assert.That(result.Count(), Is.EqualTo(4));
            }
        }

        [Test]
        public void ShouldNotCreateARoleTwice() {
            var service = _container.Resolve<IRoleService>();
            service.CreateRole("one");
            service.CreateRole("two");
            service.CreateRole("one");

            ClearSession();

            var roles = service.GetRoles();
            Assert.That(roles.Count(), Is.EqualTo(2));
            Assert.That(roles, Has.Some.Property("Name").EqualTo("one"));
            Assert.That(roles, Has.Some.Property("Name").EqualTo("two"));
        }
    }
}
