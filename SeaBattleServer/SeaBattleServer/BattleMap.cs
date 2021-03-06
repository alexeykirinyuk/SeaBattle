﻿using System.Collections.Generic;
using System.Text;
using SeaBattleLibrary;

namespace SeaBattleServer
{
    public class BattleMap
    {
        private const int N = 10;

        public StatusField[,] StatusMap { get; private set; }

        private List<Ship> _ships;

        #region Indexers
        public StatusField this[Address address]
        {
            get
            {
                return StatusMap[address.I, address.J];    
            }
            private set
            {
                StatusMap[address.I, address.J] = value;
            }
        }

        public StatusField this[int i, int j]
        {
            get
            {
                return StatusMap[i, j];
            }
            private set
            {
                StatusMap[i, j] = value;
            }
        }

        public Ship this[int indexShip]
        {
            get
            {
                return _ships[indexShip];
            }
        }
        #endregion

        public BattleMap()
        {
            StatusMap = new StatusField[N, N];
            _ships = new List<Ship>();

            for (int x = 0; x < N; x++)
            {
                for (int y = 0; y < N; y++)
                {
                    StatusMap[x, y] = StatusField.Empty;
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
            _ships.Add(ship);
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
                    if (IsDeadShip(address))
                    {
                        killed = KillResult.KillShip;
                        SetDeadShip(address);
                    }
                    else
                    {
                        killed = KillResult.KillPartShip;
                    }                    
                    break;
            }
            return killed;
        }

        public StatusField[,] GetStatusFieldsForEnemy()
        {
            StatusField[,] mapForEnemy = new StatusField[N, N];

            for (int x = 0; x < N; x++)
            {
                for (int y = 0; y < N; y++)
                {
                    if (this[x, y] == StatusField.Ship)
                    {
                        mapForEnemy[x, y] = StatusField.Empty;
                    }
                    else
                    {
                        mapForEnemy[x, y] = this[x, y];
                    }
                }
            }
            return mapForEnemy;
        }

        public bool HasShip()
        {
            return _ships.Count > 0;
        }

        private void SetDeadShip(Address address)
        {
            Ship ship = GetShipFromAddressField(address);

            for (int i = 0; i < ship.Length; i++)
            {
                this[ship[i]] = StatusField.DeadShip;
            }
            _ships.Remove(ship);
        }

        private bool IsDeadShip(Address address)
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

            foreach (Ship ship in _ships)
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("");

            for(int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    sb.Append(StatusMap[i, j]).Append(" ");
                }
                sb.Append('\n');
            }

            return sb.ToString();
        }

    }
}
