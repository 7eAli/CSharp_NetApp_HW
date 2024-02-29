using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
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
    public class ClientInfo
    {
        public long id { get; }
        public string name { get; set; }
        public TcpClient tcpClient { get; }        
        public StreamReader reader { get; }
        public StreamWriter writer { get; }
        public ChatServer chatServer { get; }

        public ClientInfo(long id, TcpClient tcpClient, ChatServer chatServer)
        {
            this.id = id;            
            this.tcpClient = tcpClient;
            this.chatServer = chatServer;            
            reader = new StreamReader(tcpClient.GetStream());
            writer = new StreamWriter(tcpClient.GetStream());            
        }

        public void Close()
        {                        
            writer.Close();
            reader.Close();
            tcpClient.Close();
        }
    }

    public class ChatServer
    {
        long id = 1;
        TcpListener _listener = new TcpListener(IPAddress.Any, 55555);

        List<ClientInfo> clients = new List<ClientInfo> { };
        public async Task Run()
        {
            try
            {
                _listener.Start();
                await Console.Out.WriteLineAsync("Сервер запущен");

                while (true)
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync();
                    ClientInfo client = new ClientInfo(id++, tcpClient, this);                    
                    clients.Add(client);
                    Task.Run(() => ProcessClient(client));
                }
            }
            catch
            {

            }
            finally
            {

            }
        }        

        public async Task ProcessClient(ClientInfo client)
        {            
            string? userName = await client.reader.ReadLineAsync();
            string? message = $"{userName} вошел в чат";            
            await client.chatServer.BroadcastMessageAsync(message, client.id);
            Console.WriteLine(message);            
            while (true)
            {
                try
                {
                    message = await client.reader.ReadLineAsync();
                    if (message == null) continue;
                    if (message == "exit")
                    {
                        await client.chatServer.SendMessageAsync("До свидания", client);
                        Disconnect(client);
                    }
                    else
                    {
                        message = $"{userName}: {message}";
                        Console.WriteLine(message);
                        await client.chatServer.BroadcastMessageAsync(message, client.id);
                    }
                }
                catch
                {
                    message = $"{userName} покинул чат";
                    Console.WriteLine(message);
                    await client.chatServer.BroadcastMessageAsync(message, client.id);
                    break;
                }
            }
        }

        public async Task SendMessageAsync(string message, ClientInfo client)
        { 
            await client.writer.WriteAsync(message);
            await client.writer.FlushAsync();
        }

        public async Task BroadcastMessageAsync(string message, long id)
        {
            foreach (var client in clients)
            {
                if (client.id != id) // если id клиента не равно id отправителя
                {
                    await client.writer.WriteLineAsync(message); //передача данных
                    await client.writer.FlushAsync();
                }
            }
        }

        public void Disconnect(ClientInfo client)
        {
            clients.Remove(client);
            client.Close();
        }
    }
}
