/* Copyright (C) 2007 Jeff Morton (jeffrey.raymond.morton@gmail.com)

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
using System.Drawing;
using System.Windows.Forms;

namespace SoundViewer
{
    class AudioFrame
    {
        private Bitmap _canvasTimeDomain;
        private Bitmap _canvasFrequencyDomain;
        private double[] _waveLeft;
        private double[] _waveRight;
        private double[] _fftLeft;
        private double[] _fftRight;
        private SignalGenerator _signalGenerator;
        private bool _isTest = false;

        public AudioFrame(bool isTest)
        {
            _isTest = isTest;
        }

        /// <summary>
        /// Process 16 bit sample
        /// </summary>
        /// <param name="wave"></param>
        public void Process(ref byte[] wave)
        {
            _waveLeft = new double[wave.Length / 4];
            _waveRight = new double[wave.Length / 4];

            if (_isTest == false)
            {
                // Split out channels from sample
                int h = 0;
                for (int i = 0; i < wave.Length; i += 4)
                {
                    _waveLeft[h] = (double)BitConverter.ToInt16(wave, i);
                    _waveRight[h] = (double)BitConverter.ToInt16(wave, i + 2);
                    h++;
                }
            }
            else
            {
                // Generate artificial sample for testing
                _signalGenerator = new SignalGenerator();
                _signalGenerator.SetWaveform("Sine");
                _signalGenerator.SetSamplingRate(44100);
                _signalGenerator.SetSamples(16384);
                _signalGenerator.SetFrequency(5000);
                _signalGenerator.SetAmplitude(32768);
                _waveLeft = _signalGenerator.GenerateSignal();
                _waveRight = _signalGenerator.GenerateSignal();
            }

            // Generate frequency domain data in decibels
            _fftLeft = FourierTransform.FFTDb(ref _waveLeft);
            _fftRight = FourierTransform.FFTDb(ref _waveRight);
        }

        /// <summary>
        /// Render time domain to PictureBox
        /// </summary>
        /// <param name="pictureBox"></param>
        public void RenderTimeDomain(ref PictureBox pictureBox)
        {
            // Set up for drawing
            _canvasTimeDomain = new Bitmap(pictureBox.Width, pictureBox.Height);
            Graphics offScreenDC = Graphics.FromImage(_canvasTimeDomain);
            SolidBrush brush = new System.Drawing.SolidBrush(Color.FromArgb(0, 0, 0));
            Pen pen = new System.Drawing.Pen(Color.WhiteSmoke);

            // Determine channnel boundries
            int width = _canvasTimeDomain.Width;
            int center = _canvasTimeDomain.Height / 2;
            int height = _canvasTimeDomain.Height;

            offScreenDC.DrawLine(pen, 0, center, width, center);

            int leftLeft = 0;
            int leftTop = 0;
            int leftRight = width;
            int leftBottom = center - 1;

            int rightLeft = 0;
            int rightTop = center + 1;
            int rightRight = width;
            int rightBottom = height;

            // Draw left channel
            double yCenterLeft = (leftBottom - leftTop) / 2;
            double yScaleLeft = 0.5 * (leftBottom - leftTop) / 32768;  // a 16 bit sample has values from -32768 to 32767
            int xPrevLeft = 0, yPrevLeft = 0;
            for (int xAxis = leftLeft; xAxis < leftRight; xAxis++)
            {
                int yAxis = (int)(yCenterLeft + (_waveLeft[_waveLeft.Length / (leftRight - leftLeft) * xAxis] * yScaleLeft));
                if (xAxis == 0)
                {
                    xPrevLeft = 0;
                    yPrevLeft = yAxis;
                }
                else
                {
                    pen.Color = Color.LimeGreen;
                    offScreenDC.DrawLine(pen, xPrevLeft, yPrevLeft, xAxis, yAxis);
                    xPrevLeft = xAxis;
                    yPrevLeft = yAxis;
                }
            }

            // Draw right channel
            int xCenterRight = rightTop + ((rightBottom - rightTop) / 2);
            double yScaleRight = 0.5 * (rightBottom - rightTop) / 32768;  // a 16 bit sample has values from -32768 to 32767
            int xPrevRight = 0, yPrevRight = 0;
            for (int xAxis = rightLeft; xAxis < rightRight; xAxis++)
            {
                int yAxis = (int)(xCenterRight + (_waveRight[_waveRight.Length / (rightRight - rightLeft) * xAxis] * yScaleRight));
                if (xAxis == 0)
                {
                    xPrevRight = 0;
                    yPrevRight = yAxis;
                }
                else
                {
                    pen.Color = Color.LimeGreen;
                    offScreenDC.DrawLine(pen, xPrevRight, yPrevRight, xAxis, yAxis);
                    xPrevRight = xAxis;
                    yPrevRight = yAxis;
                }
            }

            // Clean up
            pictureBox.Image = _canvasTimeDomain;
            offScreenDC.Dispose();
        }

        /// <summary>
        /// Render frequency domain to PictureBox
        /// </summary>
        /// <param name="pictureBox"></param>
        public void RenderFrequencyDomain(ref PictureBox pictureBox)
        {
            // Set up for drawing
            _canvasFrequencyDomain = new Bitmap(pictureBox.Width, pictureBox.Height);
            Graphics offScreenDC = Graphics.FromImage(_canvasFrequencyDomain);
            SolidBrush brush = new System.Drawing.SolidBrush(Color.FromArgb(0, 0, 0));
            Pen pen = new System.Drawing.Pen(Color.WhiteSmoke);

            // Determine channnel boundries
            int width = _canvasFrequencyDomain.Width;
            int center = _canvasFrequencyDomain.Height / 2;
            int height = _canvasFrequencyDomain.Height;

            offScreenDC.DrawLine(pen, 0, center, width, center);

            int leftLeft = 0;
            int leftTop = 0;
            int leftRight = width;
            int leftBottom = center - 1;

            int rightLeft = 0;
            int rightTop = center + 1;
            int rightRight = width;
            int rightBottom = height;

            // Draw left channel
            for (int xAxis = leftLeft; xAxis < leftRight; xAxis++)
            {
                double amplitude = (int)_fftLeft[(int)(((double)(_fftLeft.Length) / (double)(width)) * xAxis)];
                if (amplitude < 0) // Drop negative values
                    amplitude = 0;
                int yAxis = (int)(leftTop + ((leftBottom - leftTop) * amplitude) / 100);  // Arbitrary factor
                pen.Color = Color.FromArgb(0, 0, (int)amplitude % 255);
                offScreenDC.DrawLine(pen, xAxis, leftTop, xAxis, yAxis);
            }

            // Draw right channel
            for (int xAxis = rightLeft; xAxis < rightRight; xAxis++)
            {
                double amplitude = (int)_fftRight[(int)(((double)(_fftRight.Length) / (double)(width)) * xAxis)];
                if (amplitude < 0)
                    amplitude = 0;
                int yAxis = (int)(rightBottom - ((rightBottom - rightTop) * amplitude) / 100);
                pen.Color = Color.FromArgb(0, 0, (int)amplitude % 255);
                offScreenDC.DrawLine(pen, xAxis, rightBottom, xAxis, yAxis);
            }

            // Clean up
            pictureBox.Image = _canvasFrequencyDomain;
            offScreenDC.Dispose();
        }
    }
}
