using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoConference.Web.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message, string room)
        {
            await Clients.Group(room).SendAsync("ReceiveMessage", user, message);
        }

        public async Task Join(string room)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, room);
        }
    }
}
