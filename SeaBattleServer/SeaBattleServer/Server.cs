using SeaBattleLibrary;
using SeaBattleLibrary.src.Player;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SeaBattleServer
{
    class Server
    {
        private int port;
        private Socket sListener;
        private List<Game> gameList = new List<Game>();
        private List<Player> playerList = new List<Player>();
        private List<Player> playerBotList = new List<Player>();
        private IPAddress ipAddress;
        private EndPoint localIpEndPoint;
        private object block = new object();

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
                localIpEndPoint = new IPEndPoint(ipAddress, port);
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
                    var message = Encoding.Unicode.GetString(RemoveBytes(dataReceived, byteRec));
                    Method receiveMethod = Method.Deserialize(message);

                    if (find == null)
                    {
                        if (receiveMethod.Name != Method.MethodName.StartGame) continue;
                        Console.WriteLine("Connected: " + ip.ToString());
                        Player player = new Player(ip);
                        playerList.Add(player);
                    }
                    
                    Console.WriteLine("From: " + ip + "; Method: " + receiveMethod);
                    Thread callMethodThread = new Thread(new ParameterizedThreadStart(CallMethod));
                    callMethodThread.IsBackground = true;
                    callMethodThread.Start(new CallMethodParams(ip, receiveMethod));
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
                case Method.MethodName.StartGame:
                    List<Ship> listShip = ParamConvert.GetData<List<Ship>>(method[0]);
                    Game.Regime regime = ParamConvert.GetData<Game.Regime>(method[1]);
                    GetInLineForGame(GetPlayer(ip), listShip, regime);
                    break;
                case Method.MethodName.Exit:
                    Player player = GetPlayer(ip);
                    if (player == null) return;
                    PlayerExit(player);
                    break;
                case Method.MethodName.HitTheEnemy:
                    HitTheEnemy(GetPlayer(ip), ParamConvert.GetData<Address>(method[0]));
                    break;
            }
        }

        private void GetInLineForGame(Player player, List<Ship> listShip, Game.Regime regime)
        {
            foreach (Ship ship in listShip)
            {
                player.Map.AddShip(ship);
            }

            if(regime != Game.Regime.RealPerson)
            {
                Bot bot = null;
                switch(regime)
                {
                    case Game.Regime.NormalBot:
                        bot = new NormalBot(localIpEndPoint);
                        break;
                    case Game.Regime.SmartBot:
                        bot = new SmartBot(localIpEndPoint);
                        break;
                    case Game.Regime.StupidBot:
                        bot = new StupidBot(localIpEndPoint);
                        break;
                }

                bot.SetShips();
                StartGameWithBot(player, bot, regime);
                return;
            }

            if (playerList.Count % 2 == 0)
            {
                lock (block)
                {
                    if (playerList.Count % 2 == 0)
                    {
                        StartGame();
                    }
                }
            }
        }

        private void StartGameWithBot(Player player, Bot bot, Game.Regime regime)
        {
            Game nGame = new Game(player, bot, regime);
            gameList.Add(nGame);
            nGame.SetWhoseTurn(true);

            Console.WriteLine("Start Game. Player: " + nGame.Player1.Ip + "; Bot :" + nGame.Player2.Ip);

            SendTurnToClients(nGame.Player1);
        }

        private void StartGame()
        {

            Game nGame = new Game(playerList[playerList.Count - 2], playerList[playerList.Count - 1], Game.Regime.RealPerson);
            gameList.Add(nGame);
            nGame.SetWhoseTurn(true);

            Console.WriteLine("Start Game. Player1: " + nGame.Player1.Ip + "; Player2 :" + nGame.Player2.Ip);

            SendTurnToClients(nGame.Player1, nGame.Player2);
        }      

        private void SendTurnToClients(params Player[] players)
        {
            foreach (Player player in players)
            {
                Method setTurnMethod = new Method(Method.MethodName.SetTurn, ParamConvert.Convert(player.WhoseTurn));
                SendMethodToClient(setTurnMethod, player.Ip);
            }
        }

        private void HitTheEnemy(Player player, Address address)
        {
            if (player.WhoseTurn != Player.Turn.YOUR)
            {
                SendMethodToClient(new Method(Method.MethodName.Message,
                ParamConvert.Convert("Сейчас не ваш ход")), player.Ip);
                return;
            }

            Player enemy = GetEnemy(player);

            bool withBot = enemy.Ip.Equals(localIpEndPoint);

            KillResult killResult = enemy.Map.Kill(address);

            Game game = GetGame(player);
            ReverseTurn(game, killResult);

            Method methodForYour = GenerateMethodForYour(player, enemy, killResult, address);
            SendMethodToClient(methodForYour, player.Ip);

            if (!withBot)
            {
                Method methodForEnemy = GenerateMethodForYour(player, enemy, killResult, address);
                if (methodForEnemy != null) SendMethodToClient(methodForEnemy, enemy.Ip);
            }

            else
            {
                if (enemy.WhoseTurn == Player.Turn.YOUR)
                {
                    BotHit((Bot)enemy, player);
                }
            }

            if(!enemy.Map.HasShip())
                GameOver(player, enemy);
        }

        private void BotHit(Bot bot, Player player)
        {
            while (bot.WhoseTurn == Player.Turn.YOUR)
            {
                var killed = bot.HitEnemy(player);
                if (killed.result == KillResult.Error) {
                    continue;                   //govnishe
                }
                ReverseTurn(GetGame(player), killed.result);
                Method methodForPlayer = GenerateMethodForEnemy(player, killed.result, killed.address);
                SendMethodToClient(methodForPlayer, player.Ip);

                if (!player.Map.HasShip())
                {
                    GameOver(bot, player);
                    return;
                }
            }
        }

        private Method GenerateMethodForYour(Player player, Player enemy, KillResult killResult, Address address)
        {
            Method methodForYour = new Method(Method.MethodName.SetResultAfterYourHit, 3);
            switch (killResult)
            {
                case KillResult.Error:
                    methodForYour[0] = ParamConvert.Convert("Попробуй ещё раз. (" + address.ToString() + ")");
                    methodForYour[1] = ParamConvert.Convert(player.WhoseTurn);
                    methodForYour[2] = ParamConvert.Convert(enemy.Map.GetStatusFieldsForEnemy());
                    SendMethodToClient(methodForYour, player.Ip);
                    return methodForYour;
                case KillResult.KillShip:
                    methodForYour[0] = ParamConvert.Convert("Ты уничтожил корабль врага! (" + address.ToString() + ")");
                    break;
                case KillResult.KillPartShip:
                    methodForYour[0] = ParamConvert.Convert("Ты повредил корабль врага! (" + address.ToString() + ")");
                    break;
                case KillResult.KillEmpty:
                    methodForYour[0] = ParamConvert.Convert("Ты промазал! (" + address.ToString() + ")");
                    break;
            }
            methodForYour[1] = ParamConvert.Convert(player.WhoseTurn);
            methodForYour[2] = ParamConvert.Convert(enemy.Map.GetStatusFieldsForEnemy());
            Console.WriteLine("Ход{" + player.Ip + ":" + player.WhoseTurn);
            Console.WriteLine("Ход{" + enemy.Ip + ":" + enemy.WhoseTurn);
            return methodForYour;
        }

        private Method GenerateMethodForEnemy(Player enemy, KillResult killResult, Address address)
        {
            Method methodForEnemy = new Method(Method.MethodName.SetResultAfterEnemyHit, 3);
            switch (killResult)
            {
                case KillResult.Error:
                    return null;
                case KillResult.KillShip:
                    methodForEnemy[0] = ParamConvert.Convert("Враг уничтожил твой корабль! (" + address.ToString() + ")");
                    break;
                case KillResult.KillPartShip:
                    methodForEnemy[0] = ParamConvert.Convert("Враг повредил твой корабль! (" + address.ToString() + ")");
                    break;
                case KillResult.KillEmpty:
                    methodForEnemy[0] = ParamConvert.Convert("Противник промазал! (" + address.ToString() + ")");
                    break;
            }
            methodForEnemy[1] = ParamConvert.Convert(enemy.WhoseTurn);
            methodForEnemy[2] = ParamConvert.Convert(enemy.Map.StatusMap);
            return methodForEnemy;
        }

        private void ReverseTurn(Game game, KillResult killResult)
        {
            if (killResult == KillResult.KillEmpty) game.ReverseTurn();
        }

        private void GameOver(Player win, Player loser)
        {
            SendMethodToClient(new Method(Method.MethodName.GameOver, ParamConvert.Convert(Player.Turn.YOUR)), win.Ip);
            SendMethodToClient(new Method(Method.MethodName.GameOver, ParamConvert.Convert(Player.Turn.ENEMY)), loser.Ip);
            Console.WriteLine("Игра закончена!! Win: " + win.Ip.ToString() + ";\nЧистка играков...");
            gameList.Remove(GetGame(win));
            Console.WriteLine("Удаление...:" + win.Ip);
            playerList.Remove(win);
            Console.WriteLine("Удаление...:" + win.Ip);
            playerList.Remove(loser);
        }

        private void PlayerExit(Player player)
        {
            Player enemy = GetEnemy(player);
            SendMethodToClient(new Method(Method.MethodName.GameOver, ParamConvert.Convert("Ваш враг вышел из игры.")), enemy.Ip);
            Console.WriteLine(player.Ip + " вышел из игры.");
            Console.WriteLine(enemy.Ip + " - был врагом " + player.Ip);
            gameList.Remove(GetGame(enemy));
            playerList.Remove(player);
            playerList.Remove(enemy);
        }
   
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

        private class CallMethodParams
        {
            public EndPoint ip;
            public Method method;
            public CallMethodParams(EndPoint ip, Method method)
            {
                this.ip = ip;
                this.method = method;
            }
        }

        private void CallMethod(Object obj)
        {
            CallMethodParams cmc = (CallMethodParams)obj;
            Console.WriteLine("---\nНовый поток для: " + cmc.method.ToString());
            CallMethod(cmc.ip, cmc.method);
            Console.WriteLine("Поток для метода: " + cmc.method.ToString() + " завершился\n---");
        }

        private void SendMethodToClient(Method method, EndPoint ip)
        {
            string serializeMethod = method.Serialize();
            Console.WriteLine("To: " + ip + "; Method: " + method.ToString());
            byte[] methodByte = Encoding.Unicode.GetBytes(serializeMethod.ToCharArray());
            sListener.SendTo(methodByte, ip);
        }
    }
}
