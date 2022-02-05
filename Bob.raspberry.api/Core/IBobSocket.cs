using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Bob.raspberry.api.Core
{
    public interface IBobSocket
    {
        bool IsConnected();
        event EventHandler<byte[]> OnMessageReceived;
        public event EventHandler OnClose;
        void Close();
        Task Send(byte[] message, WebSocketMessageType type = WebSocketMessageType.Text);
        Task Send(string message, WebSocketMessageType type = WebSocketMessageType.Text);
        Task Start(WebSocket webSocket);
    }
}
