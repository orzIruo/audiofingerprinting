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
        private Stream m_AudioStream; 
        private MemoryStream _streamMemory;
        private Stream _streamWave;
        private FileStream _streamFile;
        private bool _isPlayer = true;  // audio output for testing
        private bool _isTest = false;  // signal generation for testing
        private bool _isAudioFile = false;  // music signal generation for testing
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
                if (_isAudioFile)
                {
                    OpenFile();

                    _bufferSize = (int)Math.Round(buffer_duration * _waveFormat.nSamplesPerSec *
                        _waveFormat.wBitsPerSample / 8) * _waveFormat.nChannels;
                }
                else
                {
                    _bufferSize = (int)Math.Round(buffer_duration * Properties.Settings.Default.SettingSamplesPerSecond *
    Properties.Settings.Default.SettingBitsPerSample / 8) * Properties.Settings.Default.SettingChannels;
                }

                //_drawTimer = new System.Threading.Timer(DrawData, null, 1000, 50);
                textBoxConsole.AppendText(DateTime.Now.ToString() + " : Audio input device detected\r\n");
                if (_isPlayer == true)
                    _streamOut = new FifoStream();
                
                _audioFrame = new AudioFrame(_isTest);
                _audioFrame.IsDetectingEvents = Properties.Settings.Default.SettingIsDetectingEvents;
                _audioFrame.AmplitudeThreshold = Properties.Settings.Default.SettingAmplitudeThreshold;
                _streamMemory = new MemoryStream();
                Start();
            }
        }
        private void OpenFile()
        {
            OpenFileDialog OpenDlg = new OpenFileDialog();
            if (OpenDlg.ShowDialog() == DialogResult.OK)
            {
                CloseFile();
                try
                {
                    WaveStream S = new WaveStream(OpenDlg.FileName);
                    if (S.Length <= 0)
                        throw new Exception("Invalid WAV file");
                    _waveFormat = S.Format;
                    if (_waveFormat.wFormatTag != (short)WaveFormats.Pcm && _waveFormat.wFormatTag != (short)WaveFormats.Float)
                        throw new Exception("Olny PCM files are supported");

                    m_AudioStream = S;
                }
                catch (Exception e)
                {
                    CloseFile();
                    MessageBox.Show(e.Message);
                }
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
            CloseFile();
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
                //CloseFile();
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
                if (_waveFormat == null)
                {
                    _waveFormat = new WaveFormat(44100, 16, 2);
                    _waveFormat = new WaveFormat(Properties.Settings.Default.SettingSamplesPerSecond, Properties.Settings.Default.SettingBitsPerSample, Properties.Settings.Default.SettingChannels);
                }
                _recorder = new WaveInRecorder(Properties.Settings.Default.SettingAudioInputDevice, _waveFormat, Properties.Settings.Default.SettingBytesPerFrame * Properties.Settings.Default.SettingChannels, 3, new BufferDoneEventHandler(DataArrived));
                if (_isAudioFile)
                {
                    if (m_AudioStream != null)
                    {
                        m_AudioStream.Position = 0;
                        if (_isPlayer)
                            _player = new WaveOutPlayer(-1, _waveFormat, 16384, 3, new BufferFillEventHandler(Filler));
                    }
                }
                else
                {
                    if (_isPlayer)
                        _player = new WaveOutPlayer(Properties.Settings.Default.SettingAudioOutputDevice, _waveFormat, Properties.Settings.Default.SettingBytesPerFrame * Properties.Settings.Default.SettingChannels, 3, new BufferFillEventHandler(Filler));
                }

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
                if (_streamOut != null)
                    _streamOut.Flush(); // clear all pending data
            }
            textBoxConsole.AppendText(DateTime.Now.ToString() + " : Audio input device polling stopped\r\n");
        }
        private void CloseFile()
        {
            Stop();
            if (m_AudioStream != null)
                try
                {
                    m_AudioStream.Close();
                }
                finally
                {
                    m_AudioStream = null;
                }
        }

        private void Filler(IntPtr data, int size)
        {
            if (_isPlayer == true)
            {
                if (_playerBuffer == null || _playerBuffer.Length < size)
                    _playerBuffer = new byte[size];

                if (_isAudioFile)
                {
                    if (m_AudioStream != null)
                    {
                        int pos = 0;
                        while (pos < size)
                        {
                            int toget = size - pos;
                            int got = m_AudioStream.Read(_playerBuffer, pos, toget);
                            if (got < toget)
                                m_AudioStream.Position = 0; // loop if the file ends
                            pos += got;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _playerBuffer.Length; i++)
                            _playerBuffer[i] = 0;
                    }
                }
                else
                {
                    if (_streamOut.Length >= size)
                        _streamOut.Read(_playerBuffer, 0, size);
                    else
                        for (int i = 0; i < _playerBuffer.Length; i++)
                            _playerBuffer[i] = 0;                    
                }
                System.Runtime.InteropServices.Marshal.Copy(_playerBuffer, 0, data, size);
            }
        }

        private byte[] _samplesBuffer = new byte[0];
        private object lock_buffers = new object();
        private int number_buffers = 32;
        private double buffer_duration = 0.0116;

        private void DrawData(object state)
        {
            lock (lock_buffers)
            {
                int sampleSize = _bufferSize * number_buffers;

                while (_samplesBuffer.Length >= sampleSize)
                //if (_framesStreamOut.Length > sampleSize)
                {
                    byte[] _frameData = new byte[sampleSize];
                    //_framesStreamOut.Read(_frameData, 0, _frameData.Length);
                    Array.Copy(_samplesBuffer, _frameData, sampleSize);
                    byte[] newSamplesBuffer = new byte[sampleSize - _bufferSize];
                    Array.Copy(newSamplesBuffer, 0, _samplesBuffer, _bufferSize, newSamplesBuffer.Length);
                    _samplesBuffer = newSamplesBuffer;

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
        private int _bufferSize = 0;
        
        private void DataArrived(IntPtr data, int size)
        {
            if (_recorderBuffer == null || _recorderBuffer.Length != size)
                _recorderBuffer = new byte[size];

            System.Runtime.InteropServices.Marshal.Copy(data, _recorderBuffer, 0, size);
            if (_isPlayer == true)
                _streamOut.Write(_recorderBuffer, 0, _recorderBuffer.Length);

            lock (lock_buffers)
            {
                //_framesStreamOut.Write(recBuffer, 0, recBuffer.Length);
                int prevLength = _samplesBuffer.Length;
                Array.Resize(ref _samplesBuffer, _samplesBuffer.Length + size);
                Array.Copy(_recorderBuffer, 0, _samplesBuffer, prevLength, size);

                DrawData(null);
            }            
        }
        private void DataArrived_2(IntPtr data, int size)
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