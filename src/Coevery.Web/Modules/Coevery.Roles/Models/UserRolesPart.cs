using System.Collections.Generic;
using Coevery.ContentManagement;

namespace Coevery.Roles.Models {
    public class UserRolesPart : ContentPart, IUserRoles {
        public UserRolesPart() {
            Roles = new List<string>();
        }

        public IList<string> Roles { get; set; }
    }
}