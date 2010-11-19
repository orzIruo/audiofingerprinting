/* Copyright (C) 2011
 
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
using System.IO;

namespace SoundCatcher
{
    public partial class FormMain : Form
    {
        private WaveInRecorder _recorder;
        private byte[] _recorderBuffer;
        private WaveOutPlayer _player;
        private byte[] _playerBuffer;
        private WaveFormat _waveFormat;
        private AudioFrame _audioFrame;
        private FifoStream _streamOut;
        private FifoStream _framesStreamOut;
        private MemoryStream _streamMemory;
        private Stream _streamWave;
        private FileStream _streamFile;
        private bool _isPlayer = false;  // audio output for testing
        private bool _isTest = false;  // signal generation for testing
        private bool _isSaving = false;
        private bool _isShown = true;
        private string _sampleFilename;
        private DateTime _timeLastDetection;
        private System.Threading.Timer _drawTimer;

        public FormMain()
        {
            InitializeComponent();
        }
        private void FormMain_Load(object sender, EventArgs e)
        {
            if (WaveNative.waveInGetNumDevs() == 0)
            {
                textBoxConsole.AppendText(DateTime.Now.ToString() + " : No audio input devices detected\r\n");
            }
            else
            {
                _drawTimer = new System.Threading.Timer(DrawData, null, 1000, 100);
                textBoxConsole.AppendText(DateTime.Now.ToString() + " : Audio input device detected\r\n");
                if (_isPlayer == true)
                    _streamOut = new FifoStream();
                _bufferSize = (int)Math.Round(0.0116 * Properties.Settings.Default.SettingSamplesPerSecond *
                    Properties.Settings.Default.SettingBitsPerSample / 8);
                _framesStreamOut = new FifoStream();
                _audioFrame = new AudioFrame(_isTest);
                _audioFrame.IsDetectingEvents = Properties.Settings.Default.SettingIsDetectingEvents;
                _audioFrame.AmplitudeThreshold = Properties.Settings.Default.SettingAmplitudeThreshold;
                _streamMemory = new MemoryStream();
                Start();
            }
        }
        private void FormMain_Resize(object sender, EventArgs e)
        {
            if (_audioFrame != null)
            {
                _audioFrame.RenderTimeDomainLeft(ref pictureBoxTimeDomainLeft);
                _audioFrame.RenderTimeDomainRight(ref pictureBoxTimeDomainRight);
                _audioFrame.RenderFrequencyDomainLeft(ref pictureBoxFrequencyDomainLeft, Properties.Settings.Default.SettingSamplesPerSecond);
                _audioFrame.RenderFrequencyDomainRight(ref pictureBoxFrequencyDomainRight, Properties.Settings.Default.SettingSamplesPerSecond);
                _audioFrame.RenderSpectrogramLeft(ref pictureBoxSpectrogramLeft);
                _audioFrame.RenderSpectrogramRight(ref pictureBoxSpectrogramRight);
            }
        }
        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
            if (_isShown & this.WindowState == FormWindowState.Minimized)
            {
                foreach (Form f in this.MdiChildren)
                {
                    f.WindowState = FormWindowState.Normal;
                }
                this.ShowInTaskbar = false;
                this.Visible = false;
                notifyIcon1.Visible = true;
                _isShown = false;
            }
        }
        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Stop();
            if (_isSaving == true)
            {
                byte[] waveBuffer = new byte[Properties.Settings.Default.SettingBitsPerSample];
                _streamWave = WaveStream.CreateStream(_streamMemory, _waveFormat);
                waveBuffer = new byte[_streamWave.Length - _streamWave.Position];
                _streamWave.Read(waveBuffer, 0, waveBuffer.Length);
                if (Properties.Settings.Default.SettingOutputPath != "")
                    _streamFile = new FileStream(Properties.Settings.Default.SettingOutputPath + "\\" + _sampleFilename, FileMode.Create);
                else
                    _streamFile = new FileStream(_sampleFilename, FileMode.Create);
                _streamFile.Write(waveBuffer, 0, waveBuffer.Length);
                _isSaving = false;
            }
            if (_streamOut != null)
                try
                {
                    _streamOut.Close();
                }
                finally
                {
                    _streamOut = null;
                }
            if (_framesStreamOut != null)
                try
                {
                    _framesStreamOut.Close();
                }
                finally
                {
                    _framesStreamOut = null;
                }
            if (_streamWave != null)
                try
                {
                    _streamWave.Close();
                }
                finally
                {
                    _streamWave = null;
                }
            if (_streamFile != null)
                try
                {
                    _streamFile.Close();
                }
                finally
                {
                    _streamFile = null;
                }
            if (_streamMemory != null)
                try
                {
                    _streamMemory.Close();
                }
                finally
                {
                    _streamMemory = null;
                }
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            notifyIcon1.Visible = false;
            this.Visible = true;
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
            _isShown = true;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormOptionsDialog form = new FormOptionsDialog();
            if (form.ShowDialog() == DialogResult.OK)
            {
                _audioFrame.IsDetectingEvents = form.IsDetectingEvents;
                _audioFrame.AmplitudeThreshold = form.AmplitudeThreshold;
            }
        }
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormSettingsDialog form = new FormSettingsDialog();
            if (form.ShowDialog() == DialogResult.OK)
            {
                Stop();
                if (_isSaving == true)
                {
                    byte[] waveBuffer = new byte[Properties.Settings.Default.SettingBitsPerSample];
                    _streamWave = WaveStream.CreateStream(_streamMemory, _waveFormat);
                    waveBuffer = new byte[_streamWave.Length - _streamWave.Position];
                    _streamWave.Read(waveBuffer, 0, waveBuffer.Length);
                    _streamFile = new FileStream(_sampleFilename, FileMode.Create);
                    _streamFile.Write(waveBuffer, 0, waveBuffer.Length);
                    _isSaving = false;
                }
                if (_streamOut != null)
                    try
                    {
                        _streamOut.Close();
                    }
                    finally
                    {
                        _streamOut = null;
                    }
                if (_streamWave != null)
                    try
                    {
                        _streamWave.Close();
                    }
                    finally
                    {
                        _streamWave = null;
                    }
                if (_streamFile != null)
                    try
                    {
                        _streamFile.Close();
                    }
                    finally
                    {
                        _streamFile = null;
                    }
                if (_streamMemory != null)
                    try
                    {
                        _streamMemory.Close();
                    }
                    finally
                    {
                        _streamMemory = null;
                    }
                if (_isPlayer == true)
                    _streamOut = new FifoStream();
                _framesStreamOut = new FifoStream();
                _audioFrame = new AudioFrame(_isTest);
                _audioFrame.IsDetectingEvents = Properties.Settings.Default.SettingIsDetectingEvents;
                _audioFrame.AmplitudeThreshold = Properties.Settings.Default.SettingAmplitudeThreshold;
                _streamMemory = new MemoryStream();
                Start();
            }
        }
        private void Start()
        {
            Stop();
            try
            {
                _waveFormat = new WaveFormat(Properties.Settings.Default.SettingSamplesPerSecond, Properties.Settings.Default.SettingBitsPerSample, Properties.Settings.Default.SettingChannels);
                _recorder = new WaveInRecorder(Properties.Settings.Default.SettingAudioInputDevice, _waveFormat, Properties.Settings.Default.SettingBytesPerFrame * Properties.Settings.Default.SettingChannels, 3, new BufferDoneEventHandler(DataArrived));
                if (_isPlayer == true)
                    _player = new WaveOutPlayer(Properties.Settings.Default.SettingAudioOutputDevice, _waveFormat, Properties.Settings.Default.SettingBytesPerFrame * Properties.Settings.Default.SettingChannels, 3, new BufferFillEventHandler(Filler));
                textBoxConsole.AppendText(DateTime.Now.ToString() + " : Audio input device polling started\r\n");
                textBoxConsole.AppendText(DateTime.Now + " : Device = " + Properties.Settings.Default.SettingAudioInputDevice.ToString() + "\r\n");
                textBoxConsole.AppendText(DateTime.Now + " : Channels = " + Properties.Settings.Default.SettingChannels.ToString() + "\r\n");
                textBoxConsole.AppendText(DateTime.Now + " : Bits per sample = " + Properties.Settings.Default.SettingBitsPerSample.ToString() + "\r\n");
                textBoxConsole.AppendText(DateTime.Now + " : Samples per second = " + Properties.Settings.Default.SettingSamplesPerSecond.ToString() + "\r\n");
                textBoxConsole.AppendText(DateTime.Now + " : Frame size = " + Properties.Settings.Default.SettingBytesPerFrame.ToString() + "\r\n");
            }
            catch (Exception ex)
            {
                textBoxConsole.AppendText(DateTime.Now + " : " + ex.InnerException.ToString() + "\r\n");
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
                _streamOut.Flush(); // clear all pending data
                _framesStreamOut.Flush();
            }
            textBoxConsole.AppendText(DateTime.Now.ToString() + " : Audio input device polling stopped\r\n");
        }
        private void Filler(IntPtr data, int size)
        {
            if (_isPlayer == true)
            {
                if (_playerBuffer == null || _playerBuffer.Length < size)
                    _playerBuffer = new byte[size];
                if (_streamOut.Length >= size)
                    _streamOut.Read(_playerBuffer, 0, size);
                else
                    for (int i = 0; i < _playerBuffer.Length; i++)
                        _playerBuffer[i] = 0;
                System.Runtime.InteropServices.Marshal.Copy(_playerBuffer, 0, data, size);
            }
        }

        private object lock_buffers = new object();
        private void DrawData(object state)
        {
            lock (lock_buffers)
            {
                int sampleSize = _bufferSize * 32;
                if (_framesStreamOut.Length > sampleSize)
                {
                    byte[] _frameData = new byte[sampleSize];
                    _framesStreamOut.Read(_frameData, 0, _frameData.Length);
                    if (_frameData != null)
                    {
                        _audioFrame.Process(ref _frameData);

                        _audioFrame.RenderTimeDomainLeft(ref pictureBoxTimeDomainLeft);
                        _audioFrame.RenderTimeDomainRight(ref pictureBoxTimeDomainRight);
                        _audioFrame.RenderFrequencyDomainLeft(ref pictureBoxFrequencyDomainLeft, Properties.Settings.Default.SettingSamplesPerSecond);
                        _audioFrame.RenderFrequencyDomainRight(ref pictureBoxFrequencyDomainRight, Properties.Settings.Default.SettingSamplesPerSecond);
                        _audioFrame.RenderSpectrogramLeft(ref pictureBoxSpectrogramLeft);
                        _audioFrame.RenderSpectrogramRight(ref pictureBoxSpectrogramRight);
                    }
                }
            }
        }
        private int _bufferSize = 64;
        
        private void DataArrived(IntPtr data, int size)
        {
            byte[] recBuffer = new byte[size];
            System.Runtime.InteropServices.Marshal.Copy(data, recBuffer, 0, size);
            lock (lock_buffers)
            {
                _framesStreamOut.Write(recBuffer, 0, recBuffer.Length);
            }
        }
        private void DataArrivedOriginal(IntPtr data, int size)
        {
            if (_isSaving == true)
            {
                byte[] recBuffer = new byte[size];
                System.Runtime.InteropServices.Marshal.Copy(data, recBuffer, 0, size);
                _streamMemory.Write(recBuffer, 0, recBuffer.Length);
            }
            if (_recorderBuffer == null || _recorderBuffer.Length != size)
                _recorderBuffer = new byte[size];
            if (_recorderBuffer != null)
            {
                System.Runtime.InteropServices.Marshal.Copy(data, _recorderBuffer, 0, size);
                if (_isPlayer == true)
                    _streamOut.Write(_recorderBuffer, 0, _recorderBuffer.Length);
                _audioFrame.Process(ref _recorderBuffer);
                if (_audioFrame.IsEventActive == true)
                {
                    if (_isSaving == false && Properties.Settings.Default.SettingIsSaving == true)
                    {
                        _sampleFilename = DateTime.Now.ToString("yyyyMMddHHmmss") + ".wav";
                        _timeLastDetection = DateTime.Now;
                        _isSaving = true;
                    }
                    else
                    {
                        _timeLastDetection = DateTime.Now;
                    }
                    Invoke(new MethodInvoker(AmplitudeEvent));
                }
                if (_isSaving == true && DateTime.Now.Subtract(_timeLastDetection).Seconds > Properties.Settings.Default.SettingSecondsToSave)
                {
                    byte[] waveBuffer = new byte[Properties.Settings.Default.SettingBitsPerSample];
                    _streamWave = WaveStream.CreateStream(_streamMemory, _waveFormat);
                    waveBuffer = new byte[_streamWave.Length - _streamWave.Position];
                    _streamWave.Read(waveBuffer, 0, waveBuffer.Length);
                    if (Properties.Settings.Default.SettingOutputPath != "")
                        _streamFile = new FileStream(Properties.Settings.Default.SettingOutputPath + "\\" + _sampleFilename, FileMode.Create);
                    else
                        _streamFile = new FileStream(_sampleFilename, FileMode.Create);
                    _streamFile.Write(waveBuffer, 0, waveBuffer.Length);
                    if (_streamWave != null) { _streamWave.Close(); }
                    if (_streamFile != null) { _streamFile.Close(); }
                    _streamMemory = new MemoryStream();
                    _isSaving = false;
                    Invoke(new MethodInvoker(FileSavedEvent));
                }
                _audioFrame.RenderTimeDomainLeft(ref pictureBoxTimeDomainLeft);
                _audioFrame.RenderTimeDomainRight(ref pictureBoxTimeDomainRight);
                _audioFrame.RenderFrequencyDomainLeft(ref pictureBoxFrequencyDomainLeft, Properties.Settings.Default.SettingSamplesPerSecond);
                _audioFrame.RenderFrequencyDomainRight(ref pictureBoxFrequencyDomainRight, Properties.Settings.Default.SettingSamplesPerSecond);
                _audioFrame.RenderSpectrogramLeft(ref pictureBoxSpectrogramLeft);
                _audioFrame.RenderSpectrogramRight(ref pictureBoxSpectrogramRight);
            }
        }
        private void AmplitudeEvent()
        {
            toolStripStatusLabel1.Text = "Last event: " + _timeLastDetection.ToString();
        }
        private void FileSavedEvent()
        {
            textBoxConsole.AppendText(_timeLastDetection.ToString() + " : File " + _sampleFilename + " saved\r\n");
        }
    }
}