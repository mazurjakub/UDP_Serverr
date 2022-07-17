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
        




        static void Main(string[] args)
        {
            ClientData[] clients = new ClientData[150];
            UDPServer.AllowNatTraversal(true);
            UDPServer.Client.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);

            
            
            

            while (true)
            {
                byte[] receivedData = new byte[1024];
                bool isNewClient = true;
                int recv = 0;
                var connectedClient = new IPEndPoint(IPAddress.Any, 1700);

                

                Console.WriteLine("Cekam na zpravu...");

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
                    if(clients[i].IP == connectedClient.Address.ToString()) {
                        isNewClient = false;
                        if(request.Equals("0"))
                        {
                            connectedClients--;
                            Array.Copy(clients, i + 1, clients, i, connectedClients);
                            SendMessageToClient("Byl jste odstranen ze zaznamu serveru.", connectedClient);
                            break;
                        }
                        else
                        {
                            SendMessageToClient("Uz jste zaznamenan na serveru.", connectedClient);
                            break;
                        }
                    }
                    else if(request == clients[i].Key)
                    {
                        // Exchanging connection info to clients
                        IPEndPoint secondClient = new IPEndPoint(IPAddress.Parse(clients[i].IP), int.Parse(clients[i].Port));
                        SendMessageToClient("Navazuji spojeni s " + connectedClient.Address.ToString() + ":" + connectedClient.Port.ToString(), secondClient);
                        SendMessageToClient(connectedClient.Address.ToString(), secondClient);
                        SendMessageToClient(connectedClient.Port.ToString(), secondClient);

                        SendMessageToClient("Navazuji spojeni s " + clients[i].IP + ":" + clients[i].Port, connectedClient);
                        SendMessageToClient(clients[i].IP, connectedClient);
                        SendMessageToClient(clients[i].Port, connectedClient);
                        
                        //Removing connected client from database
                        connectedClients--;
                        Array.Copy(clients, i + 1, clients, i, connectedClients); 
                        isNewClient = false;
                        break;
                    }
                }

                //Remove timeouted clients
                for (int i = 0; i < connectedClients; i++)
                {
                    if (DateTime.Compare(clients[i].TimeOfRemoval, DateTime.Now) <= 0)
                    {
                        IPEndPoint removedClient = new IPEndPoint(IPAddress.Parse(clients[i].IP), int.Parse(clients[i].Port));
                        SendMessageToClient("Vas zaznam na serveru byl po uplynutych 10 minutach odstranen.", removedClient);

                        connectedClients--;
                        Array.Copy(clients, i + 1, clients, i, connectedClients);
                    }
                }

                // Adds new client to server database
                if (isNewClient)
                {
                    clients[connectedClients] = new ClientData() { IP = connectedClient.Address.ToString(), Port = connectedClient.Port.ToString(), Key = request, TimeOfRemoval = DateTime.Now.AddMinutes(10)  };
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
