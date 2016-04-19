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
        private delegate void LabelTextInvokeDel(string status);
        private delegate void ActionToPicBox(PictureBox box);
        private delegate void Del();

        private const int N = 10;

        private int[] abilityAddShip = {4, 3, 2, 1};
        private int abilityAddField = 0;

        private List<Ship> ShipsBufer = new List<Ship>();
        private List<Address> AddressBufer = new List<Address>(); //тут хранятся адреса для добавления в корабль

        private BattleCient Client;

        public BattleForm()
        {
            InitializeComponent();
        }

        public void SetLabelTurn(String turn)
        {
            labelTurn.Invoke(new LabelTextInvokeDel((str) => labelTurn.Text = str), turn);
        }

        public void ShowMessageBox(string message)
        {
            MessageBox.Show(message);
        }

        public Address GetAddressPicture(PictureBox box)
        {
            return new Address(box.Location.X / 50, box.Location.Y / 50);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panelMy.Enabled = false;
            panelEnemy.Enabled = false;

            buttonSetMap.Enabled = false;
        }

        private void ForAllFields(Panel panel, ActionToPicBox actionToPictureBox)
        {
            foreach (PictureBox picture in panel.Controls.OfType<PictureBox>())
            {
                actionToPictureBox(picture);
            }
        }

        private void buttonAddShip_Click(object sender, EventArgs e)
        {
            Button button = (Button) sender;
            String tagButton = (String) button.Tag;
            int idButton = int.Parse(tagButton);
            SwichOffAllButtonsAdd();
            panelMy.Enabled = true;
            abilityAddField = idButton + 1;
        }

        private void OnMyField_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            Address address = GetAddressPicture(pictureBox);
            if (!address.CanPutShip(ShipsBufer))
            {
                MessageBox.Show("Please put the ship on an empty field. It is impossible to ship standing next to another ship. (See Rules of the Game)");
                return;
            }
            pictureBox.Image = GetImage(StatusField.Ship);
            abilityAddField--;

            AddressBufer.Add(address);

            if (abilityAddField == 0)
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
                    abilityAddShip[AddressBufer.Count - 1]--;
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
                foreach (int i in abilityAddShip)
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
                Address address = GetAddressPicture(pic);
                foreach (Address adr in AddressBufer)
                {
                    if (address.Equals(adr))
                    {
                        pic.Image = GetImage(StatusField.Empty);
                    }
                }
            });
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
            for (int i = 0; i < abilityAddShip.Length; i++)
            {
                if (abilityAddShip[i] > 0) GetButtonAddShip(i + 1).Enabled = true;
            }
        }

        private void SetMap_Click(object sender, EventArgs e)
        {
            buttonCancelShip.Enabled = false;
            Client = new BattleCient("127.0.0.1", 25, this);
            Client.SendShips(ShipsBufer);
            buttonSetMap.Enabled = false;
        }

        private void BattleForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Client != null)
                if (Client.isStart)
                    Client.ClientExit();
        }

        private void buttonResetConnect_Click(object sender, EventArgs e)
        {
            buttonSetMap.Enabled = true;
        }

        private void panelEnemy_Click(object sender, EventArgs e)
        {
            PictureBox picture = (PictureBox) sender;
            Address address = GetAddressPicture(picture);
            Client.HitTheEnemy(address);
        }

        public bool EnemyPanelEnamble
        {
            set
            {
                panelEnemy.Invoke(new Del(() => panelEnemy.Enabled = value));
            }
        }

        private void OnEnemyField_Click(object sender, EventArgs e)
        {
            if (!Client.isStart) return;
            PictureBox pictureBox = (PictureBox)sender;
            Address address = GetAddressPicture(pictureBox);
            Client.HitTheEnemy(address);
        }

        private void SetMap(Panel panel, StatusField[,] map)
        {
            for (int x = 25, i = 0; x < N * 50; x += 50, i++)
            {
                for (int y = 25, j = 0; y < N * 50; y += 50, j++)
                {
                    panel.Invoke(new Del(() => {
                        PictureBox picture = (PictureBox)panel.GetChildAtPoint(new Point(x,y));
                        picture.Image = GetImage(map[i, j]);
                    }));
                }
            }
        }

        public void SetEnemyMap(StatusField[,] map)
        {
            SetMap(panelEnemy, map);
        }

        public void SetMyMap(StatusField[,] map)
        {
            SetMap(panelMy, map);
        }

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
                case StatusField.Ship:
                    resultImage = Properties.Resources.Ship;
                    break;
                case StatusField.DeadShip:
                    resultImage = Properties.Resources.BlowUpShip;
                    break;
                default:
                    resultImage = Properties.Resources.Empty;
                    break;
            }
            return resultImage;
        }

        public void Reboot()
        {
            ForAllFields(panelEnemy, (l) => l.Image = GetImage(StatusField.Empty));
            ForAllFields(panelMy, (l) => l.Image = GetImage(StatusField.Empty));

            panelMy.Invoke(new Del(() => panelMy.Enabled = false));
            panelEnemy.Invoke(new Del(() => panelEnemy.Enabled = false));

            abilityAddShip[0] = 4;
            abilityAddShip[1] = 3;
            abilityAddShip[2] = 2;
            abilityAddShip[3] = 1;
            abilityAddField = 0;

            buttonSetMap.Invoke(new Del(() => buttonSetMap.Enabled = false));
            buttonCancelShip.Invoke(new Del(() => buttonCancelShip.Enabled = true));

            ShipsBufer.Clear();
            AddressBufer.Clear();

            SwichOnAllButtonsAdd();
        }

        private void buttonCancelShip_Click(object sender, EventArgs e)
        {
            if (Client != null) if (Client.isStart) return;
            Reboot();
        }
    }
    
    
}
