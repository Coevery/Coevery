using System.Collections.Generic;
using Coevery.Environment.Extensions.Models;
using Coevery.Security.Permissions;

namespace Coevery.Core.Navigation {
    public class Permissions : IPermissionProvider {
        public static readonly Permission ManageMainMenu = new Permission { Name = "ManageMainMenu", Description = "Manage main menu" };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                ManageMainMenu
             };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManageMainMenu}
                }
            };
        }
    }
}
