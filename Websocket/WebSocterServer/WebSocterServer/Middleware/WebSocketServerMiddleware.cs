using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;

namespace WebSocketServer.Middleware
{
    public class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _Next;

        private readonly WebsocketServerConnectionManager _Manager;

        public WebSocketServerMiddleware(RequestDelegate next, WebsocketServerConnectionManager manager)
        {
            _Manager = manager;
            _Next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine($"Websocket Connected");

                string connId = _Manager.AddSocket(webSocket);

                await SendConnIdAsync(webSocket, connId);

                await ReceivedMessage(webSocket, (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        Console.WriteLine("Message Received");
                        Console.WriteLine($"Message: {Encoding.UTF8.GetString(buffer, 0, result.Count)}");
                        return;
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        Console.WriteLine("Received Close Message");
                        return;
                    }
                });
            }
            else
            {
                Console.WriteLine("Hello from the second request delegate");
                await _Next(context);
            }
        }

        public void WriteRequestParam(HttpContext context)
        {
            Console.WriteLine("request method " + context.Request.Method);
            Console.WriteLine("request protocol " + context.Request.Protocol);

            if (context.Request.Headers != null)
            {
                foreach (var header in context.Request.Headers)
                {
                    Console.WriteLine($"--> {header.Key}  value: {header.Value}");
                    
                }
            }
        }

        private static async Task ReceivedMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(buffer: new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);
            }
        }

        private async Task SendConnIdAsync(WebSocket socket, string connId)
        {
            var buffer = Encoding.UTF8.GetBytes($"ConnID: {connId}");

            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
