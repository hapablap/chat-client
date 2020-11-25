using ChatProtocol;
using System;

namespace ChatClient.MessageHandler
{
    public class ConnectResponseMessageHandler : IMessageHandler
    {
        public void Execute(IMessage message)
        {
            var connectResponseMessage = message as ConnectResponseMessage;
            if (connectResponseMessage.Success)
            {
                Console.WriteLine($"Connected! Session Id: {Program.Client.SessionId}");
                Program.Client.RequestUserList();
            }
            else
            {
                Console.WriteLine("Connection failed!");
            }
        }
    }
}
