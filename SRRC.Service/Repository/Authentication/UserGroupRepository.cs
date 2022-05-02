
using Microsoft.EntityFrameworkCore;
using SRRC.DataLayer.Database;
using System.Linq.Expressions;
using SRRC.DomainClasses.Entities.Authentication;
using SRRC.Service.IRepository.Authentication;
using SRRC.Service.Repository;

namespace SRRC.Service.IRepository.Authentication
{
    public interface IUserGroupRepository : IRepository<UserGroup>
    {
        void Update(UserGroup userGroup);
		IEnumerable<UserGroup> GetAllByFilter(Expression<Func<UserGroup, bool>> expression);
    }
}

namespace SRRC.Service.Repository.Authentication
{
    public class UserGroupRepository : Repository<UserGroup>, IUserGroupRepository
    {
        public UserGroupRepository(SRRCDbContext context) : base(context)
        {

        }
		
        public void Update(UserGroup userGroup)
        {
            SRRCContext.Update(userGroup);
        }
		
		public IEnumerable<UserGroup> GetAllByFilter(Expression<Func<UserGroup, bool>> expression)
        {
            var userGroups = Context.Set<UserGroup>().Where(expression);
			return userGroups;
        }

        public SRRCDbContext SRRCContext
        {
            get { return Context as SRRCDbContext; }
        }
    }
}
