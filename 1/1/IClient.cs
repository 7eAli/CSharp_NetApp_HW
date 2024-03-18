using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1
{
    public interface IClient
    {
       Task<bool> Receive(string answer);
       Task Send(StreamWriter writer, string name);
    }
}
