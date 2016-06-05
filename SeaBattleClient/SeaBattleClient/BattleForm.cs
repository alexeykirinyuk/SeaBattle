using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SeaBattleLibrary;

namespace SeaBattleClient
{
    public partial class BattleForm : Form
    {
        private const int N = 10;

        private int[] _abilityAddShip;
        private int _abilityAddField;

        private List<Ship> _shipsBuf;
        private List<Address> _addressBuf;

        private BattleCient _client;

        private StatisticForm _statisticForm;

        private delegate void ActionToPicBox(PictureBox box);
        private delegate void Del();

        public BattleForm()
        {
            InitializeComponent();
            _abilityAddShip = new int[] { 4, 3, 2, 1 };
            _abilityAddField = 0;
            _shipsBuf = new List<Ship>();
            _addressBuf = new List<Address>();
            _statisticForm = new StatisticForm();

        }

        public string LabelTurnText
        {
            set
            {
                labelTurn.Invoke(new Del(() => labelTurn.Text = value));
            }
        }

        public bool EnemyPanelEnamble
        {
            set
            {
                panelEnemy.Invoke(new Del(() => panelEnemy.Enabled = value));
            }
        }

        private int countLineInBattleDialog = 0;

        public void BattleDialog(string message)
        {
            labelLastAction.Invoke(new Del(() => {
                if (countLineInBattleDialog++ < 10) 
                    labelLastAction.Text += message + "\n";
                else
                {
                    labelLastAction.Text = message + "\n";
                    countLineInBattleDialog = 0;
                }
                }));
            _statisticForm.AddList(message);
           
        }

        public void SetEnemyMap(StatusField[,] map)
        {
            SetMap(panelEnemy, map);
        }

        public void SetMyMap(StatusField[,] map)
        {
            SetMap(panelMy, map);
        }

        public void Reboot()
        {
            ForAllFields(panelEnemy, (l) => l.Image = GetImage(StatusField.Empty));
            ForAllFields(panelMy, (l) => l.Image = GetImage(StatusField.Empty));

            panelMy.Invoke(new Del(() => panelMy.Enabled = false));
            panelEnemy.Invoke(new Del(() => panelEnemy.Enabled = false));

            _abilityAddShip[0] = 4;
            _abilityAddShip[1] = 3;
            _abilityAddShip[2] = 2;
            _abilityAddShip[3] = 1;
            _abilityAddField = 0;

            buttonStartGame.Invoke(new Del(() => buttonStartGame.Enabled = false));
            buttonCancelShip.Invoke(new Del(() => buttonCancelShip.Enabled = true));
            labelLastAction.Invoke(new Del(() => labelLastAction.Text = ""));
            labelTurn.Invoke(new Del(() => labelTurn.Text = ""));

            _shipsBuf.Clear();
            _addressBuf.Clear();

            SwichOnAllButtonsAdd();
        }

        #region GUI listeners
        private void BattleForm_Load(object sender, EventArgs e)
        {
            panelMy.Enabled = false;
            panelEnemy.Enabled = false;

            buttonStartGame.Enabled = false;
        }

        private void buttonAddShip_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            String tagButton = (String)button.Tag;
            int idButton = int.Parse(tagButton);
            SwichOffAllButtonsAdd();
            panelMy.Enabled = true;
            _abilityAddField = idButton + 1;
        }

        private void onMyField_Click(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            Address address = GetAddressPicture(pictureBox);
            if (!address.CanPutShip(_shipsBuf))
            {
                MessageBox.Show("Пожалуйста, поставьте корабль в правильное место (читать правила игры)");
                CancelAddressBuffer();
                return;
            }
            pictureBox.Image = GetImage(StatusField.Ship);
            _abilityAddField--;

            _addressBuf.Add(address);

            if (_abilityAddField == 0)
            {
                Ship ship = new Ship(_addressBuf.Count);
                int i = 0;
                foreach (Address addr in _addressBuf)
                {
                    ship[i] = addr;
                    i++;
                }

                if (!ship.isNormalShip())
                {
                    MessageBox.Show("Пожалуйста, расставляйте корабли правильно (по прямой линии)");
                    CancelAddressBuffer();
                }
                else
                {
                    _shipsBuf.Add(ship);
                    _abilityAddShip[_addressBuf.Count - 1]--;
                }
                _addressBuf.Clear();
                panelMy.Enabled = false;
                SwichOnAllButtonsAdd();
                if (IsPlacingOnShips)
                {
                    buttonStartGame.Enabled = true;
                }
            }
        }

        private void OnEnemyField_Click(object sender, EventArgs e)
        {
            if (!_client.IsStart) return;
            PictureBox pictureBox = (PictureBox)sender;
            Address address = GetAddressPicture(pictureBox);
            _client.HitTheEnemy(address);
        }

        private void startGame_Click(object sender, EventArgs e)
        {
            buttonCancelShip.Enabled = false;
            SwichOffAllButtonsAdd();
            RegimeDialog dialog = new RegimeDialog();
            dialog.ShowDialog();

            _client = new BattleCient(this, _shipsBuf, dialog.Result);
            buttonStartGame.Enabled = false;
        }

        private void BattleForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_client != null)
                if (_client.IsStart)
                    _client.ClientExit();
        }

        private void buttonCancelShip_Click(object sender, EventArgs e)
        {
            if (_client != null) if (_client.IsStart) return;
            Reboot();
        }

        private void getStatisticForm_Click(object sender, EventArgs e)
        {
            _statisticForm.ShowDialog();
        }

        private void buttonNewGame_Click(object sender, EventArgs e)
        {
            Reboot();
            _client.ClientExit();
        }
        #endregion

        private void ForAllFields(Panel panel, ActionToPicBox actionToPictureBox)
        {
            foreach (PictureBox picture in panel.Controls.OfType<PictureBox>())
            {
                actionToPictureBox(picture);
            }
        }

        private void SetMap(Panel panel, StatusField[,] map)
        {
            for (int x = 25, i = 0; x < N * 50; x += 50, i++)
            {
                for (int y = 25, j = 0; y < N * 50; y += 50, j++)
                {
                    panel.Invoke(new Del(() => {
                        PictureBox picture = (PictureBox)panel.GetChildAtPoint(new Point(y, x));
                        picture.Image = GetImage(map[i, j]);
                    }));
                }
            }
        }

        private bool IsPlacingOnShips
        {
            get
            {
                bool all = true;
                foreach (int i in _abilityAddShip)
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
                foreach (Address adr in _addressBuf)
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

        private Address GetAddressPicture(PictureBox box)
        {
            return new Address(box.Location.Y / 50, box.Location.X / 50);
        }

        private Image GetImage(StatusField statusField)
        {
            Image resultImage;
            switch (statusField)
            {
                case StatusField.DeadEmpty:
                    resultImage = Properties.Resources.DeadEmpty;
                    break;
                case StatusField.DeadPartShip:
                    resultImage = Properties.Resources.DeadPartShip;
                    break;
                case StatusField.Empty:
                    resultImage = Properties.Resources.Empty;
                    break;
                case StatusField.Ship:
                    resultImage = Properties.Resources.Ship;
                    break;
                case StatusField.DeadShip:
                    resultImage = Properties.Resources.DeadShip;
                    break;
                default:
                    resultImage = Properties.Resources.Empty;
                    break;
            }
            return resultImage;
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
            for (int i = 0; i < _abilityAddShip.Length; i++)
            {
                if (_abilityAddShip[i] > 0) GetButtonAddShip(i + 1).Enabled = true;
            }
        }
    }
}
