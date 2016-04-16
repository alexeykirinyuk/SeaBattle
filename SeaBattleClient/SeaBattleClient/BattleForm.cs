using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using SeaBattleLibrary;

namespace SeaBattleClient
{
    public partial class BattleForm : Form
    {
        delegate void ActionToPictureBox(PictureBox label);
        public delegate void SetStatusDelegate(string status);

        private const int N = 10;

        private int[] AbilityAddShip = new int[4];
        private int AbilityAddField = 0;

        private List<Ship> ShipsBufer = new List<Ship>();
        private List<Address> AddressBufer = new List<Address>(); //тут хранятся адреса для добавления в корабль

        private BattleCient Client;

        public BattleForm()
        {
            InitializeComponent();
        }

        public void SetLabelStatus(String status)
        {
            labelStatus.Invoke(new SetStatusDelegate((str) => labelStatus.Text = str), status);
        }

        public void ShowMessageBox(string message)
        {
            MessageBox.Show(message);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ForAllFields(panelEnemy, (l) => l.Image = GetImage(StatusField.Empty));
            ForAllFields(panelMy, (l) => l.Image = GetImage(StatusField.Empty));

            foreach (PictureBox pictureBox in panelMy.Controls.OfType<PictureBox>())
            {
                pictureBox.Click += OnMyField_Click;
            }
            foreach (PictureBox pictureBox in panelEnemy.Controls.OfType<PictureBox>())
            {
                pictureBox.Click += OnEnemyField_Click;
            }

            SetTagForPicture(panelMy);
            SetTagForPicture(panelEnemy);
            panelMy.Enabled = false;
            panelEnemy.Enabled = false;

            AbilityAddShip[0] = 4;
            AbilityAddShip[1] = 3;
            AbilityAddShip[2] = 2;
            AbilityAddShip[3] = 1;

            buttonSetMap.Enabled = false;
        }

        private void ForAllFields(Panel panel, ActionToPictureBox actionToPictureBox)
        {
            foreach (PictureBox picture in panel.Controls.OfType<PictureBox>())
            {
                actionToPictureBox(picture);
            }
        }

        private void SetTagForPicture(Panel panel)
        {
            int i = 0;
            for (int x = 0; x < N * 50; x += 50)
            {
                int j = 0;
                for (int y = 0; y < N * 50; y += 50)
                {
                    PictureBox picture = (PictureBox) panel.GetChildAtPoint(new Point(x, y));
                    picture.Tag = i + " " +  j;
                    j++;
                }
                i++;
                
            }
        }

        private void buttonAddShip_Click(object sender, EventArgs e)
        {
            Button button = (Button) sender;
            String tagButton = (String) button.Tag;
            int idButton = int.Parse(tagButton);
            SwichOffAllButtonsAdd();
            panelMy.Enabled = true;
            AbilityAddField = idButton + 1;
        }

        private void OnMyField_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            Address address = GetAddressFromTag((string)pictureBox.Tag);
            if (!IsEmptyField(address))
            {
                MessageBox.Show("Please put the ship on an empty field. It is impossible to ship standing next to another ship. (See Rules of the Game)");
                return;
            }
            pictureBox.Image = GetImage(StatusField.PartShip);
            AbilityAddField--;

            AddressBufer.Add(address);

            if (AbilityAddField == 0)
            {
                Ship ship = new Ship(AddressBufer.Count);
                int i = 0;
                foreach (Address addr in AddressBufer)
                {
                    ship[i] = addr;
                    i++;
                }

                if (!ship.isNormalShip())
                {
                    MessageBox.Show("Please put the ship in the right way. (See Rules of the Game)");
                    CancelAddressBuffer();
                }
                else
                {
                    ShipsBufer.Add(ship);
                    AbilityAddShip[AddressBufer.Count - 1]--;
                }
                AddressBufer.Clear();
                panelMy.Enabled = false;
                SwichOnAllButtonsAdd();
                if (IsPlacingOnShips)
                {
                    buttonSetMap.Enabled = true;
                }
            }
        }

        private bool IsPlacingOnShips
        {
            get
            {
                bool all = true;
                foreach (int i in AbilityAddShip)
                {
                    all &= i == 0;
                }
                return all;
            }
        }

        private void CancelAddressBuffer()
        {
            ForAllFields(panelMy, (pic) =>
            {
                Address address = GetAddressFromTag((string)pic.Tag);
                foreach (Address adr in AddressBufer)
                {
                    if (address.equal(adr))
                    {
                        pic.Image = GetImage(StatusField.Empty);
                    }
                }
            });
        }

        private Address GetAddressFromTag(string obj)
        {
            string[] addressString = obj.Split(' ');
            Address address = new Address(int.Parse(addressString[0]), int.Parse(addressString[1]));
            return address;
        }

        private Button GetButtonAddShip(int count)
        {
            Button button = null;
            switch (count)
            {
                case 1:
                    button = buttonAddOnesShip;
                    break;
                case 2:
                    button = buttonAddTwosShip;
                    break;
                case 3:
                    button = buttonAddThreesShip;
                    break;
                case 4:
                    button = buttonAddFoursShip;
                    break;
            }
            return button;
        }

        private void SwichOffAllButtonsAdd()
        {
            buttonAddOnesShip.Enabled = false;
            buttonAddTwosShip.Enabled = false;
            buttonAddThreesShip.Enabled = false;
            buttonAddFoursShip.Enabled = false;
        }

        private void SwichOnAllButtonsAdd()
        {
            for (int i = 0; i < AbilityAddShip.Length; i++)
            {
                if (AbilityAddShip[i] > 0) GetButtonAddShip(i + 1).Enabled = true;
            }
        }

        private bool IsEmptyField(Address address)
        {
            bool normal = true;
            foreach (Ship ship in ShipsBufer)
            {
                for (int i = 0; i < ship.Length; i++)
                {
                    normal &= !(address.x == ship[i].x && address.y == ship[i].y);
                    bool xEqualYBeside = address.x == ship[i].x && Math.Abs(address.y - ship[i].y) == 1;
                    bool yEqualXBeside = address.y == ship[i].y && Math.Abs(address.x - ship[i].x) == 1;
                    normal &= !(xEqualYBeside || yEqualXBeside);
                }
            }
            return normal;
        }

        private void SetMap_Click(object sender, EventArgs e)
        {
            Client = new BattleCient("127.0.0.1", 25);
            Client.SetBattleForm(this);
            Client.StartClient();
            Client.SendShips(ShipsBufer);
            buttonSetMap.Enabled = false;
        }

        private void BattleForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Client != null)
                if (Client.isStart)
                    Client.Exit();
        }

        private void buttonResetConnect_Click(object sender, EventArgs e)
        {
            buttonSetMap.Enabled = true;
        }

        private void panelEnemy_Click(object sender, EventArgs e)
        {
            PictureBox picture = (PictureBox) sender;
            Address address = GetAddressFromTag((string) picture.Tag);
            Client.HitTheEnemy(address);
        }

        public void MyTurn()
        {
            panelEnemy.Enabled = true;
        }

        public void EnemyTurn()
        {
            panelEnemy.Enabled = false;
        }

        private void OnEnemyField_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            Address address = GetAddressFromTag((string)pictureBox.Tag);
            Client.HitTheEnemy(address);
        }

        private void SetMap(Panel panel, StatusField[,] map)
        {
            PictureBox[] pictures = (PictureBox[])panel.Invoke(new GetPicureBoxesFromPanel(() => { 
                return panel.Controls.OfType<PictureBox>(); 
            }));
            foreach (PictureBox pictureBox in pictures)
            {
                Address address = GetAddressFromTag((string) pictureBox.Tag);
                pictureBox.Image = GetImage(map[address.x, address.y]);
            }
        }

        public void SetEnemyMap(StatusField[,] map)
        {
            SetMap(panelEnemy, map);
        }

        public delegate Object GetPicureBoxesFromPanel();

        private Image GetImage(StatusField statusField)
        {
            Image resultImage;
            switch (statusField)
            {
                case StatusField.DeadEmpty:
                    resultImage = Properties.Resources.KilledEmpty;
                    break;
                case StatusField.DeadPartShip:
                    resultImage = Properties.Resources.KilledShip;
                    break;
                case StatusField.Empty:
                    resultImage = Properties.Resources.Empty;
                    break;
                case StatusField.PartShip:
                    resultImage = Properties.Resources.Ship;
                    break;
                default:
                    resultImage = Properties.Resources.Empty;
                    break;
            }
            return resultImage;
        }
    }
}
