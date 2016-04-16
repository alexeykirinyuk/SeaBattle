using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattleServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server(25);
            server.StartServer();
            Console.ReadLine();
        }

    }
}
