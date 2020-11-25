using ChatProtocol;
using System;
using System.Net.Sockets;

namespace ChatClient.MessageHandler
{
    public class UserCountMessageHandler : IMessageHandler
    {
        public void Execute(IMessage message)
        {
            var userCountMessage = message as UserCountMessage;
            Console.WriteLine($"Users (online / sum): {userCountMessage.UserOnlineCount} / {userCountMessage.UserCount}");
        }
    }
}
