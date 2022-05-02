
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using SRRC.DomainClasses.Entities.Authentication;

namespace SRRC.Model
{
    public class LoginUserModel : CaptchaBase
    {
        public LoginUserModel()
        {
            UserRoles = new HashSet<UserRole>();
            UserTokens = new HashSet<UserToken>();
            UserGroups = new HashSet<UserGroup>();
        }

        public int Id { get; set; }

        [DisplayName("نام کاربری")]
        public string Username { get; set; }

        [DisplayName("کلمه عبور")]
        public string Password { get; set; }

        [DisplayName("نام")]
        public string DisplayName { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset? LastLoggedIn { get; set; }

        [DisplayName("شماره سریال")]
        public string SerialNumber { get; set; }

        [JsonIgnore] public virtual ICollection<UserRole> UserRoles { get; set; }

        [JsonIgnore] public virtual ICollection<UserToken> UserTokens { get; set; }

        [JsonIgnore] public ICollection<UserGroup> UserGroups { get; set; }
    }
}
