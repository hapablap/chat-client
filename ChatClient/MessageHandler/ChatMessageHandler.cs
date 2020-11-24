using ChatProtocol;
using System;
using System.Net.Sockets;

namespace ChatClient.MessageHandler
{
    public class ChatMessageHandler : IMessageHandler
    {
        public void Execute(TcpClient client, IMessage message)
        {
            var chatMessage = message as ChatMessage;
            var user = Program.Users.Find(u => u.Id == chatMessage.UserId);
            var username = $"Unbekannt ({chatMessage.UserId})";
            if (user != null)
                username = user.Username;

            Console.WriteLine($"{username} [{DateTime.Now}]: {chatMessage.Content}");
        }
    }
}
