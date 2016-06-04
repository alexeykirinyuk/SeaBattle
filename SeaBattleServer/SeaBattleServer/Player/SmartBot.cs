using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SeaBattleServer
{
    public class SmartBot: Bot
    {
        private int indexShip = 0;

        public override BotKillResult HitEnemy(Player enemy)
        {
            var killed = new BotKillResult(KillResult.Error);
            var rand = random.Next(0, 100);
            var mapEnemy = enemy.Map;
            if (rand < 50)
            {
                var hitShip = mapEnemy[indexShip];
                for(int i = 0; i < hitShip.Length; i++)
                {
                    if (!deadAddress.Contains(hitShip[i]))
                    {
                        killed.address = hitShip[i];
                        killed.result = enemy.Map.Kill(hitShip[i]);
                        deadAddress.Add(hitShip[i]);
                        hitAddress.ForEach(adr =>
                        {
                            if (adr == hitShip[i]) hitAddress.Clear();
                            return;
                        });
                        return killed.result == KillResult.Error ? HitEnemy(enemy) : killed;
                    }
                }
                return HitEnemy(enemy);
            }
            else
            {
                if (hitAddress.Count == 0)
                {
                    killed = HitRandom(enemy);
                }
                else if (hitAddress.Count == 1)
                {
                    killed = HitAround(enemy, hitAddress[0]);
                }
                else if (hitAddress.Count > 1)
                {
                    killed = KillShip(enemy);
                }
                return killed;
            }
            
        }
    }
}
