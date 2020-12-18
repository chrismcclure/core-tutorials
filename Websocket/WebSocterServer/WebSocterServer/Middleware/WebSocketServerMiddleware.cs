using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

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

                await ReceivedMessage(webSocket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        Console.WriteLine("Message Received");
                        Console.WriteLine($"Message: {Encoding.UTF8.GetString(buffer, 0, result.Count)}");
                        await RouteJsonMessageAsync(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        var id = _Manager.GetAllSockets().FirstOrDefault(x => x.Key == connId).Key;

                        Console.WriteLine("Received Close Message");

                        _Manager.GetAllSockets().TryRemove(id, out var socket);

                        if (result.CloseStatus != null)
                            await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription,
                                CancellationToken.None);
                    }
                });
            }
            else
            {
                Console.WriteLine("Hello from the second request delegate");
                await _Next(context);
            }
        }

        public async Task RouteJsonMessageAsync(string message)
        {
            var routeOb = JsonConvert.DeserializeObject<dynamic>(message);

            if (Guid.TryParse(routeOb.To.ToString(), out Guid _))
            {
                Console.WriteLine("Targeted");
                var sock = _Manager.GetAllSockets();

                var goodSocket = sock.FirstOrDefault(x => x.Key == routeOb.To.ToString());


                if (goodSocket.Value != null)
                {
                    if (goodSocket.Value.State == WebSocketState.Open )
                    {
                        await goodSocket.Value.SendAsync(Encoding.UTF8.GetBytes(routeOb.Message.ToString()),
                            WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else
                    {
                    }
                }
                else
                {
                    Console.WriteLine("Invalid receptient");
                }
               
            }
            else
            {
                Console.WriteLine("Broadcast");
                foreach (System.Collections.Generic.KeyValuePair<string, WebSocket> sock in _Manager.GetAllSockets())
                {
                    if (sock.Value.State == WebSocketState.Open)
                    {
                        await sock.Value.SendAsync(Encoding.UTF8.GetBytes(routeOb.Message.ToString()),
                            WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
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

        private static async Task SendConnIdAsync(WebSocket socket, string connId)
        {
            var buffer = Encoding.UTF8.GetBytes($"ConnID: {connId}");

            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
