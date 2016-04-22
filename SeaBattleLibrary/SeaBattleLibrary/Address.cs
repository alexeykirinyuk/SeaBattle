using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SeaBattleLibrary
{
    [JsonObject("Address")]
    public class Address: IEquatable<Address>
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

        public override bool Equals(object obj)
        {
            return base.Equals((Address)obj);
        }

        public bool Equals(Address address)
        {
            if (address == null) return false;
            else return i == address.i && j == address.j;
        }

        public bool CanPutShip(List<Ship> addedShips)
        {
            bool can = true;
            foreach (Ship ship in addedShips)
            {
                for (int index = 0; index < ship.Length; index++)
                {
                    can &= !(Equals(ship[index]));
                    bool xEqualYBeside = i == ship[index].i && Math.Abs(j - ship[index].j) == 1;
                    bool yEqualXBeside = j == ship[index].j && Math.Abs(i - ship[index].i) == 1;
                    bool RightTop = i - 1 == ship[index].i && j + 1== ship[index].j;
                    bool LeftTop = i - 1 == ship[index].i && j - 1 == ship[index].j;
                    bool RightDown = i + 1 == ship[index].i && j + 1 == ship[index].j;
                    bool LeftDown = i + 1 == ship[index].i && j - 1 == ship[index].j;
                    can &= !(xEqualYBeside || yEqualXBeside || RightDown || RightTop || LeftDown || LeftTop);
                }
            }
            return can;
        }

        public override int GetHashCode()
        {
            return i*10+j;
        }

        public override string ToString()
        {
            int iAbs = Math.Abs(i);
            char iChar = (char)((int)'А' + iAbs);
            return iChar + (j+1).ToString();
        }
    }
}
