using Coevery.Environment.Extensions.Models;
using Coevery.Security.Permissions;
using System.Collections.Generic;

namespace Coevery.Translations
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ImportExport = new Permission { Description = "Import / Export translations", Name = "Translations.Upload" };
        public static readonly Permission Translate = new Permission { Description = "Translate strings", Name = "Translation.Translate", ImpliedBy = new[] { ImportExport } };

        public virtual Feature Feature { get; set; }

        public IEnumerable<Permission> GetPermissions()
        {
            return new[] {
                Translate,
                ImportExport
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = GetPermissions()
                }
            };
        }
    }
}
