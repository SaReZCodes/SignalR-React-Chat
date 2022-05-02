
using Microsoft.EntityFrameworkCore;
using SRRC.Service.Repository;
using SRRC.DataLayer.Database;
using SRRC.DomainClasses.Entities.Authentication;
using SRRC.Service.IRepository.Authentication;

namespace SRRC.Service.IRepository.Authentication
{
    public interface IGroupRepository : IRepository<Group>
    {
        void Update(Group group);
    }
}

namespace SRRC.Service.Repository.Authentication
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        public GroupRepository(SRRCDbContext context) : base(context)
        {

        }
		
        public void Update(Group group)
        {
            Group selectedGroup = SRRCContext.Groups.Find(group.Id);
            selectedGroup.Title = group.Title;

        }

        public SRRCDbContext SRRCContext
        {
            get { return Context as SRRCDbContext; }
        }
    }
}
