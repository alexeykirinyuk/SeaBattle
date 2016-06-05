using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace SeaBattleLibrary
{
    [JsonObject("Address")]
    public class Address: IEquatable<Address>
    {
        [JsonProperty("I")]
        private int _i;

        [JsonProperty("J")]
        private int _j;
        
        [JsonIgnore]
        public int I
        {
            get
            {
                return _i;
            }
            set
            {
                if (value > 10 || value < 0) new IncorrectIndexException(0, 10);
                else _i = value;
            }
        }

        [JsonIgnore]
        public int J
        {
            get
            {
                return _j;
            }

            set
            {
                if (value > 10 || value < 0) new IncorrectIndexException(0, 10);
                else _j = value;
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
            else return _i == address._i && _j == address._j;
        }

        public bool CanPutShip(List<Ship> addedShips)
        {
            bool can = true;
            foreach (Ship ship in addedShips)
            {
                for (int index = 0; index < ship.Length; index++)
                {
                    can &= !(Equals(ship[index]));
                    bool xEqualYBeside = _i == ship[index]._i && Math.Abs(_j - ship[index]._j) == 1;
                    bool yEqualXBeside = _j == ship[index]._j && Math.Abs(_i - ship[index]._i) == 1;
                    bool RightTop = _i - 1 == ship[index]._i && _j + 1== ship[index]._j;
                    bool LeftTop = _i - 1 == ship[index]._i && _j - 1 == ship[index]._j;
                    bool RightDown = _i + 1 == ship[index]._i && _j + 1 == ship[index]._j;
                    bool LeftDown = _i + 1 == ship[index]._i && _j - 1 == ship[index]._j;
                    can &= !(xEqualYBeside || yEqualXBeside || RightDown || RightTop || LeftDown || LeftTop);
                }
            }
            return can;
        }

        public override int GetHashCode()
        {
            return _i*10+_j;
        }

        public override string ToString()
        {
            int iAbs = _i;
            char iChar = (char)((int)'А' + iAbs);
            return iChar + (_j+1).ToString();
        }
    }
}
