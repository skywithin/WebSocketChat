using System;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace ChatServer
{
    class Program
    {
        static readonly string ServerAddress = "ws://localhost:5000";

        static void Main(string[] args)
        {
            var wssv = new WebSocketServer(ServerAddress);
            wssv.AddWebSocketService<Chat>("/Chat");
            wssv.Start();

            Console.WriteLine($"WebSocket chat server started on {ServerAddress}");

            // TODO: Stop application properly
            Console.WriteLine("Press any key to shut down WebSocket chat server");
            Console.ReadKey(); 
            wssv.Stop();
        }
    }

    public class Chat : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            //TODO: Store message
            Console.WriteLine($"Received data from client: {e.Data}");

            Sessions.Broadcast(e.Data);
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            Console.WriteLine($"New connection opened");

            //TODO: Send chat history
            //Send(e.Data);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            base.OnClose(e);
            Console.WriteLine($"Connection closed");
        }

        protected override void OnError(ErrorEventArgs e)
        {
            base.OnError(e);
            Console.WriteLine($"Error has occurred");
            Console.WriteLine(e.Message);
        }
    }
}
