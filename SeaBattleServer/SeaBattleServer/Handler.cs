
using System.Net.Sockets;

namespace SeaBattleServer
{
    public abstract class Handler
    {
        public Player Player { get; set; }
        
        public Game Game { get; set; }
    }
}
