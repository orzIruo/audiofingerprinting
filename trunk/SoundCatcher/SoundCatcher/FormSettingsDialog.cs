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
    public partial class FormSettingsDialog : Form
    {
        public int AudioInputDevice;
        public int SamplesPerSecond;
        public int BytesPerFrame;
        public byte BitsPerSample;
        public byte Channels;

        public FormSettingsDialog()
        {
            InitializeComponent();
        }

        private void FormSettingsDialog_Load(object sender, EventArgs e)
        {
            AudioInputDevice = Properties.Settings.Default.SettingAudioInputDevice;
            comboBox5.SelectedText = AudioInputDevice.ToString();
            SamplesPerSecond = Properties.Settings.Default.SettingSamplesPerSecond;
            comboBox1.SelectedText = SamplesPerSecond.ToString();
            BytesPerFrame = Properties.Settings.Default.SettingBytesPerFrame;
            comboBox2.SelectedText = BytesPerFrame.ToString();
            BitsPerSample = Properties.Settings.Default.SettingBitsPerSample;
            comboBox3.SelectedText = BitsPerSample.ToString();
            Channels = Properties.Settings.Default.SettingChannels;
            comboBox4.SelectedText = Channels.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.SettingAudioInputDevice = AudioInputDevice;
            Properties.Settings.Default.SettingSamplesPerSecond = SamplesPerSecond;
            Properties.Settings.Default.SettingBytesPerFrame = BytesPerFrame;
            Properties.Settings.Default.SettingBitsPerSample = BitsPerSample;
            Properties.Settings.Default.SettingChannels = Channels;
            Properties.Settings.Default.Save();
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
                AudioInputDevice = int.Parse(comboBox5.SelectedItem.ToString());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
                SamplesPerSecond = int.Parse(comboBox1.SelectedItem.ToString());
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
                BytesPerFrame = int.Parse(comboBox2.SelectedItem.ToString());
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
                BitsPerSample = byte.Parse(comboBox3.SelectedItem.ToString());
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
                Channels = byte.Parse(comboBox4.SelectedItem.ToString());
        }
    }
}
