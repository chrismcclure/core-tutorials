using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace SignalRProject.Hubs
{
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {

            Console.WriteLine($"--> connected.  {Context.ConnectionId}");

            Clients.Client(Context.ConnectionId).SendAsync("ReceiveConnID", Context.ConnectionId);

            return base.OnConnectedAsync();
        }

        public async Task SendMessageAsync(string message)
        {
            var routeOb = JsonConvert.DeserializeObject<dynamic>(message);
            string toClient = routeOb.To;

            if (string.IsNullOrEmpty(toClient))
            {
                await Clients.All.SendAsync("ReceivedMessage", message);
            }
            else
            {
                await Clients.Client(toClient).SendAsync($"received message", message);
            }

            Console.WriteLine($"Message Received on {Context.ConnectionId}");
        }
    }
}
