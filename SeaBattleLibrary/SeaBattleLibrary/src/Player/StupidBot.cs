using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace SeaBattleLibrary.src.Player
{
    public class StupidBot : Bot
    {

        public StupidBot() : base() { }

        public StupidBot(EndPoint ipServer) : base(ipServer) { }

        public override BotKillResult HitEnemy(Player enemy)
        {
            return HitRandom(enemy);
        }
    }
}
