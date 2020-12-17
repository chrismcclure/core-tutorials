using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;

namespace WebSocketServer.Middleware
{
    public class WebsocketServerConnectionManager
    {

        private readonly ConcurrentDictionary<string, WebSocket> _Sockets = new ConcurrentDictionary<string, WebSocket>();

        public ConcurrentDictionary<string, WebSocket> GetAllSockets()
        {
            return _Sockets;
        }

        public string AddSocket(WebSocket socket)
        {
            string connId = Guid.NewGuid().ToString();

            _Sockets.TryAdd(connId, socket);

            Console.WriteLine($"Connection {connId} added");
            
            return connId;

        }

    }
}
