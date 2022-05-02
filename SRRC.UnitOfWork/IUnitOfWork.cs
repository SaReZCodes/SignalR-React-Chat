using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SRRC.Service.IRepository.Authentication;
using SRRC.Service.IRepository.ChatRepository;

namespace SRRC.UOW
{
    public interface IUnitOfWork
    {
       
        public IGroupRepository Groups { get; }
        public IUserGroupRepository UserGroups { get; }
        public IChatService ChatService { get; }
        public IChatGroupService ChatGroupService { get; }

        DatabaseFacade Database { set; get; }

        Task<int> SaveChangesAsync();
    }
}