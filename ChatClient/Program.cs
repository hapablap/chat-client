using ChatClient.MessageHandler;
using ChatProtocol;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

namespace ChatClient
{
    class Program
    {
        static string serverIpAddress = "127.0.0.1";
        static int serverPort = 13000;

        static TcpClient client;
        public static string SessionId;
        public static bool IsConnected = false;
        public static bool IsConnecting = false;
        public static bool IsApplicationExecuting = true;

        static Thread receiveDataThread;

        public static List<User> Users = new List<User>();

        static void Register(string username, string password)
        {
            client = new TcpClient(serverIpAddress, serverPort);

            var registerMessage = new UserRegisterMessage
            {
                Username = username,
                Password = password
            };

            StartReceiveDataThread();
            SendMessage(JsonSerializer.Serialize(registerMessage));
        }

        static void Connect(string username, string password)
        {
            IsConnecting = true;
            client = new TcpClient(serverIpAddress, serverPort);

            var connectMessage = new ConnectMessage
            {
                ServerPassword = "test123",
                Username = username,
                Password = password
            };

            StartReceiveDataThread();
            SendMessage(JsonSerializer.Serialize(connectMessage));
        }

        static void Disconnect()
        {
            var disconnectMessage = new DisconnectMessage()
            {
                SessionId = SessionId
            };
            SendMessage(JsonSerializer.Serialize(disconnectMessage));

            StopReceiveDataThread();

            IsConnecting = false;
            IsConnected = false;
            SessionId = string.Empty;
            client.Close();
            client = null;
        }

        public static void RequestUserList()
        {
            var userListRequestMessage = new UserListRequestMessage
            {
                SessionId = SessionId
            };
            SendMessage(JsonSerializer.Serialize(userListRequestMessage));
        }

        public static void StopReceiveDataThread()
        {
            if(receiveDataThread != null)
                receiveDataThread.Join();
        }

        public static void StartReceiveDataThread()
        {
            var threadStart = new ThreadStart(ReceiveData);
            receiveDataThread = new Thread(threadStart);
            receiveDataThread.Start();
        }

        static void SendMessage(string messageJson)
        {
            // Verschlüsselung: messageJson verschlüsseln
            var data = System.Text.Encoding.UTF8.GetBytes(messageJson);
            var stream = client.GetStream();
            stream.Write(data, 0, data.Length);
        }

        static void ReceiveData()
        {
            while (client != null)
            {
                try
                {
                    lock (client)
                    {
                        var data = new byte[1024];
                        var bytes = client.GetStream().Read(data, 0, data.Length);
                        var responseData = System.Text.Encoding.UTF8.GetString(data, 0, bytes);
                        var genericMessage = JsonSerializer.Deserialize<GenericMessage>(responseData);
                        var message = MessageFactory.GetMessage(genericMessage.MessageId, responseData);
                        var messageHandler = MessageHandlerFactory.GetMessageHandler(genericMessage.MessageId);
                        messageHandler.Execute(client, message);
                    }
                }
                catch(System.IO.IOException)
                { }
                catch(System.ObjectDisposedException)
                { }
            }
        }

        static void SendChatMessage(string messageContent)
        {
            try
            {
                // Prepare chat message
                var chatMessage = new ChatMessage
                {
                    Content = messageContent,
                    SessionId = SessionId
                };

                // Send message
                SendMessage(JsonSerializer.Serialize(chatMessage));
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        static void Main()
        {
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
                    Connect(username, password);

                    while (IsConnecting)
                    {

                    }

                    while (IsConnected)
                    {
                        Console.WriteLine("Nachricht eingeben:");
                        var input = Console.ReadLine();

                        switch (input)
                        {
                            case "/disconnect":
                                Disconnect();
                                break;
                            case "/exit":
                                Disconnect();
                                IsApplicationExecuting = false;
                                break;
                            default:
                                SendChatMessage(input);
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

                    Register(username, password);
                }
            }
        }
    }
}
