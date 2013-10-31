using System.Collections.Generic;
using Coevery.Environment.Extensions.Models;
using Coevery.Security.Permissions;

namespace Coevery.Packaging {
    public class Permissions : IPermissionProvider {
        public static readonly Permission ManagePackages = new Permission { Description = "Manage packages", Name = "ManagePackages" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new[] { ManagePackages };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            // By default no one can manage packages except the default site administrator
            return new List<PermissionStereotype>();
        }
    }
}