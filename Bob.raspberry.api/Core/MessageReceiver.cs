using System;
using System.Threading;

namespace Bob.raspberry.api.Core
{
    public class MessageReceiver: IMessageReceiver
    {
        private object _lock = new object();
        private byte[] _message = null;
        private IBobSocket _socket;
        public MessageReceiver(IBobSocket socket)
        {
            this._socket = socket;
            this._socket.OnMessageReceived += this.HandleReceivedMessage;
        }

        private void Close()
        {
            this._socket.OnMessageReceived -= this.HandleReceivedMessage;
        }

        private void HandleReceivedMessage(object obj, byte[] message)
        {
            lock (this._lock)
            {
                this._message = message;
                this.Close();
            }
        }

        public byte[] Receive(int milliseconds)
        {
            var timeout = DateTime.Now.AddMilliseconds(milliseconds);
            byte[] message;
            do
            {
                if (timeout < DateTime.Now)
                {
                    this.Close();
                    throw new TimeoutException();
                }
                Thread.Sleep(100);
                message = this.Pop();
            } while (message == null);
            return message;
        }

        private byte[] Pop()
        {
            lock (this._lock)
            {
                if (this._message != null)
                {
                    var message = this._message;
                    this._message = null;
                    return message;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
