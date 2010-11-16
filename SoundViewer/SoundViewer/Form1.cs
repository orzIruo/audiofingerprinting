/* Copyright (C) 2007 Jeff Morton jeffrey.raymond.morton@gmail.com
 
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

namespace SoundViewer
{
    public partial class Form1 : Form
    {
        private WaveInRecorder _recorder;
        private byte[] _recorderBuffer;
        private WaveOutPlayer _player;
        private byte[] _playerBuffer;
        private FifoStream _stream;
        private WaveFormat _waveFormat;
        private AudioFrame _audioFrame;
        private int _audioSamplesPerSecond = 44100;
        private int _audioFrameSize = 16384;
        private byte _audioBitsPerSample = 16;
        private byte _audioChannels = 2;
        private bool _isPlayer = false;
        private bool _isTest = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (WaveNative.waveInGetNumDevs() == 0)
            {
                textBox1.AppendText(DateTime.Now.ToString() + " : There are no audio devices available\r\n");
            }
            else
            {
                if (_isPlayer == true)
                    _stream = new FifoStream();
                _audioFrame = new AudioFrame(_isTest);
                Start();
            }
        }

        private void Start()
        {
            Stop();
            try
            {
                _waveFormat = new WaveFormat(_audioFrameSize, _audioBitsPerSample, _audioChannels);
                _recorder = new WaveInRecorder(0, _waveFormat, _audioFrameSize * 2, 3, new BufferDoneEventHandler(DataArrived));
                if (_isPlayer == true)
                    _player = new WaveOutPlayer(-1, _waveFormat, _audioFrameSize * 2, 3, new BufferFillEventHandler(Filler));

                textBox1.AppendText(DateTime.Now.ToString() + " : Audio device initialized\r\n");
                textBox1.AppendText(DateTime.Now.ToString() + " : Audio device polling started\r\n");
                textBox1.AppendText(DateTime.Now + " : Samples per second = " + _audioSamplesPerSecond.ToString() + "\r\n");
                textBox1.AppendText(DateTime.Now + " : Frame size = " + _audioFrameSize.ToString() + "\r\n");
                textBox1.AppendText(DateTime.Now + " : Bits per sample = " + _audioBitsPerSample.ToString() + "\r\n");
                textBox1.AppendText(DateTime.Now + " : Channels = " + _audioChannels.ToString() + "\r\n");
            }
            catch (Exception ex)
            {
                textBox1.AppendText(DateTime.Now + " : Audio exception\r\n" + ex.ToString() + "\r\n");
            }
        }

        private void Stop()
        {
            if (_recorder != null)
                try
                {
                    _recorder.Dispose();
                }
                finally
                {
                    _recorder = null;
                }
            if (_isPlayer == true)
            {
                if (_player != null)
                    try
                    {
                        _player.Dispose();
                    }
                    finally
                    {
                        _player = null;
                    }
                _stream.Flush(); // clear all pending data
            }
        }

        private void Filler(IntPtr data, int size)
        {
            if (_isPlayer == true)
            {
                if (_playerBuffer == null || _playerBuffer.Length < size)
                    _playerBuffer = new byte[size];
                if (_stream.Length >= size)
                    _stream.Read(_playerBuffer, 0, size);
                else
                    for (int i = 0; i < _playerBuffer.Length; i++)
                        _playerBuffer[i] = 0;
                System.Runtime.InteropServices.Marshal.Copy(_playerBuffer, 0, data, size);
            }
        }

        private void DataArrived(IntPtr data, int size)
        {
            if (_recorderBuffer == null || _recorderBuffer.Length < size)
                _recorderBuffer = new byte[size];
            if (_recorderBuffer != null)
            {
                System.Runtime.InteropServices.Marshal.Copy(data, _recorderBuffer, 0, size);
                if (_isPlayer == true)
                    _stream.Write(_recorderBuffer, 0, _recorderBuffer.Length);
                _audioFrame.Process(ref _recorderBuffer);
                _audioFrame.RenderTimeDomain(ref pictureBox1);
                _audioFrame.RenderFrequencyDomain(ref pictureBox2);
            }
        }
    }
}