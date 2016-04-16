using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace SampleClientUdpConsole
{
    class Client
    {
        private int Port;
        private string HostName;

        public Client(string HostName, int SampleUdpPort)
        {
            this.HostName = HostName;
            this.Port = SampleUdpPort;
            Console.WriteLine("HostName and Port init: " + HostName + ":" + Port);
        }

        public void StartClient()
        {
            try
            {
                while (true)
                {
                    Console.WriteLine("Write whatEver: ");
                    var whatEver = Console.ReadLine();
                    UdpClient client = new UdpClient(HostName, Port);
                    Byte[] inputToBeSent = new Byte[256];
                    inputToBeSent = System.Text.Encoding.ASCII.GetBytes(whatEver.ToCharArray());
                    IPHostEntry remoteHostEntry = Dns.GetHostEntry(HostName);
                    IPEndPoint remoteIpEndPoint = new IPEndPoint(remoteHostEntry.AddressList[0], Port);
                    int nBytesSent = client.Send(inputToBeSent, inputToBeSent.Length);
                    Byte[] received = new Byte[512];
                    received = client.Receive(ref remoteIpEndPoint);
                    String dataReceived = System.Text.Encoding.ASCII.GetString(received);
                    Console.WriteLine(dataReceived);
                    client.Close();
                }
            }
            catch (Exception)
            {
                Console.WriteLine("error, sorry pidor");
            }
        }
    }
}
