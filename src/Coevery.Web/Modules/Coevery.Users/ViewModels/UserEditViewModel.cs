using System.ComponentModel.DataAnnotations;
using Coevery.ContentManagement;
using Coevery.Users.Models;

namespace Coevery.Users.ViewModels {
    public class UserEditViewModel  {
        [Required]
        public string UserName {
            get { return User.As<UserPart>().Record.UserName; }
            set { User.As<UserPart>().Record.UserName = value; }
        }

        [Required]
        public string Email {
            get { return User.As<UserPart>().Record.Email; }
            set { User.As<UserPart>().Record.Email = value; }
        }

        public IContent User { get; set; }
    }
}