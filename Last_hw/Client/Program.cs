using System.Net.Sockets;
using System.Net;
using NetMQ.Sockets;
using NetMQ;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var runtime = new NetMQRuntime())
            {
                var client = new Client();

                runtime.Run(client.Run());
            }
        }
    }    

    public class Client
    {
        PushSocket? clientSend;
        PullSocket? clientReceive;

        public async Task Run()
        {
            clientSend = new PushSocket();
            clientReceive = new PullSocket();
            try
            {
                clientSend.Connect("tcp://localhost:55555");
                clientReceive.Connect("tcp://localhost:55556");
                Console.WriteLine("Введите имя");
                string name = Console.ReadLine()!;
                while (true)
                {
                    Task.Run(() => Manage(clientSend, name, clientReceive));                    
                }
            }
            finally
            {
                clientSend.Close();
            }
        }

        public async Task Manage(PushSocket client, string name, PullSocket pullSocket)
        {
            while (true)
            {
                string? message = await Console.In.ReadLineAsync();
                client.SendFrame(name + ": " + message);
                var response = await pullSocket.ReceiveFrameStringAsync();
                Console.WriteLine(response);
            }
        }
    }
}
