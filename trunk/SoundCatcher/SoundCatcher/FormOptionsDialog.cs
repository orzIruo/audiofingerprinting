/* Copyright (C) 2008 Jeff Morton (jeffrey.raymond.morton@gmail.com)
 
   This program is free software; you can redistribute it and/or modify
   it under the terms of the GNU General Public License as published by
   the Free Software Foundation; either version 2 of the License, or
   (at your option) any later version.

   This program is distributed in the hope that it will be useful,
   but WITHOUT ANY WARRANTY; without even the implied warranty of
   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
   GNU General Public License for more details.
 
   You should have received a copy of the GNU General Public License
   along with this program; if not, write to the Free Software
   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace SoundCatcher
{
    public partial class FormOptionsDialog : Form
    {
        public bool IsDetectingEvents;
        public bool IsSaving;
        public int AmplitudeThreshold;
        public int SecondsToSave;
        public string OutputPath;

        public FormOptionsDialog()
        {
            InitializeComponent();
        }

        private void FormOptionsDialog_Load(object sender, EventArgs e)
        {
            IsDetectingEvents = Properties.Settings.Default.SettingIsDetectingEvents;
            checkBox1.Checked = IsDetectingEvents;
            IsSaving = Properties.Settings.Default.SettingIsSaving;
            checkBox2.Checked = IsSaving;
            AmplitudeThreshold = Properties.Settings.Default.SettingAmplitudeThreshold;
            textBox2.Text = AmplitudeThreshold.ToString();
            trackBar1.Value = AmplitudeThreshold;
            OutputPath = Properties.Settings.Default.SettingOutputPath;
            textBox1.Text = OutputPath;
            SecondsToSave = Properties.Settings.Default.SettingSecondsToSave;
            numericUpDown1.Value = SecondsToSave;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SettingIsDetectingEvents = IsDetectingEvents;
            Properties.Settings.Default.SettingIsSaving = IsSaving;
            Properties.Settings.Default.SettingAmplitudeThreshold = AmplitudeThreshold;
            Properties.Settings.Default.SettingOutputPath = OutputPath;
            Properties.Settings.Default.SettingSecondsToSave = SecondsToSave;
            Properties.Settings.Default.Save();
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            AmplitudeThreshold = trackBar1.Value;
            textBox2.Text = trackBar1.Value.ToString();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            IsDetectingEvents = checkBox1.Checked;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            IsSaving = checkBox2.Checked;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            SecondsToSave = (int)numericUpDown1.Value;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            textBox1.Text = folderBrowserDialog1.SelectedPath;
            OutputPath = folderBrowserDialog1.SelectedPath;
        }
    }
}
