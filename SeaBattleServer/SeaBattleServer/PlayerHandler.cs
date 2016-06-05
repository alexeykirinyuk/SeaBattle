using SeaBattleLibrary;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SeaBattleServer
{
    public class PlayerHandler : Handler
    {
        private Thread threadListen;
        private bool isListen;
        private object block = new object();

        public TcpClient TcpClient { get; set; }
        
        public void Start()
        {
            threadListen = new Thread(Listen);
            threadListen.IsBackground = true;
            threadListen.Start();
            isListen = true;
            SendMethod(new Method(Method.MethodName.GetStartGame));
        }

        private void Listen()
        {
            try
            {
                while (isListen)
                {
                    var method = TcpClient.ListenMethod();
                    Task task = new Task(() => CallMethod(method));
                    task.Start();
                }
            }
            catch(Exception)
            {
                Console.WriteLine(Player.Nickname + " отключился");
                isListen = false;
                TcpClient.Close();
            }
        }

        ~PlayerHandler()
        {
            TcpClient.Close();
        }

        private void CallMethod(Method method)
        {
            if (method == null) return;

            switch (method.Name)
            {
                case Method.MethodName.StartGame:
                    List<Ship> listShip = ParamConvert.GetData<List<Ship>>(method[0]);
                    GameRegime regime = ParamConvert.GetData<GameRegime>(method[1]);
                    GetInLineForGame(Player, listShip, regime);
                    break;
                case Method.MethodName.Exit:
                    PlayerExit();
                    break;
                case Method.MethodName.HitTheEnemy:
                    HitTheEnemy(ParamConvert.GetData<Address>(method[0]));
                    break;
            }
        }

        private void GetInLineForGame(Player player, List<Ship> listShip, GameRegime regime)
        {
            foreach (Ship ship in listShip)
            {
                player.Map.AddShip(ship);
            }

            Server.Instance.PlayerHadlerList.Add(this);

            if (regime != GameRegime.RealPerson)
            {
                Bot bot = null;
                switch (regime)
                {
                    case GameRegime.NormalBot:
                        bot = new NormalBot();
                        break;
                    case GameRegime.SmartBot:
                        bot = new SmartBot();
                        break;
                    case GameRegime.StupidBot:
                        bot = new StupidBot();
                        break;
                }

                bot.SetShips();
                var botHandler = new BotHandler() { Player = bot };
                Server.Instance.PlayerHadlerList.Add(botHandler);

                StartGameWithBot(botHandler, regime);
                return;
            }

            var count = Server.Instance.PlayerHadlerList.Count;
            if (count % 2 == 0 && count != 0)
            {
                lock (block)
                {
                    var countAfterLock = Server.Instance.PlayerHadlerList.Count;
                    if (countAfterLock % 2 == 0 && countAfterLock != 0)
                    {
                        StartGame();
                    }
                }
            }
        }

        private void PlayerExit()
        {
            var enemyHandler = GetEnemyHandler();
            SendMethod(new Method(Method.MethodName.GameOver, ParamConvert.Convert("Ваш враг вышел из игры.")));
            Console.WriteLine(Player.Nickname + " вышел из игры.");
            Server.Instance.Games.Remove(Game);
            Server.Instance.PlayerHadlerList.Remove(this);
            Server.Instance.PlayerHadlerList.Remove(enemyHandler);
        }

        private void HitTheEnemy(Address address)
        {
            if (Player.WhoseTurn != Turn.YOUR)
            {
                SendMethod(new Method(
                    Method.MethodName.Message, 
                    ParamConvert.Convert("Сейчас не ваш ход"))
                    );
                return;
            }

            Handler enemyHandler = GetEnemyHandler();
            KillResult killResult = enemyHandler.Player.Map.Kill(address);
            Game.ReverseTurn(killResult);

            Method methodForYour = GenerateMethodAfterHitForMe(enemyHandler.Player, killResult, address);
            SendMethod(methodForYour);

            if (Game.Regm == GameRegime.RealPerson)
            {
                Method methodForEnemy = GenerateMethodAfterHitForEnemy(enemyHandler.Player, killResult, address);
                if (methodForEnemy != null)
                    (GetEnemyHandler() as PlayerHandler).SendMethod(methodForEnemy);
            }
            else
            {
                if (enemyHandler.Player.WhoseTurn == Turn.YOUR)
                {
                    (GetEnemyHandler() as BotHandler).HitPlayer();
                }
            }

            if (!enemyHandler.Player.Map.HasShip())
                GameOver(this, enemyHandler, Game);
        }

        private void StartGameWithBot(BotHandler botHandler, GameRegime regime)
        {
            Game = new Game(this, botHandler, regime);
            Game.Handler2.Game = Game;

            Server.Instance.Games.Add(Game);
            Game.SetWhoseTurn(true);

            Console.WriteLine("Start Game. Player: " + Game.Handler1.Player.Nickname + "; Bot :" + Game.Handler2.Player.Nickname);

            SendTurnToClients(Game.Handler1 as PlayerHandler);
        }

        private void StartGame()
        {
            var playerHandlers = Server.Instance.PlayerHadlerList;
            var handler1 = playerHandlers[playerHandlers.Count - 2];

            Game nGame = new Game(handler1, this, GameRegime.RealPerson);
            handler1.Game = Game = nGame;

            Server.Instance.Games.Add(nGame);
            nGame.SetWhoseTurn(true);

            Console.WriteLine("Start Game. Player1: " + nGame.Handler1.Player.Nickname + "; Player2 :" + nGame.Handler2.Player.Nickname);
            
            SendTurnToClients(nGame.Handler1 as PlayerHandler, nGame.Handler2 as PlayerHandler);
        }

        public void SendMethod(Method method)
        {
            try
            {
                var stream = TcpClient.GetStream();
                var strMethod = method.Serialize();
                var byteMethod = Encoding.UTF8.GetBytes(strMethod);

                Console.WriteLine(method + " TO " + Player.Nickname);
                stream.Write(byteMethod, 0, byteMethod.Length);
            }
            catch (Exception)
            {
                Console.WriteLine("Ошибка при отправке сообщения");
            }
        }

        public Method ListenMethod()
        {
            var byteMethod = new byte[TcpClient.ReceiveBufferSize];
            TcpClient.GetStream().Read(byteMethod, 0, byteMethod.Length);
            var strMethod = Encoding.UTF8.GetString(byteMethod);
            var method = Method.Deserialize(strMethod);

            Console.WriteLine(method + " FROM " + Player.Nickname);
            return method;
        }
    }
}
