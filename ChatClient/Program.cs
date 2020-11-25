using ChatClient.MessageHandler;
using ClientLibrary;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ChatClient
{
    class Program
    {
        public static bool IsApplicationExecuting = true;

        public static List<User> Users = new List<User>();

        public static Client Client;

        static void MessageHandle()
        {
            while (true)
            {
                lock (Client.ReceivedMessages)
                {
                    foreach (var message in Client.ReceivedMessages)
                    {
                        MessageHandlerFactory.GetMessageHandler(message.MessageId).Execute(message);
                    }

                    Client.ReceivedMessages.Clear();
                }
            }
        }

        static void StartMessageHandleThread()
        {
            var messageHandleThreadStart = new ThreadStart(MessageHandle);
            var messageHandleThread = new Thread(messageHandleThreadStart);
            messageHandleThread.Start();
        }

        static void Main()
        {
            Client = new Client("127.0.0.1", 13000);
            StartMessageHandleThread();

            while (IsApplicationExecuting)
            {
                Console.WriteLine("Do you want to login (l) or to register (r)?");
                var loginRegisterInput = Console.ReadKey();

                if (loginRegisterInput.Key == ConsoleKey.L)
                {
                    Console.Clear();

                    Console.WriteLine("Login");

                    Console.WriteLine("Username: ");
                    var username = Console.ReadLine();

                    Console.WriteLine("Password: ");
                    var password = Console.ReadLine();

                    Console.WriteLine("Connecting to server.");
                    Client.Connect(username, password);

                    while (Client.IsConnecting)
                    {

                    }

                    while (Client.IsConnected)
                    {
                        Console.WriteLine("Nachricht eingeben:");
                        var input = Console.ReadLine();

                        switch (input)
                        {
                            case "/disconnect":
                                Client.Disconnect();
                                break;
                            case "/exit":
                                Client.Disconnect();
                                IsApplicationExecuting = false;
                                break;
                            default:
                                Client.SendChatMessage(input);
                                break;
                        }
                    }
                }

                if(loginRegisterInput.Key == ConsoleKey.R)
                {
                    Console.Clear();

                    Console.WriteLine("Register");

                    Console.WriteLine("Username: ");
                    var username = Console.ReadLine();

                    Console.WriteLine("Password: ");
                    var password = Console.ReadLine();

                    Client.Register(username, password);
                }
            }
        }
    }
}
