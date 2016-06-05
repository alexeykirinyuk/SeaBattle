using SeaBattleLibrary;

namespace SeaBattleServer
{
    public class Game
    {
        public Handler Handler1 { get; private set; }

        public Handler Handler2 { get; private set;  }
 
        public GameRegime Regm { get; private set; }

        public Game(Handler handler1, Handler handler2, GameRegime regime)
        {
            this.Handler1 = handler1;
            this.Handler2 = handler2;
            this.Regm = regime;
        }

        public void ReverseTurn()
        {
            Turn st = Handler1.Player.WhoseTurn;
            Handler1.Player.WhoseTurn = Handler2.Player.WhoseTurn;
            Handler2.Player.WhoseTurn = st;
        }

        public void ReverseTurn(KillResult killResult)
        {
            if (killResult == KillResult.KillEmpty) ReverseTurn();
        }

        public void SetWhoseTurn(bool player)
        {
            if (player)
            {
                Handler1.Player.WhoseTurn = Turn.YOUR;
                Handler2.Player.WhoseTurn = Turn.ENEMY;
            }
            else
            {
                Handler2.Player.WhoseTurn = Turn.YOUR;
                Handler1.Player.WhoseTurn = Turn.ENEMY;
            }
        }

        
    }
}
