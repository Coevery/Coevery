using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Coevery.CoeveryNavigation
{
    public class Permissions : IPermissionProvider {
        public static readonly Permission ManageMainMenu = new Permission { Name = "ManageMainMenu", Description = "Manage main menu" };
        public static readonly Permission PublishContent = new Permission { Description = "Publish or unpublish content for others", Name = "PublishContent" };
        public static readonly Permission EditContent = new Permission { Description = "Edit content for others", Name = "EditContent", ImpliedBy = new[] { PublishContent } };
        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions() {
            return new [] {
                PublishContent,
                EditContent,
                ManageMainMenu
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = GetPermissions()
                }
            };
        }
    }
}
