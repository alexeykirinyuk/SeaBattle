using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SeaBattleClient
{
    public partial class StatisticForm : Form
    {
        private delegate void Del();

        public StatisticForm()
        {
            InitializeComponent();
        }

        public void AddList(string str)
        {
            listBox1.Items.Add(str);
        }
    }
}
