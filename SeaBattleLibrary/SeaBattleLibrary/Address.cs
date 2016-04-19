using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SeaBattleLibrary
{
    [JsonObject("A")]
    public class Address: Param
    {
        [JsonProperty("X")]
        private int x;
        [JsonProperty("Y")]
        private int y;
        
        [JsonIgnore]
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                if (value >= 10 || value < 0) new IncorrectIndexException(0, 10);
                else x = value;
            }
        }
        [JsonIgnore]
        public int Y
        {
            get
            {
                return y;
            }

            set
            {
                if (value >= 10 || value < 0) new IncorrectIndexException(0, 10);
                else y = value;
            }
        }

        public Address() {
            
        }
        public Address(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.type = "A";
        }

        public bool Equals(Address address)
        {
            return x == address.x && y == address.y;
        }

        public bool CanPutShip(List<Ship> addedShips)
        {
            bool can = true;
            foreach (Ship ship in addedShips)
            {
                for (int i = 0; i < ship.Length; i++)
                {
                    can &= !(Equals(ship[i]));
                    bool xEqualYBeside = x == ship[i].x && Math.Abs(y - ship[i].y) == 1;
                    bool yEqualXBeside = y == ship[i].y && Math.Abs(x - ship[i].x) == 1;
                    can &= !(xEqualYBeside || yEqualXBeside);
                }
            }
            return can;
        }
    }
}
