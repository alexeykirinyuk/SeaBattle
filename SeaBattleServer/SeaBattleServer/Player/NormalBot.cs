using System;
using System.Net;


namespace SeaBattleServer
{
    public class NormalBot: Bot
    {
        public override BotKillResult HitEnemy(Player enemy)
        {
            BotKillResult result = new BotKillResult(KillResult.Error);

            if (hitAddress.Count == 0)
            {
                result = HitRandom(enemy);
            }
            else if (hitAddress.Count == 1)
            {
                result = HitAround(enemy, hitAddress[0]);
            }
            else if (hitAddress.Count > 1)
            {
                result = KillShip(enemy);                
            }
            return result;
        }

        
    }
}
