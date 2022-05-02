using SRRC.DomainClasses.Entities.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRRC.DomainClasses.Entities.Chat
{
    public class ChatGroup
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string GroupToken { get; set; }
        public int OwnerId { get; set; }
        public DateTime CreateDate { get; set; }
        public User Owner { get; set; }
        public ICollection<Chat> Chats { get; set; }
    }
}
