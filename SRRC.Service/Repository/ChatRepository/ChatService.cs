
using Microsoft.EntityFrameworkCore;
using SRRC.Service.Repository;
using SRRC.DataLayer.Database;
using SRRC.DomainClasses.Entities.Authentication;
using SRRC.Service.IRepository.Authentication;
using SRRC.Service.IRepository.ChatRepository;

namespace SRRC.Service.IRepository.ChatRepository
{
    public interface IChatService : IRepository<Group>
    {
        void Update(Group group);
    }
}

namespace SRRC.Service.Repository.ChatRepository
{
    public class ChatService : Repository<Group>, IChatService
    {
        public ChatService(SRRCDbContext context) : base(context)
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
