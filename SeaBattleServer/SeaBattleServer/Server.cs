using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using Newtonsoft.Json;
using SeaBattleLibrary;

namespace SeaBattleServer
{
    class Server
    {
        private int Port;
        private Socket sListener;
        private List<Game> Games = new List<Game>();
        private List<Game.Gamer> Gamers = new List<Game.Gamer>();
        IPAddress ipAddress;

        public Server(int Port)
        {   
            this.Port = Port;
            Console.WriteLine("The port is set");
        }

        public void StartServer()
        {
            Console.WriteLine("Trying to start the Server");
            
            try
            {
                sListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                try
                {
                    ipAddress = IPAddress.Parse("127.0.0.1");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Local host not found: " + e.Message);
                    return;
                }
                IPEndPoint localIpEndPoint = new IPEndPoint(ipAddress, Port);
                Console.WriteLine(ipAddress.ToString() + ":" + Port);
                sListener.Bind(localIpEndPoint);
                new Thread(new ThreadStart(FromClient)).Start();

            }
            catch (Exception e)
            {
                Console.WriteLine("XZ: " + e.Message);
            }
        }

        private void FromClient()
        {
            while (true)
            {
                try
                {
                    byte[] dataReceived = new byte[1024];
                    EndPoint ip = new IPEndPoint(ipAddress, Port);
                    var byteRec = sListener.ReceiveFrom(dataReceived, ref ip);
                    var find = Gamers.Find((gamer) => gamer.Ip.Equals(ip));
                    if (find == null)
                    {
                        Console.WriteLine("Connected: " + ip.ToString());
                        Gamers.Add(new Game.Gamer(ip));
                    }
                    var message = Encoding.ASCII.GetString(RemoveBytes(dataReceived, byteRec));
                    CallMethod(ip, GetMethod(message));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
            }
        }

        private void SendMessage(Method method, EndPoint ip)
        {
            string serializeMethod = JsonConvert.SerializeObject(method);
            byte[] methodByte = Encoding.ASCII.GetBytes(serializeMethod.ToCharArray());
            sListener.SendTo(methodByte, ip);
        }

        private byte[] RemoveBytes(byte[] buf, int bytesReceived)
        {
            byte[] received = new byte[bytesReceived];
            for (int i = 0; i < bytesReceived; i++)
                received[i] = buf[i];
            return received;
        }

        private Method GetMethod(String str)
        {
            return JsonConvert.DeserializeObject<Method>(str);
        }

        private void CallMethod(EndPoint ip, Method method)
        {
            switch (method.MethodName)
            {
                case Method.NamesServer.SetShips:
                    List<Ship> listShip = JsonConvert.DeserializeObject<List<Ship>>(method.Parameters[0]);
                    SetShip(getGamer(ip), listShip);
                    break;
                case Method.NamesServer.Exit:
                    ExitClient(getGamer(ip));
                    break;
                case Method.NamesServer.HitTheEnemy:
                    Console.WriteLine("Hit The Enemy");
                    HitTheEnemy(getGamer(ip), JsonConvert.DeserializeObject<Address>(method.Parameters[0]));
                    break;
            }
        }

        private void SetShip(Game.Gamer gamer, List<Ship> listShip)
        {
            gamer.Map = new BattleMap();
            foreach (Ship sh in listShip)
            {
                gamer.Map.AddShip(sh.address);
            }
            if (Gamers.Count == 2)
            {
                new Thread(new ThreadStart(StartGame)).Start();
            }
        }

        private void StartGame()
        {
            Console.WriteLine("Start Game");
            Game.Gamer gamer1 = Gamers[Gamers.Count - 1];
            Game.Gamer gamer2 = Gamers[Gamers.Count - 2];
            Gamers.Clear();
            Games.Add(new Game(gamer1, gamer2));
            int first = new Random().Next(1, 2); ;
            if (first == 1)
            {
                gamer1.Status = Game.Status.YOUR;
                gamer2.Status = Game.Status.ENEMY;

            }
            else 
            {
                gamer2.Status = Game.Status.YOUR;
                gamer1.Status = Game.Status.ENEMY;
            }

            SendStatusToClients(gamer2, gamer1);
        }

        private void SendEnemyMapForClient(Game.Gamer gamer)
        {
            string serializeMap = JsonConvert.SerializeObject(GetEnemyGamer(gamer).Map.GetMapForOpponent().OwnMap);
            Method method = new Method(Method.NamesClient.SetEnemyMap, new string[] { serializeMap });
            SendMessage(method, gamer.Ip);
        }

        private void SendYourMapForClient(Game.Gamer gamer)
        {
            string serializeMap = JsonConvert.SerializeObject(gamer.Map.OwnMap);
            Method method = new Method(Method.NamesClient.SetEnemyMap, new string[] { serializeMap });
            SendMessage(method, gamer.Ip);
        }

        private Game.Gamer getGamer(EndPoint ip)
        {
            return Gamers.Find((gs) => gs.Ip.Equals(ip));
        }

        private Game GetGame(Game.Gamer gamer)
        {
            return Games.Find((gms) => gms.Gamer1.Ip.Equals(gamer.Ip) || gms.Gamer2.Ip.Equals(gamer.Ip));
        }

        private Game.Gamer GetEnemyGamer(Game.Gamer gamer)
        {
            var game = GetGame(gamer);
            Game.Gamer enemy;
            if (game.Gamer1 == gamer) 
                enemy = game.Gamer2;
            else
                enemy = game.Gamer1;
            return enemy;
        }

        private void SendStatusToClients(params Game.Gamer[] gamers)
        {
            foreach(Game.Gamer gmr in gamers) {
                Method method1 = new Method(Method.NamesClient.SetStatus, new string[] { JsonConvert.SerializeObject(gmr.Status) });
                Console.WriteLine(JsonConvert.SerializeObject(gmr.Status) + "");
                SendMessage(method1, gmr.Ip);
            }
        }

        private void HitTheEnemy(Game.Gamer gamer, Address address)
        {
            var enemy = GetEnemyGamer(gamer);
            int v = enemy.Map.Kill(address);
            if (v == -2)
            {
                SendMessage(new Method(Method.NamesClient.Message, new string[] { "Try Again" }), gamer.Ip);
            }
            else
            {
                Method method = new Method();
                method.MethodName = Method.NamesClient.Message;
                if (v == 1) {
                    method.Parameters[0] = "You Killed a ship!!";
                }else if (v == -1)
                {
                    method.Parameters[0] = "You've come to the empty cage";
                }
                else if (v == -1)
                {
                    method.Parameters[0] = "You killed field with part ship";
                }
                SendEnemyMapForClient(gamer);
            }
        }

        private void ExitClient(Game.Gamer gamer)
        {
            Console.WriteLine(gamer.Ip + " is Exit from Game");
            Gamers.Remove(gamer);
        }
    }
}
