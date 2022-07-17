using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Server
{
    class Program
    {
        public static UdpClient UDPServer = new UdpClient(1700);
        public static int connectedClients = 0;
        public static string[] clientIPs = new string[100];
        public static string[] clientPorts = new string[100];
        public static string[] clientKeys = new string[100];




        static void Main(string[] args)
        {
            

            while (true)
            {
                byte[] receivedData = new byte[1024];
                bool isNewClient = true;
                int recv = 0;
                var connectedClient = new IPEndPoint(IPAddress.Any, 1700);

                UDPServer.AllowNatTraversal(true);
                UDPServer.Client.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);

                Console.WriteLine("Waiting for message");

                receivedData = UDPServer.Receive(ref connectedClient); // listen on port 1700 for any message

                foreach (byte b in receivedData)
                {
                    if (b != 0)
                    {
                        recv++;
                    }
                }

                Console.WriteLine("\n");

                string request = Encoding.UTF8.GetString(receivedData, 0, recv);

                // Check if client is already in server database
                for (int i = 0; i < connectedClients; i++)
                {
                    if(clientIPs[i] == connectedClient.Address.ToString()) {
                        isNewClient = false;
                        if(request.Equals("0"))
                        {
                            connectedClients -= 1;
                            Array.Copy(clientIPs, i + 1, clientIPs, i, connectedClients);
                            Array.Copy(clientPorts, i + 1, clientPorts, i, connectedClients);
                            Array.Copy(clientKeys, i + 1, clientKeys, i, connectedClients);
                            SendMessageToClient("Byl jste odstranen ze zaznamu serveru.", connectedClient);
                            break;
                        }
                        else
                        {
                            SendMessageToClient("Uz jste zaznamenan na serveru.", connectedClient);
                            break;
                        }
                    }
                    else if(request == clientKeys[i])
                    {
                        // Exchanging connection info to clients
                        IPEndPoint secondClient = new IPEndPoint(IPAddress.Parse(clientIPs[i]), int.Parse(clientPorts[i]));
                        SendMessageToClient("Navazuji spojeni s " + connectedClient.Address.ToString() + ":" + connectedClient.Port.ToString(), secondClient);
                        SendMessageToClient(connectedClient.Address.ToString(), secondClient);
                        SendMessageToClient(connectedClient.Port.ToString(), secondClient);

                        SendMessageToClient("Navazuji spojeni s " + clientIPs[i] + ":" + clientPorts[i], connectedClient);
                        SendMessageToClient(clientIPs[i], connectedClient);
                        SendMessageToClient(clientPorts[i], connectedClient);
                        
                        //Removing connected client from database
                        connectedClients -= 1;
                        Array.Copy(clientIPs, i + 1, clientIPs, i, connectedClients);
                        Array.Copy(clientPorts, i + 1, clientPorts, i, connectedClients);
                        Array.Copy(clientKeys, i + 1, clientKeys, i, connectedClients);
                        
                        isNewClient = false;
                        break;
                    }
                }

                // Adds new client to server database
                if (isNewClient)
                {
                    clientIPs[connectedClients] = connectedClient.Address.ToString();
                    clientPorts[connectedClients] = connectedClient.Port.ToString();
                    clientKeys[connectedClients] = request;
                    connectedClients++;
                    SendMessageToClient("Byl jste zaznamenan na serveru s klicem " + request, connectedClient);
                }

                Console.WriteLine("Prichozi zprava z IP: " + connectedClient.Address.ToString() + " Port: " + connectedClient.Port.ToString());
                Console.WriteLine("Obsah zpravy: " + request + "\n");
                
            }
        }

        public static void SendMessageToClient(string message, IPEndPoint reciever)
        {
            int byteCount = Encoding.ASCII.GetByteCount(message);
            byte[] sendData = Encoding.ASCII.GetBytes(message);
            UDPServer.Send(sendData, byteCount, reciever);
        }

       
    }
}
