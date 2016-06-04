using System;
using Newtonsoft.Json;

namespace SeaBattleLibrary
{
    [JsonObject("Ship")]
    public class Ship: Param
    {
        [JsonProperty("AddressArray")]
        public Address[] AddressArray { get; private set; }

        [JsonIgnore]
        public int Length
        {
            get
            {
                return AddressArray.Length;
            }
        }

        public Ship() { }

        public Ship(int length)
        {
            AddressArray = new Address[length];
        }

        public Address this[int i]
        {
            get
            {
                return AddressArray[i];
            }

            set
            {
                AddressArray[i] = value;
            }
        }

        public bool isNormalShip()
        {
            if (Length == 1) return true;
            int[] x = new int[Length];
            int[] y = new int[Length];
            int i = 0;
            foreach (Address address in AddressArray)
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
