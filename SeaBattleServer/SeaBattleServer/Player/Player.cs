namespace SeaBattleServer
{
    public class Player
    {
        private static int _countId = 0;

        public BattleMap Map { get; private set; }

        private int _id;

        public string Nickname
        {
            get
            {
                return "Player_" + _id; 
            }
        }

        public Turn WhoseTurn { get; set; }

        public Player()
        {
            Map = new BattleMap();
            _id = _countId++;
        }

        public Player(BattleMap map)
        {
            Map = map;
            _id = _countId++;
        }
    }
}
