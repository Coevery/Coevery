using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Coevery.Data;
using Coevery.Events;
using Coevery.Roles.Models;

namespace Coevery.Roles.ImportExport {
    public interface IExportEventHandler : IEventHandler {
        void Exporting(dynamic context);
        void Exported(dynamic context);
    }

    public class RolesExportEventHandler : IExportEventHandler {
        private readonly IRepository<RoleRecord> _roleRecordepository;

        public RolesExportEventHandler(IRepository<RoleRecord> roleRecordRepository) {
            _roleRecordepository = roleRecordRepository;
        }

        public void Exporting(dynamic context) {}

        public void Exported(dynamic context) {

            if (!((IEnumerable<string>) context.ExportOptions.CustomSteps).Contains("Roles")) {
                return;
            }

            var roles = _roleRecordepository.Table.ToList();

            if (!roles.Any()) {
                return;
            }

            var root = new XElement("Roles");
            context.Document.Element("Coevery").Add(root);

            foreach (var role in roles) {
                root.Add(new XElement("Role",
                                      new XAttribute("Name", role.Name),
                                      new XAttribute("Permissions", string.Join(",", role.RolesPermissions.Select(rolePermission => rolePermission.Permission.Name)))));
            }
        }
    }
}