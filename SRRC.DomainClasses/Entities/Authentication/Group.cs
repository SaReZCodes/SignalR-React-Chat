using System.Collections.Generic;

namespace SRRC.DomainClasses.Entities.Authentication
{
    public class Group
    {
        public Group()
        {
            GroupRoles = new HashSet<GroupRole>();
            UserGroups = new HashSet<UserGroup>();
        }

        public int Id { get; set; }
        public string? Title { get; set; }

        public virtual ICollection<GroupRole> GroupRoles { get; set; }
        public virtual ICollection<UserGroup> UserGroups { get; set; }
    }
}