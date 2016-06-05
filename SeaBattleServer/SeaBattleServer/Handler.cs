using SeaBattleLibrary;
using System;

namespace SeaBattleServer
{
    public abstract class Handler
    {
        public Player Player { get; set; }
        
        public Game Game { get; set; }

        protected Handler GetEnemyHandler()
        {
            return Game.Handler1 == this ? Game.Handler2 : Game.Handler1;
        }

        protected Method GenerateMethodAfterHitForMe(Player enemy, KillResult killResult, Address address)
        {
            Method methodForYour = new Method(Method.MethodName.SetResultAfterYourHit, 3);
            switch (killResult)
            {
                case KillResult.Error:
                    methodForYour[0] = ParamConvert.Convert("Попробуй ещё раз. (" + address.ToString() + ")");
                    methodForYour[1] = ParamConvert.Convert(Player.WhoseTurn);
                    methodForYour[2] = ParamConvert.Convert(enemy.Map.GetStatusFieldsForEnemy());
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
            methodForYour[2] = ParamConvert.Convert(enemy.Map.GetStatusFieldsForEnemy());
            Console.WriteLine("Ход{" + Player.Nickname + ":" + Player.WhoseTurn);
            return methodForYour;
        }

        protected Method GenerateMethodAfterHitForEnemy(Player player, KillResult killResult, Address address)
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

        protected static void GameOver(Handler win, Handler loser, Game game)
        {
            var winPlayer = win as PlayerHandler;
            if(winPlayer != null) winPlayer
                    .SendMethod(new Method(
                        Method.MethodName.GameOver, 
                        ParamConvert.Convert(Turn.YOUR))
                        );

            var loserPlayer = loser as PlayerHandler;
            if(loserPlayer != null) loserPlayer
                    .SendMethod(new Method(
                        Method.MethodName.GameOver, 
                        ParamConvert.Convert(Turn.ENEMY))
                        );

            Console.WriteLine("Игра закончена!! Win: " + win.Player.Nickname);
            Server.Instance.Games.Remove(game);
            Server.Instance.PlayerHadlerList.Remove(win);
            Server.Instance.PlayerHadlerList.Remove(loser);
        }

        protected static void SendTurnToClients(params PlayerHandler[] handlers)
        {
            foreach (var handler in handlers)
            {
                if (handler == null) break;
                Method setTurnMethod = new Method(Method.MethodName.SetTurn, ParamConvert.Convert(handler.Player.WhoseTurn));
                handler.SendMethod(setTurnMethod);
            }
        }

       
    }
}
