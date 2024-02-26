using System.Net;
using System.Net.Sockets;
using System.Text;

namespace _1_server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var server = new ChatServer();
            await server.Run();
        }
    }
    public class ChatServer
    {
        TcpListener _listener = new TcpListener(IPAddress.Any, 55555);

        List<TcpClient> clients = new List<TcpClient>();
        public async Task Run()
        {
            try
            {
                _listener.Start();
                await Console.Out.WriteLineAsync("Сервер запущен");

                while (true)
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync();
                    Console.WriteLine("Успешное поключение");
                    while (true)
                    {
                        var stream = tcpClient.GetStream();
                        await Task.Run(() => ProcessClient(stream));
                    }
                }
            }
            catch
            {

            }
            finally
            {
                
            }
        }

        public async Task ProcessClient(NetworkStream stream)
        {
            byte[] data = new byte[1024];
            int bytes = 0;
            var message = new StringBuilder();
            do
            {
                bytes = await stream.ReadAsync(data);
                message.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (stream.DataAvailable);
            Console.WriteLine($"{DateTime.Now.ToLongTimeString()}: " + message);
            await stream.WriteAsync(Encoding.UTF8.GetBytes($"Сообщение доставлено: {DateTime.Now.ToLongTimeString()}"));
        }

    }
}
