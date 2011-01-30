namespace Fingerprinting.AudioSimilarityMeasure
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using Fingerprinting.Audio.FooIdFingerprinting;
    using Fingerprinting.Audio.Wav;
    using Mp3Sharp;
    using Yeti.WMFSdk;

    public partial class MainForm : Form
    {

        public MainForm()
        {
            this.InitializeComponent();
        }

        private void AddLogLine(string line)
        {
            this.textBoxLog.Text = this.textBoxLog.Text + line + Environment.NewLine;
            Application.DoEvents();
        }

        private void btnBrowseInputFile1_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                this.textBoxInputFile1.Text = this.openFileDialog1.FileName;
            }
        }

        private void btnBrowseInputFile2_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                this.textBoxInputFile2.Text = this.openFileDialog2.FileName;
            }
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                this.ValidateInputParameters();
                this.Calculate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void Calculate()
        {
            this.ClearLog();
            this.labelSimilarity.Text = "?";
            try
            {
                FooIdFingerprint fingerprint1 = this.CreateFingerprint(this.InputWavFile1);
                //foreach (FileInfo file in new FileInfo(InputWavFile2).Directory.GetFiles())
                {
                    //if (file.FullName != this.InputWavFile1)
                    {
                        //if (file.Extension == ".wma")
                        {
                            FooIdFingerprint fingerprint2 = this.CreateFingerprint(this.InputWavFile2);
                            float similarity = fingerprint1.Match(fingerprint2);
                            float similarityInPerc = 100f * similarity;
                            this.labelSimilarity.Text = string.Format("{0:f}%", similarityInPerc);
                            this.AddLogLine("");
                            this.AddLogLine(string.Format("Calculated Similarity: {0:f}%", similarityInPerc));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                this.AddLogLine("Error: " + e.Message);
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void ClearLog()
        {
            this.textBoxLog.Text = "";
        }

        private FooIdFingerprint CreateFingerprint(string filename)
        {
            //using (FileStream stream = new FileStream(filename, FileMode.Open))
            {
                this.AddLogLine(string.Format("Reading the input WAVE file: '{0}'...", filename));
                DateTime timestamp1 = DateTime.Now;

                using (WmaStream mp3Stream = new WmaStream(filename))
                //using (Mp3Stream mp3Stream = new Mp3Stream(stream))
                {
                    using (Wave wave = new Wave(mp3Stream))
                    {
                        DateTime timestamp2 = DateTime.Now;
                        TimeSpan timespan21 = new TimeSpan(timestamp2.Ticks - timestamp1.Ticks);

                        this.AddLogLine(string.Format("Done in {0}.", timespan21.ToString()));
                        this.AddLogLine("Calculating audio fingerprint for the input WAVE file...");

                        DateTime timestamp3 = DateTime.Now;
                        FooIdFingerprint fingerprint = new FooIdFingerprint(wave);
                        DateTime timestamp4 = DateTime.Now;
                        TimeSpan timespan43 = new TimeSpan(timestamp4.Ticks - timestamp3.Ticks);
                        this.AddLogLine(string.Format("Done in {0}.", timespan43.ToString()));
                        return fingerprint;
                    }
                }
            }
        }

        private void textBoxInputFile1_TextChanged(object sender, EventArgs e)
        {
            this.UpdateCalculateButtonState();
        }

        private void textBoxInputFile2_TextChanged(object sender, EventArgs e)
        {
            this.UpdateCalculateButtonState();
        }

        private void UpdateCalculateButtonState()
        {
            bool file1Exists = File.Exists(this.InputWavFile1);
            bool file2Exists = File.Exists(this.InputWavFile2);
            FontStyle fontStyle = (file1Exists && file2Exists) ? FontStyle.Bold : FontStyle.Regular;
            this.btnCalculate.Font = new Font(this.btnCalculate.Font, fontStyle);
        }

        private void ValidateInputParameters()
        {
            if (this.InputWavFile1 == "")
            {
                throw new Exception("The first input WAV file is not specified. Pleasy specify the input file and try again.");
            }
            if (!File.Exists(this.InputWavFile1))
            {
                throw new FileNotFoundException(string.Format("The specified input WAV file '{0}' could not be found.", this.InputWavFile1), this.InputWavFile1);
            }
            if (this.InputWavFile2 == "")
            {
                throw new Exception("The second input WAV file is not specified. Pleasy specify the input file and try again.");
            }
            if (!File.Exists(this.InputWavFile2))
            {
                throw new FileNotFoundException(string.Format("The specified input WAV file '{0}' could not be found.", this.InputWavFile2), this.InputWavFile2);
            }
        }

        private string InputWavFile1
        {
            get
            {
                return this.textBoxInputFile1.Text;
            }
            set
            {
                this.textBoxInputFile1.Text = value;
            }
        }

        private string InputWavFile2
        {
            get
            {
                return this.textBoxInputFile2.Text;
            }
            set
            {
                this.textBoxInputFile2.Text = value;
            }
        }
    }
}

