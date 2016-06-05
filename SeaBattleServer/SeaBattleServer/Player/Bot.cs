using System;
using System.Collections.Generic;
using SeaBattleLibrary;

namespace SeaBattleServer
{
    public abstract class Bot : Player
    {
        protected static readonly Random random = new Random();
        protected int[] _abilityAddShip = { 4, 3, 2, 1 };

        protected List<Address> hitAddress = new List<Address>();
        protected List<Address> deadAddress = new List<Address>();
        

        #region generate map
        public void SetShips()
        {
            for (int countFields = 0; countFields < _abilityAddShip.Length; countFields++)
            {
                for (int countShips = 0; countShips < _abilityAddShip[countFields]; countShips++)
                {
                    Ship ship = new Ship(countFields + 1);
                    bool sw = false;
                    Console.WriteLine(countFields + 1);
                    while (!sw)
                    {
                        sw = GenerateShip(ship);
                    }

                    for(int i = 0; i < 10; i++)
                    {
                        for(int j = 0; j < 10; j++)
                        {
                            Console.Write((Map[i, j] == StatusField.Empty? "0" : "1") + " ");
                        }
                        Console.Write("\n");
                    }

                }
            }
        }

        private bool GenerateShip(Ship ship)
        {
            bool res = false;
            int i = random.Next(0,9);
            int j = random.Next(0, 9);
            Address address = new Address(i, j);
            if(!isNormalAddress(address))
            {
                return false;
            }
            if(HasRight(address, ship.Length))
            {
                int ck = 1;
                ship[0] = address;
                for(int k = j + 1; k < j + ship.Length; k++)
                {
                    ship[ck] = new Address(i, k);
                    ck++;
                }
                res = true;
            }
            else if (HasTop(address, ship.Length))
            {
                int ck = 1;
                ship[0] = address;
                for (int k = i - 1; k > i - ship.Length; k--)
                {
                    ship[ck] = new Address(k, j);
                    ck++;
                }
                res = true;
            }
            else if(HasLeft(address, ship.Length))
            {
                int ck = 1;
                ship[0] = address;
                for (int k = j - 1; k > j - ship.Length; k--)
                {
                    ship[ck] = new Address(i, k);
                    ck++;
                }
                res = true;
            }
            else if(HasDown(address, ship.Length))
            {
                int ck = 1;
                ship[0] = address;
                for (int k = i + 1; k < i + ship.Length; k++)
                {
                    ship[ck] = new Address(k, j);
                    ck++;
                }
                res = true;
            }
            if(res)
            {
                Map.AddShip(ship);
            }
            return res;
        }

        private bool isNormalAddress(Address address)
        {
            int i = address.I;
            int j = address.J;
            bool res = true;
            res &= Map[i, j] == StatusField.Empty;
            if(j + 1 <= 9) res &= Map[i, j + 1] == StatusField.Empty;
            if(j - 1 >= 0) res &= Map[i, j - 1] == StatusField.Empty;
            if(i + 1 <= 9) res &= Map[i + 1, j] == StatusField.Empty;
            if(i - 1 >= 0) res &= Map[i - 1, j] == StatusField.Empty;

            if (i + 1 <= 9 && j + 1 <= 9) res &= Map[i + 1, j + 1] == StatusField.Empty;
            if (i + 1 <= 9 && j - 1 >= 0) res &= Map[i + 1, j - 1] == StatusField.Empty;
            if (i - 1 >= 0 && j + 1 <= 9) res &= Map[i - 1, j + 1] == StatusField.Empty;
            if (i - 1 >= 0 && j - 1 >= 0) res &= Map[i - 1, j - 1] == StatusField.Empty;
            return res; 
        }

        private bool HasRight(Address address, int count)
        {
            bool res = true;
            if (!isNormalAddress(address)) return false;
            if (address.J + count - 1 > 9 || address.J + count - 1 < 0) return false;
            for(int j = address.J; j < address.J + count; j++)
            {
                res &= isNormalAddress(new Address(address.I, j));
                if (!res) return res;
            }
            return res;
        }

        private bool HasLeft(Address address, int count)
        {
            bool res = isNormalAddress(address);
            if (!res) return res;
            if (address.J - count + 1 > 9 || address.J - count + 1 < 0) return false;
            for (int j = address.J; j > address.J - count; j--)
            {
                res &= isNormalAddress(new Address(address.I, j));
                if (!res) return res;
            }
            return res;
        }

        private bool HasTop(Address address, int count)
        {
            bool res = isNormalAddress(address);
            if (!res) return res;
            if (address.I - count + 1 > 9 || address.I - count + 1 < 0) return false;
            for (int i = address.I; i > address.I - count; i--)
            {
                res &= isNormalAddress(new Address(i, address.J));
                if (!res) return res;
            }
            return res;
        }

        private bool HasDown(Address address, int count)
        {
            bool res = isNormalAddress(address);
            if (!res) return res;
            if (address.I + count - 1 > 9 || address.J + count - 1 < 0) return false;
            for (int i = address.I; i < address.I + count; i++)
            {
                res &= isNormalAddress(new Address(i, address.J));
                if (!res) return res;
            }
            return res;
        }
        #endregion

        #region HitEnemy
        public abstract BotKillResult HitEnemy(Player enemy);

        protected BotKillResult HitAround(Player enemy, Address address)
        {
            BotKillResult botKillResult = new BotKillResult(KillResult.Error);
            
            bool left = address.J > 0;
            bool right = address.J < 9;
            bool top = address.I > 0;
            bool down = address.I < 9;
            Address addressLeft = null;
            Address addressRight = null;
            Address addressTop = null;
            Address addressDown = null;

            if (left) addressLeft = new Address(address.I, address.J - 1);
            if (right) addressRight = new Address(address.I, address.J + 1);
            if (top) addressTop = new Address(address.I - 1, address.J);
            if (down) addressDown = new Address(address.I + 1, address.J);

            left = left ? !deadAddress.Contains(addressLeft) : false;
            right = right ? !deadAddress.Contains(addressRight) : false;
            top = top ? !deadAddress.Contains(addressTop) : false;
            down = down ? !deadAddress.Contains(addressDown) : false;


            if (left)
            {
                botKillResult.Result = enemy.Map.Kill(addressLeft);
                if (botKillResult.Result == KillResult.KillPartShip) hitAddress.Add(addressLeft);
                else if (botKillResult.Result == KillResult.KillShip) hitAddress.Clear();
                botKillResult.Address = addressLeft;
                deadAddress.Add(addressLeft);
            }
            else if (right)
            {
                botKillResult.Result = enemy.Map.Kill(addressRight);
                if (botKillResult.Result == KillResult.KillPartShip) hitAddress.Add(addressRight);
                else if (botKillResult.Result == KillResult.KillShip) hitAddress.Clear();
                botKillResult.Address = addressRight;
                deadAddress.Add(addressRight);
            }
            else if (top)
            {
                botKillResult.Result = enemy.Map.Kill(addressTop);
                if (botKillResult.Result == KillResult.KillPartShip) hitAddress.Add(addressTop);
                else if (botKillResult.Result == KillResult.KillShip) hitAddress.Clear();
                botKillResult.Address = addressTop;
                deadAddress.Add(addressTop);
            }
            else if (down)
            {
                botKillResult.Result = enemy.Map.Kill(addressDown);
                if (botKillResult.Result == KillResult.KillPartShip) hitAddress.Add(addressDown);
                else if (botKillResult.Result == KillResult.KillShip) hitAddress.Clear();
                botKillResult.Address = addressDown;
                deadAddress.Add(addressDown);
            }
            else
            {
                if (down) botKillResult = HitAround(enemy, addressDown);
                else if (top) botKillResult = HitAround(enemy, addressTop);
                else if (left) botKillResult = HitAround(enemy, addressLeft);
                else if (right) botKillResult = HitAround(enemy, addressRight);
                else botKillResult = HitRandom(enemy);
            }
            return botKillResult;
        }

        protected BotKillResult HitRandom(Player enemy)
        {
            BotKillResult killed = new BotKillResult(KillResult.Error);
            Address address = GetRandomAddress();
            bool hasDead = deadAddress.Contains(address);
            if (hasDead)
            {
                return HitAround(enemy, address);
            }

            killed.Result = enemy.Map.Kill(address);
            if (killed.Result == KillResult.KillPartShip) hitAddress.Add(address);
            else if (killed.Result == KillResult.KillShip) hitAddress.Clear();
            deadAddress.Add(address);
            killed.Address = address;
            return killed;
        }

        protected Address GetRandomAddress()
        {
            int i = random.Next(0, 9);
            int j = random.Next(0, 9);
            return new Address(i, j);
        }

        protected BotKillResult KillShip(Player enemy)
        {
            BotKillResult killed = new BotKillResult(KillResult.Error);
            int[] i = new int[hitAddress.Count];
            int[] j = new int[hitAddress.Count];
            for (int m = 0; m < hitAddress.Count; m++)
            {
                i[m] = hitAddress[m].I;
                j[m] = hitAddress[m].J;
            }

            bool equalsI = true;
            Array.ForEach(i, new Action<int>((k) => { equalsI &= k == i[0]; }));
            bool equalsJ = true;
            Array.ForEach(j, new Action<int>((k) => { equalsJ &= k == j[0]; }));

            if (equalsI)
            {
                int max = j[0];
                int min = j[0];
                Array.ForEach(j, new Action<int>((k) => { if (max < k) max = k; }));
                Array.ForEach(j, new Action<int>((k) => { if (min > k) min = k; }));

                bool MaxNormalAddress = ++max <= 9;
                bool MinNormalAddress = --min >= 0;
                Address addressMax = null;
                Address addressMin = null;

                if (MaxNormalAddress) addressMax = new Address(i[0], max);
                if (MinNormalAddress) addressMin = new Address(i[0], min);

                if (MaxNormalAddress && !deadAddress.Contains(addressMax))
                {
                    killed.Result = enemy.Map.Kill(addressMax);
                    if (killed.Result == KillResult.KillPartShip) hitAddress.Add(addressMax);
                    else if (killed.Result == KillResult.KillShip) hitAddress.Clear();
                    killed.Address = addressMax;
                    deadAddress.Add(addressMax);
                }
                else if(MinNormalAddress && !deadAddress.Contains(addressMin))
                {
                    killed.Result = enemy.Map.Kill(addressMin);
                    if (killed.Result == KillResult.KillPartShip) hitAddress.Add(addressMin);
                    else if (killed.Result == KillResult.KillShip) hitAddress.Clear();
                    killed.Address = addressMin;
                    deadAddress.Add(addressMin);
                }
            }
            else
            {
                int max = i[0];
                int min = i[0];
                Array.ForEach(i, new Action<int>((k) => { if (max < k) max = k; }));
                Array.ForEach(i, new Action<int>((k) => { if (min > k) min = k; }));

                bool MaxNormalAddress = ++max <= 9;
                bool MinNormalAddress = --min >= 0;
                Address addressMax = null;
                Address addressMin = null;

                if (MaxNormalAddress) addressMax = new Address(max, j[0]);
                if (MinNormalAddress) addressMin = new Address(min, j[0]);

                if (MaxNormalAddress && !deadAddress.Contains(addressMax))
                {
                    killed.Result = enemy.Map.Kill(addressMax);
                    if (killed.Result == KillResult.KillPartShip) hitAddress.Add(addressMax);
                    else if (killed.Result == KillResult.KillShip) hitAddress.Clear();
                    deadAddress.Add(addressMax);
                    killed.Address = addressMax;
                }
                else if (MinNormalAddress && !deadAddress.Contains(addressMin))
                {
                    killed.Result = enemy.Map.Kill(addressMin);
                    if (killed.Result == KillResult.KillPartShip) hitAddress.Add(addressMin);
                    else if (killed.Result == KillResult.KillShip) hitAddress.Clear();
                    deadAddress.Add(addressMin);
                    killed.Address = addressMin;
                }
                else { return killed; }
            }
            return killed;
        }

        protected enum Where
        {
            Left, Right, Top, Down, Error
        }
        #endregion

        public struct BotKillResult
        {
            public Address Address { get; set; }

            public KillResult Result { get; set; }

            public BotKillResult(Address address, KillResult result)
            {
                this.Address = address;
                this.Result = result;
            }

            public BotKillResult(KillResult result)
            {
                Address = null;
                this.Result = result;
            }
        }

    }
}
