using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace SeaBattleLibrary
{
    public class BattleMap
    {
        private StatusField[,] statusMap = new StatusField[N, N];
        private List<Ship> ships = new List<Ship>();
        private const int N = 10;

        public StatusField this[Address address]
        {
            get
            {
                return statusMap[address.X, address.Y];
            }
            private set
            {
                statusMap[address.X, address.Y] = value;
            }
        }
        public StatusField this[int x, int y]
        {
            get
            {
                return statusMap[x, y];
            }
            private set
            {
                statusMap[x, y] = value;
            }
        }
        public Ship this[int indexShip]
        {
            get
            {
                return ships[indexShip];
            }
        }
        public StatusField[,] StatusMap
        {
            get
            {
                return statusMap;
            }
        }

        public BattleMap()
        {
            for (int x = 0; x < N; x++)
            {
                for (int y = 0; y < N; y++)
                {
                    statusMap[x, y] = StatusField.Empty;
                }
            }
        }

        public void AddShip(Address[] addressAray)
        {
            Ship ship = new Ship(addressAray.Length);
            int i = 0;
            foreach (Address address in addressAray)
            {
                this[address] = StatusField.Ship;
                ship[i] = address;
                i++;
            }
            ships.Add(ship);
        }
        public void AddShip(Ship ship)
        {
            AddShip(ship.AddressArray);
        }
        public KillResult Kill(Address address)
        {
            KillResult killed = KillResult.Error;
            switch (this[address])
            {
                case StatusField.Empty:
                    this[address] = StatusField.DeadEmpty;
                    killed = KillResult.KillEmpty;
                    break;
                case StatusField.Ship:
                    this[address] = StatusField.DeadPartShip;
                    if (isDeadShip(address))
                    {
                        killed = KillResult.KillShip;
                        SetDeadShip(address);
                    }
                    killed = KillResult.KillPartShip;
                    break;
            }
            return killed;
        }
        public BattleMap GetMapForEnemy()
        {
            BattleMap mapForEnemy = new BattleMap();
            for (int x = 0; x < N; x++)
            {
                for (int y = 0; y < N; y++)
                {
                    if (mapForEnemy[x, y] == StatusField.Ship)
                    {
                        mapForEnemy[x, y] = StatusField.Empty;
                    }
                    else
                    {
                        mapForEnemy[x, y] = mapForEnemy[x, y];
                    }
                }
            }
            return mapForEnemy;
        }
        public bool HasShip()
        {
            return ships.Count != 0;
        }

        private void SetDeadShip(Address address)
        {
            Ship ship = GetShipFromAddressField(address);

            for (int i = 0; i < ship.Length; i++)
            {
                this[ship[i]] = StatusField.DeadShip;
            }
            ships.Remove(ship);
        }
        private bool isDeadShip(Address address)
        {
            Ship killedShip = GetShipFromAddressField(address);

            bool result = killedShip.Length != 0;
            for (int i = 0; i < killedShip.Length; i++)
            {
                result &= this[killedShip[i]] == StatusField.DeadPartShip;
            }
            return result;
        }

        private Ship GetShipFromAddressField(Address address)
        {
            Ship resultShip = new Ship(0);
            foreach (Ship ship in ships)
            {
                bool has = false;
                for (int i = 0; i < ship.Length; i++)
                {
                    if (ship[i].Equals(address)) has = true;
                }
                if (has)
                {
                    resultShip = ship;
                    break;
                }
            }
            return resultShip;
        }

    }
}
