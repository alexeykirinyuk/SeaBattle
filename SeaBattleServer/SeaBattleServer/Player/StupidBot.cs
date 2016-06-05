namespace SeaBattleServer
{
    public class StupidBot : Bot
    {
        public override BotKillResult HitEnemy(Player enemy)
        {
            return HitRandom(enemy);
        }
    }
}
