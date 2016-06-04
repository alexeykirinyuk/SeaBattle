using System.Net;
using SeaBattleLibrary;

namespace SeaBattleServer
{
    public class Player
    {
        public BattleMap Map { get; private set; }

        private static int countId = 0;

        public int ID { get; set; } = countId++;

        public Turn WhoseTurn { get; set; }

        public Player()
        {
            Map = new BattleMap();
        }

        public Player(BattleMap map)
        {
            Map = map;
        }
    }
}
