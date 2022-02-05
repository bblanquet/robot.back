using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bob.raspberry.api.Core
{
    public class BobSocket : IBobSocket
    {
        private const int defaultSize = 1024 * 4;
        private WebSocket Ws;
        public event EventHandler<Byte[]> OnMessageReceived;
        public event EventHandler OnClose;
        private Byte[] Message = new Byte[0];

        public bool IsConnected()
        {
            return Ws != null && Ws.State == WebSocketState.Open;
        }


        public async Task Start(WebSocket webSocket)
        {
            if (IsConnected())
            {
                throw new Exception();
            }
            else
            {
                Ws = webSocket;
                await LifeCycle(webSocket);
            }
        }

        private async Task LifeCycle(WebSocket webSocket)
        {
            var buffer = new byte[defaultSize];
            WebSocketReceiveResult result;
            do
            {
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                HandleMessage(result, buffer);
            } while (!result.CloseStatus.HasValue);

            _ = webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
            Ws = null;
            OnClose?.Invoke(null,new EventArgs());
        }

        private void HandleMessage(WebSocketReceiveResult result, byte[] fragment)
        {
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var frag = fragment.Take(result.Count).ToArray();
                Message = Message.Concat(frag).ToArray();
                if (result.EndOfMessage)
                {
                    OnMessageReceived?.Invoke(null, Message);
                    Message = new Byte[0];
                }
            }
        }

        public async Task Send(byte[] messages, WebSocketMessageType type = WebSocketMessageType.Text)
        {
            if (IsConnected())
            {
                await Ws.SendAsync(new ArraySegment<Byte>(messages), WebSocketMessageType.Text, true, new CancellationToken());
            }
        }
        public async Task Send(string message, WebSocketMessageType type = WebSocketMessageType.Text)
        {
            await Ws.SendAsync(new ArraySegment<Byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text, true, new CancellationToken());
        }

        public void Close()
        {
            if (Ws != null) {
                _ = Ws.CloseAsync(WebSocketCloseStatus.NormalClosure,String.Empty,new CancellationToken());
                OnClose?.Invoke(null, new EventArgs());
            }
        }
    }
}
