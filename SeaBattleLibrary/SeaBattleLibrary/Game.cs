using SeaBattleLibrary.src.Player;

namespace SeaBattleLibrary
{
    public class Game
    {
        public Player Player1
        {
            get; private set;
        }
        public Player Player2
        {
            get; private set;
        }

        public Regime Regm { get; private set; }
        
        public Game(Player player1, Player player2, Regime regime)
        {
            this.Player1 = player1;
            this.Player2 = player2;
            this.Regm = regime;
        }

        public void ReverseTurn()
        {
            Player.Turn st = Player1.WhoseTurn;
            Player1.WhoseTurn = Player2.WhoseTurn;
            Player2.WhoseTurn = st;
        }
        public void SetWhoseTurn(bool player)
        {
            if (player)
            {
                Player1.WhoseTurn = Player.Turn.YOUR;
                Player2.WhoseTurn = Player.Turn.ENEMY;
            }
            else
            {
                Player2.WhoseTurn = Player.Turn.YOUR;
                Player1.WhoseTurn = Player.Turn.ENEMY;
            }
        }

        public enum Regime
        {
            RealPerson, StupidBot, NormalBot, SmartBot
        }
    }
}
