using System;
using Newtonsoft.Json;

namespace SeaBattleLibrary
{
    [JsonObject("Ship")]
    public class Ship: Param
    {
        [JsonProperty("AddressArray")]
        private Address[] addressArray;

        [JsonIgnore]
        public int Length
        {
            get
            {
                return addressArray.Length;
            }
        }
        internal Address[] AddressArray
        {
            get
            {
                return addressArray;
            }
        }

        public Ship() { }
        public Ship(int length)
        {
            addressArray = new Address[length];
        }

        public Address this[int i]
        {
            get
            {
                return addressArray[i];
            }

            set
            {
                addressArray[i] = value;
            }
        }
        public bool isNormalShip()
        {
            if (Length == 1) return true;
            int[] x = new int[Length];
            int[] y = new int[Length];
            int i = 0;
            foreach (Address address in addressArray)
            {
                x[i] = address.I;
                y[i] = address.J;
                i++;
            }

            bool xEqual = true;
            bool yEqual = true;

            for (int m = 0; m < Length - 1; m++)
            {
                xEqual &= x[m] == x[m + 1];
                yEqual &= y[m] == y[m + 1];
            }

            if (!(xEqual || yEqual)) return false;

            bool isNormal = true;
            if (xEqual)
            {
                Array.Sort(y);
                for (int k = 0; k < y.Length - 1; k++)
                {
                    isNormal &= Math.Abs(y[k] - y[k + 1]) == 1;
                }
            }
            if (yEqual)
            {
                Array.Sort(x);
                for (int k = 0; k < x.Length - 1; k++)
                {
                    isNormal &= Math.Abs(x[k] - x[k + 1]) == 1;
                }
            }
            return isNormal;
        }
    }
}
