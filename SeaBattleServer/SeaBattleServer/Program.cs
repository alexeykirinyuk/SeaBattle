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
            Server server = Server.Instance;
            server.Start();
            Console.ReadLine();
        }

    }
}
