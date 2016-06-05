using System;
using Newtonsoft.Json;

namespace SeaBattleLibrary
{
    [JsonObject("Ship")]
    public class Ship
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

            int[] xArray = new int[Length];
            int[] yArray = new int[Length];

            int i = 0;

            foreach (var address in AddressArray)
            {
                xArray[i] = address.I;
                yArray[i] = address.J;
                i++;
            }

            bool xEqual = true;
            bool yEqual = true;

            for (var m = 0; m < Length - 1; m++)
            {
                xEqual &= xArray[m] == xArray[m + 1];
                yEqual &= yArray[m] == yArray[m + 1];
            }

            if (!(xEqual || yEqual)) return false;

            bool isNormal = true;

            if (xEqual)
            {
                Array.Sort(yArray);
                for (int k = 0; k < yArray.Length - 1; k++)
                {
                    isNormal &= Math.Abs(yArray[k] - yArray[k + 1]) == 1;
                }
            }
            if (yEqual)
            {
                Array.Sort(xArray);
                for (int k = 0; k < xArray.Length - 1; k++)
                {
                    isNormal &= Math.Abs(xArray[k] - xArray[k + 1]) == 1;
                }
            }
            return isNormal;
        }
    }
}
