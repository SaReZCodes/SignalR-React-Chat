namespace SRRC.DomainClasses.Entities.Authentication
{
    public class GroupRole
    {
        public int GroupId { get; set; }
        public int RoleId { get; set; }

        public virtual Group? User { get; set; }
        public virtual Role? Role { get; set; }
    }
}