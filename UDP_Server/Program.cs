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
        static void Main(string[] args)
        {
            UdpClient udpServer = new UdpClient(1700);
            

            while (true)
            {
                byte[] receivedData = new byte[1024];
                int recv = 0;
                var remoteEP = new IPEndPoint(IPAddress.Any, 1700);
                udpServer.AllowNatTraversal(true);
                receivedData = udpServer.Receive(ref remoteEP); // listen on port 11000

                foreach (byte b in receivedData)
                {
                    if (b != 0)
                    {
                        recv++;
                    }
                }

                string request = Encoding.UTF8.GetString(receivedData, 0, recv);

                Console.WriteLine("receive data from " + remoteEP.ToString());
                Console.WriteLine("Recieved data: " + request);
                udpServer.Send(new byte[] { 1 }, 1, remoteEP); // reply back
            }
        }
    }
}
