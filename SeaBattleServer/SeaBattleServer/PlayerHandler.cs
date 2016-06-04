using SeaBattleLibrary;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
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
            TcpClient.SendMethod(new Method(Method.MethodName.GetStartGame));
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
                Console.WriteLine(Player.ID + " отключился");
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
                StartGameWithBot(new BotHandler() { Player = bot }, regime);
                return;
            }

            if (Server.Instance.PlayerHadlerList.Count % 2 == 0)
            {
                lock (block)
                {
                    if (Server.Instance.PlayerHadlerList.Count % 2 == 0)
                    {
                        StartGame();
                    }
                }
            }
        }

        private void PlayerExit()
        {
            var enemyHandler = GetEnemyHandler();
            TcpClient.SendMethod(new Method(Method.MethodName.GameOver, ParamConvert.Convert("Ваш враг вышел из игры.")));
            Console.WriteLine(Player.ID + " вышел из игры.");
            Server.Instance.Games.Remove(Game);
            Server.Instance.PlayerHadlerList.Remove(this);
            Server.Instance.PlayerHadlerList.Remove(enemyHandler);
        }

        private void HitTheEnemy(Address address)
        {
            if (Player.WhoseTurn != Turn.YOUR)
            {
                TcpClient.SendMethod(new Method(Method.MethodName.Message, ParamConvert.Convert("Сейчас не ваш ход")));
                return;
            }

            Handler enemyHandler = GetEnemyHandler();

            KillResult killResult = enemyHandler.Player.Map.Kill(address);
            
            ReverseTurn(killResult);

            Method methodForYour = GenerateMethodForYour(enemyHandler.Player, killResult, address);
            TcpClient.SendMethod(methodForYour);

            if (Game.Regm == GameRegime.RealPerson)
            {
                Method methodForEnemy = GenerateMethodForYour(enemyHandler.Player, killResult, address);
                if (methodForEnemy != null)
                    (GetEnemyHandler() as PlayerHandler).TcpClient.SendMethod(methodForEnemy);
            }
            else
            {
                if (enemyHandler.Player.WhoseTurn == Turn.YOUR)
                {
                    BotHit();
                }
            }

            if (!enemyHandler.Player.Map.HasShip())
                GameOver(this, enemyHandler);
        }

        private void StartGameWithBot(BotHandler botHandler, GameRegime regime)
        {
            Game = new Game(this, botHandler, regime);
            Game.Handler2.Game = Game;

            Server.Instance.Games.Add(Game);
            Game.SetWhoseTurn(true);

            Console.WriteLine("Start Game. Player: " + Game.Handler1.Player.ID + "; Bot :" + Game.Handler2.Player.ID);

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

            Console.WriteLine("Start Game. Player1: " + nGame.Handler1.Player.ID + "; Player2 :" + nGame.Handler2.Player.ID);
            
            SendTurnToClients(nGame.Handler1 as PlayerHandler, nGame.Handler2 as PlayerHandler);
        }

        private Handler GetEnemyHandler()
        {
            return Game.Handler1 == this ? Game.Handler2 : Game.Handler1;
        }
        
        private void ReverseTurn(KillResult killResult)
        {
            if (killResult == KillResult.KillEmpty) Game.ReverseTurn();
        }

        private Method GenerateMethodForYour(Player enemy, KillResult killResult, Address address)
        {
            Method methodForYour = new Method(Method.MethodName.SetResultAfterYourHit, 3);
            switch (killResult)
            {
                case KillResult.Error:
                    methodForYour[0] = ParamConvert.Convert("Попробуй ещё раз. (" + address.ToString() + ")");
                    methodForYour[1] = ParamConvert.Convert(Player.WhoseTurn);
                    methodForYour[2] = ParamConvert.Convert(enemy.Map.GetStatusFieldsForEnemy());
                    TcpClient.SendMethod(methodForYour);
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
            methodForYour[1] = ParamConvert.Convert(Player.WhoseTurn);
            methodForYour[2] = ParamConvert.Convert(enemy.Map.StatusMap);
            Console.WriteLine("Ход{" + Player.ID + ":" + Player.WhoseTurn);
            return methodForYour;
        }

        private Method GenerateMethodForEnemy(Player player, KillResult killResult, Address address)
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
            methodForEnemy[1] = ParamConvert.Convert(player.WhoseTurn);
            methodForEnemy[2] = ParamConvert.Convert(player.Map.StatusMap);
            return methodForEnemy;
        }

        private void BotHit()
        {
            var botHandler = GetEnemyHandler() as BotHandler;


            while (botHandler.Player.WhoseTurn == Turn.YOUR)
            {
                var bot = botHandler.Player as Bot;
                if (bot == null) return;

                var killed = bot.HitEnemy(Player);
                if (killed.result == KillResult.Error)
                {
                    continue;                   //govnishe
                }
                ReverseTurn(killed.result);
                Method methodForPlayer = GenerateMethodForEnemy(Player, killed.result, killed.address);
                TcpClient.SendMethod(methodForPlayer);

                Thread.Sleep(1000);

                if (!Player.Map.HasShip())
                {
                    GameOver(botHandler, this);
                    return;
                }
            }
        }

        private void GameOver(Handler win, Handler loser)
        {
            TcpClient.SendMethod(new Method(Method.MethodName.GameOver, ParamConvert.Convert(this == win ? Turn.YOUR : Turn.ENEMY)));
            if (!(Game.Regm != GameRegime.RealPerson))
                (GetEnemyHandler() as PlayerHandler).TcpClient.SendMethod(new Method(Method.MethodName.GameOver, ParamConvert.Convert(GetEnemyHandler() == win ? Turn.YOUR : Turn.ENEMY)));
            Console.WriteLine("Игра закончена!! Win: " + win.Player.ID);
            Server.Instance.Games.Remove(Game);
            Server.Instance.PlayerHadlerList.Remove(win);
            Server.Instance.PlayerHadlerList.Remove(loser);
        }

        private void SendTurnToClients(params PlayerHandler[] handlers)
        {
            foreach (var handler in handlers)
            {
                if (handler == null) break;
                Method setTurnMethod = new Method(Method.MethodName.SetTurn, ParamConvert.Convert(handler.Player.WhoseTurn));
                TcpClient.SendMethod(setTurnMethod);
            }
        }
    }
}
