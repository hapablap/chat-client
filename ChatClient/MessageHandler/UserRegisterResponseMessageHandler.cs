using ChatProtocol;
using System;
using System.Net.Sockets;

namespace ChatClient.MessageHandler
{
    public class UserRegisterResponseMessageHandler : IMessageHandler
    {
        public void Execute(TcpClient client, IMessage message)
        {
            var userRegisterResponseMessage = message as UserRegisterResponseMessage;

            if(userRegisterResponseMessage.Success)
            {
                Console.WriteLine("Registered! You can now log in with your credentials.");
            }
            else
            {
                Console.WriteLine($"Registration failed: {userRegisterResponseMessage.ErrorMessage}");
            }

            Program.StopReceiveDataThread();
        }
    }
}
