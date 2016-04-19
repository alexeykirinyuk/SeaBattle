using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using SeaBattleLibrary;

namespace SeaBattleServer
{
    class Server
    {
        private int port;
        private Socket sListener;
        private List<Game> gameList = new List<Game>();
        private List<Player> playerList = new List<Player>();
        private IPAddress ipAddress;

        public Server(int port)
        {
            this.port = port;
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
                IPEndPoint localIpEndPoint = new IPEndPoint(ipAddress, port);
                Console.WriteLine(ipAddress.ToString() + ":" + port);
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
                    byte[] dataReceived = new byte[5000];
                    EndPoint ip = new IPEndPoint(ipAddress, port);
                    var byteRec = sListener.ReceiveFrom(dataReceived, ref ip);
                    var find = playerList.Find((gamer) => gamer.Ip.Equals(ip));
                    if (find == null)
                    {
                        Console.WriteLine("Connected: " + ip.ToString());
                        playerList.Add(new Player(ip));
                    }
                    var message = Encoding.Unicode.GetString(RemoveBytes(dataReceived, byteRec));
                    Console.WriteLine(message);
                    //Thread callMethodThread = new Thread(new ParameterizedThreadStart(CallMethod));
                    //callMethodThread.IsBackground = true;
                    //callMethodThread.Start(new CallMethodClass(ip, GetMethod(message)));
                    CallMethod(ip, Method.Deserialize(message));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }
        
        private void CallMethod(EndPoint ip, Method method)
        {
            switch (method.Name)
            {
                case Method.MethodName.SetShips:
                    Param param = (method[0]);
                    List<Ship> listShip = ParamConvert.GetShipList(method[0]);
                    SetShip(GetPlayer(ip), listShip);
                    break;
                case Method.MethodName.Exit:
                    PlayerExit(GetPlayer(ip));
                    break;
                case Method.MethodName.HitTheEnemy:
                    HitTheEnemy(GetPlayer(ip), (Address)method[0]);
                    break;
            }
        }

        private void SetShip(Player player, List<Ship> listShip)
        {
            foreach (Ship ship in listShip)
            {
                player.Map.AddShip(ship);
            }
            if (playerList.Count % 2 == 0)
            {
                StartGame();
            }
        }

        private void StartGame()
        {
            Console.WriteLine("Start Game");

            Game nGame = new Game(playerList[playerList.Count - 2], playerList[playerList.Count - 1]);
            gameList.Add(nGame);
            nGame.SetWhoseTurn(true);

            SendTurnToClients(nGame.Player1, nGame.Player2);
        }      

        private void SendTurnToClients(params Player[] players)
        {
            foreach (Player player in players)
            {
                Method setTurnMethod = new Method(Method.MethodName.SetTurn, ParamConvert.Convert(player.WhoseTurn));
                SendMessage(setTurnMethod, player.Ip);
            }
        }

        private void HitTheEnemy(Player player, Address address)
        {
            Player enemy = GetEnemy(player);
            KillResult killResult = enemy.Map.Kill(address);

            Method methodForYour = new Method(Method.MethodName.SetResultAfterYourHit, 3);
            Method methodForEnemy = new Method(Method.MethodName.SetResultAfterEnemyHit, 3);

            switch (killResult)
            {
                case KillResult.Error:
                    methodForYour[0] = ParamConvert.Convert("Попробуй ещё раз.");
                    methodForYour[1] = ParamConvert.Convert(player.WhoseTurn);
                    methodForYour[2] = ParamConvert.Convert(enemy.Map.GetMapForEnemy().StatusMap);
                    SendMessage(methodForYour, player.Ip);
                    return;
                case KillResult.KillShip:
                    methodForYour[0] = ParamConvert.Convert("Ты уничтожил корабль врага!");
                    methodForEnemy[0] = ParamConvert.Convert("Враг уничтожил твой корабль!");
                    break;
                case KillResult.KillPartShip:
                    methodForYour[0] = ParamConvert.Convert("Ты повредил корабль врага!");
                    methodForEnemy[0] = ParamConvert.Convert("Враг повредил твой корабль!");
                    break;
                case KillResult.KillEmpty:
                    GetGame(player).ReverseTurn();
                    methodForYour[0] = ParamConvert.Convert("Ты промазал!");
                    methodForEnemy[0] = ParamConvert.Convert("Противник промазал!");
                    break;
            }
            methodForYour[1] = ParamConvert.Convert(player.WhoseTurn);
            methodForEnemy[1] = ParamConvert.Convert(enemy.WhoseTurn);
            methodForYour[2] = ParamConvert.Convert(enemy.Map.GetMapForEnemy().StatusMap);
            methodForEnemy[2] = ParamConvert.Convert(enemy.Map.StatusMap);
            Console.WriteLine("Ход{" + player.Ip + ":" + player.WhoseTurn);
            Console.WriteLine("Ход{" + enemy.Ip + ":" + enemy.WhoseTurn);

            SendMessage(methodForYour, player.Ip);
            SendMessage(methodForEnemy, enemy.Ip);

            GameOver(!enemy.Map.HasShip(), player, enemy);
        }

        private void GameOver(bool isOver, Player player, Player enemy)
        {
            SendMessage(new Method(Method.MethodName.GameOver, ParamConvert.Convert(Player.Turn.YOUR)), player.Ip);
            SendMessage(new Method(Method.MethodName.GameOver, ParamConvert.Convert(Player.Turn.ENEMY)), enemy.Ip);
            Console.WriteLine("Игра закончена!! Win: " + player.Ip.ToString() + ";\nЧистка играков...");
            gameList.Remove(GetGame(player));
            Console.WriteLine("Удаление...:" + player.Ip);
            playerList.Remove(player);
            Console.WriteLine("Удаление...:" + player.Ip);
            playerList.Remove(enemy);
        }

        private void PlayerExit(Player player)
        {
            Player enemy = GetEnemy(player);
            SendMessage(new Method(Method.MethodName.GameOver, ParamConvert.Convert("Ваш враг вышел из игры.")), enemy.Ip);
            Console.WriteLine(player.Ip + " вышел из игры.");
            Console.WriteLine(enemy.Ip + " - был врагом " + player.Ip);
            gameList.Remove(GetGame(enemy));
            playerList.Remove(player);
            playerList.Remove(enemy);
        }

        /*
        private class CallMethodClass
        {
            public EndPoint ip;
            public Method method;
            public CallMethodClass(EndPoint ip, Method method)
            {
                this.ip = ip;
                this.method = method;
            }
        }
        private void CallMethod(Object obj)
        {
            CallMethodClass cmc = (CallMethodClass)obj;
            CallMethod(cmc.ip, cmc.method);
            Console.WriteLine("New thread for: " + cmc.method.Name);
        }
         */

        private Player GetPlayer(EndPoint ip)
        {
            Player find = playerList.Find((player) => player.Ip.Equals(ip));
            return find;
        }
        private Game GetGame(Player player)
        {
            return gameList.Find((game) => game.Player1.Ip.Equals(player.Ip) || game.Player2.Ip.Equals(player.Ip));
        }
        private Player GetEnemy(Player player)
        {
            Game game = GetGame(player);
            Player enemy;
            if (game.Player1 == player)
                enemy = game.Player2;
            else
                enemy = game.Player1;
            return enemy;
        }
        private byte[] RemoveBytes(byte[] buf, int bytesReceived)
        {
            byte[] received = new byte[bytesReceived];
            for (int i = 0; i < bytesReceived; i++)
                received[i] = buf[i];
            return received;
        }

        private void SendMessage(Method method, EndPoint ip)
        {
            string serializeMethod = method.Serialize();
            byte[] methodByte = Encoding.Unicode.GetBytes(serializeMethod.ToCharArray());
            sListener.SendTo(methodByte, ip);
        }
    }
}
