using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;

namespace SeaBattleLibrary
{
    public class Game
    {
        private Player player1;
        private Player player2;

        public Player Player1
        {
            get
            {
                return player1;
            }
        }
        public Player Player2
        {
            get
            {
                return player2;
            }
        }

        public Game() { }
        public Game(Player player1, Player player2)
        {
            this.player1 = player1;
            this.player2 = player2;
        }
        public void ReverseTurn()
        {
            Player.Turn st = player1.WhoseTurn;
            player1.WhoseTurn = player2.WhoseTurn;
            player2.WhoseTurn = st;
        }
        public void SetWhoseTurn(bool player)
        {
            if (player)
            {
                player1.WhoseTurn = Player.Turn.YOUR;
                player2.WhoseTurn = Player.Turn.ENEMY;
            }
            else
            {
                player2.WhoseTurn = Player.Turn.YOUR;
                player1.WhoseTurn = Player.Turn.ENEMY;
            }
        }
    }
}
