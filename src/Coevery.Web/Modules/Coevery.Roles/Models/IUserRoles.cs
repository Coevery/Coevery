using System.Collections.Generic;
using Coevery.ContentManagement;

namespace Coevery.Roles.Models {
    public interface IUserRoles : IContent {
        IList<string> Roles { get; }
    }
}