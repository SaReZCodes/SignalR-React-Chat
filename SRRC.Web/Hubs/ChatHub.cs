using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace SRRC.Web.Hubs
{
    [AllowAnonymous]
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
           // Clients.All.SendAsync("Wellcome", "Hello","test", 25);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
