using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SeaBattleLibrary;

namespace SeaBattleClient
{
    public partial class RegimeDialog : Form
    {
        public GameRegime Result { get; private set; }

        public RegimeDialog()
        {
            InitializeComponent();
        }

        private void buttonWithRealPerson_Click(object sender, EventArgs e)
        {
            Result = GameRegime.RealPerson;
            Close();
        }

        private void buttonBotLvl1_Click(object sender, EventArgs e)
        {
            Result = GameRegime.StupidBot;
            Close();
        }

        private void buttonBotLvl2_Click(object sender, EventArgs e)
        {
            Result = GameRegime.NormalBot;
            Close();
        }

        private void buttonBotLvl3_Click(object sender, EventArgs e)
        {
            Result = GameRegime.SmartBot;
            Close();
        }
    }
}
