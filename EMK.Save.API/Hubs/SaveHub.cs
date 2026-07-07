using Microsoft.AspNetCore.SignalR;

namespace EMK.Save.API.Hubs
{
    public class SaveHub : Hub
    {
        public static string BudgetGroup(Guid sharedBudgetId) => $"budget-{sharedBudgetId}";

        /// <summary>Client calls this after connecting so it receives real-time events for its SharedBudget.</summary>
        public async Task JoinBudget(Guid sharedBudgetId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, BudgetGroup(sharedBudgetId));
        }

        public async Task LeaveBudget(Guid sharedBudgetId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, BudgetGroup(sharedBudgetId));
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
