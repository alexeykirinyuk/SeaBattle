using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace SeaBattleServer
{
    public class Server
    {
        private static Server server;
        private static object blockSingletone = new object();

        public static Server Instance
        {
            get
            {
                if (server == null)
                {
                    lock (blockSingletone)
                    {
                        if (server == null)
                            server = new Server();
                    }
                }
                return server;
            }
        }

        private TcpListener tcpListner;

        public List<Game> Games { get; set; }

        public List<Handler> PlayerHadlerList { get; set; }

        private Server()
        {
            var ip = Dns.GetHostAddresses("127.0.0.1")[0];
            var endPoint = new IPEndPoint(ip, 14000);
            tcpListner = new TcpListener(endPoint);

            Games = new List<Game>();
            PlayerHadlerList = new List<Handler>();
        }

        public void Start()
        {
            tcpListner.Start();
            while (true)
            {
                var acceptClient = tcpListner.AcceptTcpClient();
                Console.WriteLine("Присоединился новый клиент");
                var nPlayer = new Player();
                var nPlayerHandler = new PlayerHandler() { TcpClient = acceptClient, Player = nPlayer };
                nPlayerHandler.Start();
            }
        }
    }
}
