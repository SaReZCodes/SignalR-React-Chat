using System.Collections.Generic;

namespace SRRC.DomainClasses.Entities.Authentication
{
    public class Role
    {
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
            GroupRoles = new HashSet<GroupRole>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Title { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<GroupRole> GroupRoles { get; set; }
    }
}