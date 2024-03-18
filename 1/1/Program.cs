using _1;
using System.Net.Sockets;

namespace _1_client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new Client();
            await client.Run();            
        }
    }
}
