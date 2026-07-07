using Microsoft.AspNetCore.SignalR;

namespace EMK.Save.API.Hubs
{
    public class SaveHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            // BL Calls!

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
