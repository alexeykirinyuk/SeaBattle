using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SeaBattleLibrary
{
    [JsonObject("Address")]
    public class Address: Param
    {
        [JsonProperty("I")]
        private int i;
        [JsonProperty("J")]
        private int j;
        
        [JsonIgnore]
        public int I
        {
            get
            {
                return i;
            }
            set
            {
                if (value >= 10 || value < 0) new IncorrectIndexException(0, 10);
                else i = value;
            }
        }
        [JsonIgnore]
        public int J
        {
            get
            {
                return j;
            }

            set
            {
                if (value >= 10 || value < 0) new IncorrectIndexException(0, 10);
                else j = value;
            }
        }

        public Address() {
            
        }
        public Address(int i, int j)
        {
            this.I = i;
            this.J = j;
        }

        public bool Equals(Address address)
        {
            return i == address.i && j == address.j;
        }
        public override string ToString()
        {
            return "Address";
        }

        public bool CanPutShip(List<Ship> addedShips)
        {
            bool can = true;
            foreach (Ship ship in addedShips)
            {
                for (int i = 0; i < ship.Length; i++)
                {
                    can &= !(Equals(ship[i]));
                    bool xEqualYBeside = this.i == ship[i].i && Math.Abs(j - ship[i].j) == 1;
                    bool yEqualXBeside = j == ship[i].j && Math.Abs(this.i - ship[i].i) == 1;
                    can &= !(xEqualYBeside || yEqualXBeside);
                }
            }
            return can;
        }
    }
}
