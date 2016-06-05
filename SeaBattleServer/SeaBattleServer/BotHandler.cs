using SeaBattleLibrary;
using System.Threading;

namespace SeaBattleServer
{
    public class BotHandler : Handler
    {
        public Bot Bot
        {
            get
            {
                return Player as Bot;
            }
        }

        public void HitPlayer()
        {
            var enemyHandler = GetEnemyHandler() as PlayerHandler;
            if (enemyHandler == null) return;

            while (Bot.WhoseTurn == Turn.YOUR)
            {
                var killed = Bot.HitEnemy(GetEnemyHandler().Player);
                if (killed.Result == KillResult.Error) continue;

                Game.ReverseTurn(killed.Result);

                Method methodForPlayer = GenerateMethodAfterHitForEnemy(enemyHandler.Player, killed.Result, killed.Address);
                enemyHandler.SendMethod(methodForPlayer);

                Thread.Sleep(1000);

                if (!Player.Map.HasShip())
                {
                    GameOver(this, GetEnemyHandler(), Game);
                    return;
                }
            }
        }
    }
}
