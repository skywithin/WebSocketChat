using System;
using System.Threading.Tasks;
using ChatCore;
using ChatCore.Messages;
using Newtonsoft.Json;
using WebSocketSharp;

namespace ChatClient
{
    class Program
    {
        static readonly string ServerAddress = "ws://localhost:5000";

        static void Main(string[] args)
        {
            var userDetails = Login();

            StartChat(userDetails).GetAwaiter().GetResult();
        }

        private static UserDetails Login()
        {
            Console.WriteLine("What is your name?");

            string userName;

            while (string.IsNullOrWhiteSpace(userName = Console.ReadLine()))
            {
                Console.WriteLine("Please enter a valid name");
            }

            return new UserDetails
            {
                UserName = userName
            };
        }

        private static async Task StartChat(UserDetails user)
        {
            using (var ws = new WebSocket($"{ServerAddress}/Chat"))
            {
                ws.OnMessage += OnMessageReceived;
                ws.Connect();

                SendUserJoinedMessage(ws, user);

                var send = Task.Run(() =>
                {
                    string text;
                    while (!string.IsNullOrEmpty(text = Console.ReadLine()))
                    {
                        SendChatMessage(ws, user, text);
                    }
                });

                await Task.WhenAll(send);
            }

            Console.WriteLine("Session is closed. Press any key to exit");
            Console.ReadKey();
        }

        private static void SendUserJoinedMessage(WebSocket ws, UserDetails user)
        {
            var envelope =
                    new MessageEnvelope
                    {
                        MessageType = MessageType.UserJoined,
                        Author = user
                    };

            ws.Send(JsonConvert.SerializeObject(envelope));
        }

        private static void SendChatMessage(WebSocket ws, UserDetails user, string message)
        {
            var envelope =
                new MessageEnvelope
                {
                    MessageType = MessageType.Chat,
                    Author = user,
                    Payload = JsonConvert.SerializeObject(
                        new ChatMessage
                        {
                            Content = message
                        })
                };

            ws.Send(JsonConvert.SerializeObject(envelope));
        }

        private static void OnMessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                var envelope = JsonConvert.DeserializeObject<MessageEnvelope>(e.Data);

                switch (envelope.MessageType) 
                {
                    case MessageType.Chat:
                        var message = JsonConvert.DeserializeObject<ChatMessage>(envelope.Payload);
                        Console.WriteLine($"[{envelope.DateCreatedUtc.ToLocalTime()}] {envelope.Author.UserName}: {message.Content}");
                        break;

                    case MessageType.UserJoined:
                        Console.WriteLine($"{envelope.Author.UserName} has joined");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
