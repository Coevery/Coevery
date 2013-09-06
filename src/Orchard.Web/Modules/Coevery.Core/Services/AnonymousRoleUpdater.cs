using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Orchard.Data;
using Orchard.Environment;
using Orchard.Environment.Extensions.Models;
using Orchard.Logging;
using Orchard.Roles.Models;
using Orchard.Roles.Services;
using Orchard.Security;
using Orchard.Security.Permissions;

namespace Orchard.Roles {
    [UsedImplicitly]
    public class AnonymousRoleUpdater : IFeatureEventHandler {
        private readonly IRepository<RolesPermissionsRecord> _permissionRepository;

        public AnonymousRoleUpdater(IRepository<RolesPermissionsRecord> permissionRepository) {
            _permissionRepository = permissionRepository;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public Feature Feature { get; set; }

        void IFeatureEventHandler.Installing(Feature feature) {
        }

        void IFeatureEventHandler.Installed(Feature feature) {
            if (feature == Feature) {
                RemoveAnonymousFrontEndPermission();
            }
        }

        void IFeatureEventHandler.Enabling(Feature feature) {
        }

        void IFeatureEventHandler.Enabled(Feature feature) {
        }

        void IFeatureEventHandler.Disabling(Feature feature) {
        }

        void IFeatureEventHandler.Disabled(Feature feature) {
        }

        void IFeatureEventHandler.Uninstalling(Feature feature) {
        }

        void IFeatureEventHandler.Uninstalled(Feature feature) {
        }

        private void RemoveAnonymousFrontEndPermission() {

            var permissionRecord = _permissionRepository.Get(x =>
                x.Permission.Name == StandardPermissions.AccessFrontEnd.Name && x.Role.Name == "Anonymous");

            _permissionRepository.Delete(permissionRecord);
        }
    }
}
