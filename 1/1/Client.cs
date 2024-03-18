using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace _1
{
    public class Client : IClient
    {
        TcpClient tcpClient {  get; set; }
        StreamReader reader;
        StreamWriter writer;

        public async Task Run()
        {
            tcpClient = new TcpClient();
            try
            {
                await tcpClient.ConnectAsync("127.0.0.1", 55555);
                reader = new StreamReader(tcpClient.GetStream());
                writer = new StreamWriter(tcpClient.GetStream());

                Console.WriteLine("Введите имя");
                string name = Console.ReadLine()!;

                Task.Run(() => HandleReceive(reader));
                await Send(writer, name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                reader.Close();
                writer.Close();
                tcpClient.Close();
            }
        }
        async Task HandleReceive(StreamReader reader)
        {
            bool flag = true;
            while (flag)
            {
                if(!Receive(await reader.ReadLineAsync()).Result)
                {
                    flag = false;
                }
            }
        }
        public async Task<bool> Receive(string answer)
        {
            try
            {                
                if (string.IsNullOrEmpty(answer))
                    return false;
                Console.WriteLine(answer);
                return true;
            }
            catch
            {
                return true;
            }
        }

        public async Task Send(StreamWriter writer, string name)
        {
            await writer.WriteLineAsync(name);
            await writer.FlushAsync();

            while (true)
            {
                string message = Console.ReadLine()!;
                await writer.WriteLineAsync(message);
                await writer.FlushAsync();
                if (message == "exit")
                    return;
            }
        }
    }
}
