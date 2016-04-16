using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleClientUdpConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client("127.0.0.1", 5000);
            client.StartClient();
            Console.ReadLine();
        }
    }
}
