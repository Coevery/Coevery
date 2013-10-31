using Coevery.Security;

namespace Coevery.Users.Events {
    public class UserContext {
        public IUser User { get; set; }
        public bool Cancel { get; set; }
        public CreateUserParams UserParameters { get; set; }
    }
}