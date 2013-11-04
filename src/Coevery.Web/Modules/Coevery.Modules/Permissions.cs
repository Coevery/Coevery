using System.Collections.Generic;
using Coevery.Environment.Extensions.Models;
using Coevery.Security.Permissions;

namespace Coevery.Modules {
    public class Permissions : IPermissionProvider {
        public static readonly Permission ManageFeatures = new Permission {Description = "Manage Features", Name = "ManageFeatures" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new[] {ManageFeatures};
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                             new PermissionStereotype {
                                                          Name = "Administrator",
                                                          Permissions = new[] {ManageFeatures}
                                                      }
                         };
        }
    }
}