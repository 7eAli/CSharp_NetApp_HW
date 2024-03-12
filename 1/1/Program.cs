﻿using System.Net.Sockets;

namespace _1_client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var tcpClient = new TcpClient();
            StreamReader reader = null;
            StreamWriter writer = null;
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

            async Task HandleReceive(StreamReader reader)
            {
                while (Receive(reader).Result)
                {
                    Receive(reader);
                }
            }

            async Task<bool> Receive(StreamReader reader)
            {
                try
                {
                    string answer = await reader.ReadLineAsync();
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

            async Task Send(StreamWriter writer, string name)
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
}
