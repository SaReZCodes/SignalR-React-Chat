using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRRC.DomainClasses.Entities.Chat
{
    public class Chat
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public ChatGroup ChatGroup  { get; set; }

    }
}
