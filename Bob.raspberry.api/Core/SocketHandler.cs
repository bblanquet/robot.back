using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Bob.raspberry.api.Core
{
    public static class SocketHandler
    {
        private static IBobSocket RaspberrySocket = null;
        private static IBobSocket StreamingSocket = null;
        private const string STREAM_ON = "STREAM_ON";
        private const string STREAM_OFF = "STREAM_OFF";
        public async static Task AddRobotSocket(WebSocket websocket){
            RaspberrySocket = new BobSocket();
            await RaspberrySocket.Start(websocket);
        }

        public async static Task AddStreamingSocket(WebSocket websocket)
        {
            if (IsRobotConnected())
            {
                if (StreamingSocket != null) {
                    StreamingSocket.Close();
                }

                StreamingSocket = new BobSocket();
                RaspberrySocket.OnMessageReceived += HandleReceivedMessage;
                StreamingSocket.OnClose += HandleStreamClose;
                Debug.WriteLine("STREAM START");
                await RaspberrySocket.Send(Encoding.UTF8.GetBytes(STREAM_ON));
                await StreamingSocket.Start(websocket);
            }
            else
            {
                throw new Exception("Robot is not connected.");
            }
        }

        public static bool IsRobotConnected()
        {
            return RaspberrySocket != null && RaspberrySocket.IsConnected();
        }

        public static IBobSocket GetRobotSocket() {
            return RaspberrySocket;
        }
        private async static void HandleStreamClose(object obj, object message)
        {
            Debug.WriteLine("STREAM CLOSE");
            StreamingSocket.OnClose -= HandleStreamClose;
            RaspberrySocket.OnMessageReceived -= HandleReceivedMessage;
            await RaspberrySocket.Send(Encoding.UTF8.GetBytes(STREAM_OFF));
        }

        private static void HandleReceivedMessage(object obj, byte[] message)
        {
            StreamingSocket.Send(message, WebSocketMessageType.Binary);
        }
    }
}