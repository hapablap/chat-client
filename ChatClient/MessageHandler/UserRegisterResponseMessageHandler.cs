using ChatProtocol;
using System;

namespace ChatClient.MessageHandler
{
    public class UserRegisterResponseMessageHandler : IMessageHandler
    {
        public void Execute(IMessage message)
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
        }
    }
}
