using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeaBattleLibrary
{
    class BattleMap
    {
        private StatusField[,] OwnMap;
        private List<Ship> Ships = new List<Ship>();

        private const int N = 10;

        public StatusField this[Address address] {
            get
            {
                return OwnMap[address.x, address.y];
            }
        }

        public BattleMap()
        {
            OwnMap = new StatusField[N, N];
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    OwnMap[i, j] = StatusField.Empty;
                }
            }
        }

        public void AddShip(Address[] adrs)
        {
            Ship ship = new Ship(adrs.Length);
            int i = 0;
            foreach (Address ad in adrs)
            {
                OwnMap[ad.x, ad.y] = StatusField.PartShip;
                ship[i] = ad;
                i++;
            }

            Ships.Add(ship);
        }

        /*
         * 1 - Ship Killed
         * 0 - Field Killed 
         * -1 - Empty Killed
         **/
        public int Kill(Address address)
        {
            int killed = -1;
            switch (OwnMap[address.x, address.y])
            {
                case StatusField.Empty:
                    OwnMap[address.x, address.y] = StatusField.DeadEmpty;
                    killed = -1;
                    break;
                case StatusField.PartShip:
                    SetStatusToField(address, StatusField.DeadPartShip);
                    if(isKilled(address)) {
                        killed = 1;
                        SetKillShip(address);
                    }
                    killed = 0;
                    break;
            }


            return killed;
        }

        public BattleMap GetMapForOpponent()
        {
            BattleMap map = new BattleMap();
            for (int i = 0; i < N; i++)
            {
                for(int j = 0; j < N; j++) {
                    if (OwnMap[i, j] == StatusField.PartShip)
                    {
                        map.OwnMap[i, j] = StatusField.Empty;
                    }
                    else
                    {
                        map.OwnMap[i, j] = OwnMap[i, j];
                    }
                }
            }
            return map;
        }

        private void SetStatusToField(Address address, StatusField status)
        {
            OwnMap[address.x, address.y] = status;
        }

        private void SetKillShip(Address address)
        {
            Ship ship = GetShipFromAddressField(address);

            for (int i = 0; i < ship.Length; i++)
            {
                SetStatusToField(ship[i], StatusField.KilledShip);
            }
        }

        private bool isKilled(Address address)
        {
            Ship killedShip = GetShipFromAddressField(address);

            var result = killedShip.Length != 0;
            for (int i = 0; i < killedShip.Length; i++)
            {
                result &= OwnMap[killedShip[i].x, killedShip[i].y] == StatusField.DeadPartShip;
            }
            return result;
        }

        private Ship GetShipFromAddressField(Address address)
        {
            Ship killedShip = new Ship(0);
            foreach (Ship ship in Ships)
            {
                bool has = false;
                for (int i = 0; i < ship.Length; i++)
                {
                    if (ship[i].x == address.x && ship[i].y == address.y) has = true;
                }
                if (has)
                {
                    killedShip = ship;
                    break;
                }
            }
            return killedShip;
        }

        public void Print()
        {
            Console.WriteLine("-----------------------Your map---------------------");
            PrintMap(OwnMap);
            Console.WriteLine("\n\n-------------Your map for the enemy-------------");
            PrintMap(GetMapForOpponent().OwnMap);
        }

        private void PrintMap(StatusField[,] statusFields)
        {
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    Console.Write(statusFields[i, j].ToString() + "(" + i + "," + j + ")\t");
                }
                Console.WriteLine();
            }
        }
    }

    [Serializable]
    class Address
    {
        public int x { get; set; }
        public int y { get; set; }
        
        public Address(int x,int y) {
            if(x < 0 || x > 10 || y < 0 || y > 10) throw new AddressIndexOverException();
            this.x = x;
            this.y = y;
        }

        public bool equal(Address adr)
        {
            return x == adr.x && y == adr.y;
        }
    }

    [Serializable]
    class Ship
    {
        public Address[] address { get; set; }

        public int Length
        {
            get
            {
                return address.Length;
            }
        }

        public Ship(int length) {
            address = new Address[length];
        }
        
        public Address this[int i]
        {
            get
            {
                return address[i];
            }

            set
            {
                address[i] = value;
            }
        }

        public bool isNormalShip()
        {
            if (Length == 1) return true;
            int[] x = new int[Length];
            int[] y = new int[Length];
            int i = 0;
            foreach (Address adr in address)
            {
                x[i] = adr.x;
                y[i] = adr.y;
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
            bool result = true;
            if (xEqual)
            {
                Array.Sort(y);
                for (int k = 0; k < y.Length - 1; k++)
                {
                    result &= Math.Abs(y[k] - y[k + 1]) == 1;
                }
            }
            if (yEqual)
            {
                Array.Sort(x);
                for (int k = 0; k < x.Length - 1; k++)
                {
                    result &= Math.Abs(x[k] - x[k + 1]) == 1;
                }
            }
            return result;
        }
    }


    public enum StatusField
    {
        Empty, PartShip, DeadPartShip, DeadEmpty, KilledShip
    }

    public class AddressIndexOverException: Exception {
    }

    private class Method
    {
        public string MethodName { get; set; }
        public string[] Parameters { get; set; }

        public Method() { }
        public Method(string MethodName, string[] Parameters)
        {
            this.MethodName = MethodName;
            this.Parameters = Parameters;
        }
    }
}
