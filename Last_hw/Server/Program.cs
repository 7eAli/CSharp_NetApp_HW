using NetMQ;
using NetMQ.Sockets;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var runtime = new NetMQRuntime())
            {
                var server = new Server();

                runtime.Run(server.Run());
            }

        }
    }

    public class Server
    {
        PullSocket? serverReceive;
        PushSocket? serverRespond;

        public async Task Run()
        {
            serverReceive = new PullSocket();
            serverRespond = new PushSocket();
            serverReceive.Bind("tcp://*:55555");
            serverRespond.Bind("tcp://*:55556");
            while (true)
            {
                var message = await serverReceive.ReceiveFrameStringAsync();
                await Console.Out.WriteLineAsync(message.Item1);
                serverRespond.SendFrame(message.Item1);
            }
        }
    }
}
