using System.Net.Sockets;
using System.Text;

namespace _1_client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync("127.0.0.1", 55555);
            

            while (true)
            {
                NetworkStream netStream = tcpClient.GetStream();
                string message = Console.ReadLine()!;
                var request = Encoding.UTF8.GetBytes(message);
                await netStream.WriteAsync(request);

                byte[] response = new byte[256];
                int read = await netStream.ReadAsync(response);

                Console.WriteLine($"{Encoding.UTF8.GetString(response, 0, read)}");
            }
        }
    }
}
