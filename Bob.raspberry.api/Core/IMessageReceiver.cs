using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bob.raspberry.api.Core
{
    public interface IMessageReceiver
    {
        byte[] Receive(int milliseconds);
    }
}
