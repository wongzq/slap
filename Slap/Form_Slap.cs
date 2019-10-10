﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Slap
{
    public partial class Form_Slap : Form
    {
        public Form_Slap()
        {
            InitializeComponent();
            btn_NewSort_Click(null, new EventArgs());
        }

        private void ButtonReset()
        {
            btn_MenuNewSort.BackColor = Color.White;
            btn_MenuNewSort.ForeColor = Color.Black;
            btn_MenuSortHistory.BackColor = Color.White;
            btn_MenuSortHistory.ForeColor = Color.Black;
        }

        private void btn_NewSort_Click(object sender, EventArgs e)
        {
            ButtonReset();
            btn_MenuNewSort.BackColor = Color.BlueViolet;
            btn_MenuNewSort.ForeColor = Color.White;
        }

        private void btn_SortHistory_Click(object sender, EventArgs e)
        {
            ButtonReset();
            btn_MenuSortHistory.BackColor = Color.FromArgb(255, 128, 0);
            btn_MenuSortHistory.ForeColor = Color.White;
        }
    }
}