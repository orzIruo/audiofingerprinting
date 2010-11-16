namespace Fingerprinting.AudioSimilarityMeasure
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.IO;
    using System.Windows.Forms;
    using Fingerprinting.Audio.FooIdFingerprinting;
    using Fingerprinting.Audio.Wav;

    public partial class MainForm : Form
    {
        private ToolStripMenuItem aboutToolStripMenuItem;
        private Button btnBrowseInputFile1;
        private Button btnBrowseInputFile2;
        private Button btnCalculate;
        private IContainer components;
        private GroupBox groupBoxInputFiles;
        private GroupBox groupBoxLog;
        private GroupBox groupBoxResult;
        private ToolStripMenuItem helpToolStripMenuItem;
        private Label labelInputFile1;
        private Label labelInputFile2;
        private Label labelSimilarity;
        private MenuStrip menuStrip;
        private OpenFileDialog openFileDialog1;
        private OpenFileDialog openFileDialog2;
        private StatusStrip statusStrip;
        private TextBox textBoxInputFile1;
        private TextBox textBoxInputFile2;
        private TextBox textBoxLog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.groupBoxInputFiles = new GroupBox();
            this.labelInputFile2 = new Label();
            this.textBoxInputFile2 = new TextBox();
            this.btnBrowseInputFile2 = new Button();
            this.btnBrowseInputFile1 = new Button();
            this.textBoxInputFile1 = new TextBox();
            this.labelInputFile1 = new Label();
            this.groupBoxResult = new GroupBox();
            this.labelSimilarity = new Label();
            this.statusStrip = new StatusStrip();
            this.btnCalculate = new Button();
            this.groupBoxLog = new GroupBox();
            this.textBoxLog = new TextBox();
            this.openFileDialog1 = new OpenFileDialog();
            this.openFileDialog2 = new OpenFileDialog();
            this.menuStrip = new MenuStrip();
            this.helpToolStripMenuItem = new ToolStripMenuItem();
            this.aboutToolStripMenuItem = new ToolStripMenuItem();
            this.groupBoxInputFiles.SuspendLayout();
            this.groupBoxResult.SuspendLayout();
            this.groupBoxLog.SuspendLayout();
            this.menuStrip.SuspendLayout();
            base.SuspendLayout();
            this.groupBoxInputFiles.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.groupBoxInputFiles.Controls.Add(this.labelInputFile2);
            this.groupBoxInputFiles.Controls.Add(this.textBoxInputFile2);
            this.groupBoxInputFiles.Controls.Add(this.btnBrowseInputFile2);
            this.groupBoxInputFiles.Controls.Add(this.btnBrowseInputFile1);
            this.groupBoxInputFiles.Controls.Add(this.textBoxInputFile1);
            this.groupBoxInputFiles.Controls.Add(this.labelInputFile1);
            this.groupBoxInputFiles.Location = new Point(12, 0x1b);
            this.groupBoxInputFiles.Name = "groupBoxInputFiles";
            this.groupBoxInputFiles.Size = new Size(480, 0x52);
            this.groupBoxInputFiles.TabIndex = 0;
            this.groupBoxInputFiles.TabStop = false;
            this.groupBoxInputFiles.Text = "Input Files";
            this.labelInputFile2.AutoSize = true;
            this.labelInputFile2.Location = new Point(6, 0x35);
            this.labelInputFile2.Name = "labelInputFile2";
            this.labelInputFile2.Size = new Size(0x57, 13);
            this.labelInputFile2.TabIndex = 3;
            this.labelInputFile2.Text = "Input WAV File 2";
            this.textBoxInputFile2.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxInputFile2.Location = new Point(0x63, 50);
            this.textBoxInputFile2.Name = "textBoxInputFile2";
            this.textBoxInputFile2.Size = new Size(0x158, 20);
            this.textBoxInputFile2.TabIndex = 4;
            this.textBoxInputFile2.TextChanged += new EventHandler(this.textBoxInputFile2_TextChanged);
            this.btnBrowseInputFile2.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.btnBrowseInputFile2.Location = new Point(0x1c1, 0x30);
            this.btnBrowseInputFile2.Name = "btnBrowseInputFile2";
            this.btnBrowseInputFile2.Size = new Size(0x19, 0x17);
            this.btnBrowseInputFile2.TabIndex = 5;
            this.btnBrowseInputFile2.Text = "...";
            this.btnBrowseInputFile2.UseVisualStyleBackColor = true;
            this.btnBrowseInputFile2.Click += new EventHandler(this.btnBrowseInputFile2_Click);
            this.btnBrowseInputFile1.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            this.btnBrowseInputFile1.Location = new Point(0x1c1, 0x13);
            this.btnBrowseInputFile1.Name = "btnBrowseInputFile1";
            this.btnBrowseInputFile1.Size = new Size(0x19, 0x17);
            this.btnBrowseInputFile1.TabIndex = 2;
            this.btnBrowseInputFile1.Text = "...";
            this.btnBrowseInputFile1.UseVisualStyleBackColor = true;
            this.btnBrowseInputFile1.Click += new EventHandler(this.btnBrowseInputFile1_Click);
            this.textBoxInputFile1.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.textBoxInputFile1.Location = new Point(0x63, 0x15);
            this.textBoxInputFile1.Name = "textBoxInputFile1";
            this.textBoxInputFile1.Size = new Size(0x158, 20);
            this.textBoxInputFile1.TabIndex = 1;
            this.textBoxInputFile1.TextChanged += new EventHandler(this.textBoxInputFile1_TextChanged);
            this.labelInputFile1.AutoSize = true;
            this.labelInputFile1.Location = new Point(6, 0x18);
            this.labelInputFile1.Name = "labelInputFile1";
            this.labelInputFile1.Size = new Size(0x57, 13);
            this.labelInputFile1.TabIndex = 0;
            this.labelInputFile1.Text = "Input WAV File 1";
            this.groupBoxResult.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.groupBoxResult.Controls.Add(this.labelSimilarity);
            this.groupBoxResult.Location = new Point(12, 0x73);
            this.groupBoxResult.Name = "groupBoxResult";
            this.groupBoxResult.Size = new Size(480, 100);
            this.groupBoxResult.TabIndex = 1;
            this.groupBoxResult.TabStop = false;
            this.groupBoxResult.Text = "Similarity";
            this.labelSimilarity.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.labelSimilarity.Font = new Font("Microsoft Sans Serif", 24f, FontStyle.Regular, GraphicsUnit.Point, 0xee);
            this.labelSimilarity.ForeColor = SystemColors.ControlText;
            this.labelSimilarity.Location = new Point(6, 0x10);
            this.labelSimilarity.Name = "labelSimilarity";
            this.labelSimilarity.Size = new Size(0x1d4, 0x51);
            this.labelSimilarity.TabIndex = 0;
            this.labelSimilarity.Text = "?";
            this.labelSimilarity.TextAlign = ContentAlignment.MiddleCenter;
            this.statusStrip.Location = new Point(0, 0x1a3);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new Size(0x1f8, 0x16);
            this.statusStrip.TabIndex = 4;
            this.btnCalculate.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            this.btnCalculate.Location = new Point(410, 0x183);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new Size(0x52, 0x17);
            this.btnCalculate.TabIndex = 3;
            this.btnCalculate.Text = "Calculate";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new EventHandler(this.btnCalculate_Click);
            this.groupBoxLog.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.groupBoxLog.Controls.Add(this.textBoxLog);
            this.groupBoxLog.Location = new Point(12, 0xdd);
            this.groupBoxLog.Name = "groupBoxLog";
            this.groupBoxLog.Size = new Size(480, 0x9c);
            this.groupBoxLog.TabIndex = 2;
            this.groupBoxLog.TabStop = false;
            this.groupBoxLog.Text = "Log";
            this.textBoxLog.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
            this.textBoxLog.BackColor = SystemColors.ControlLightLight;
            this.textBoxLog.Font = new Font("Courier New", 8.25f, FontStyle.Regular, GraphicsUnit.Point, 0xee);
            this.textBoxLog.Location = new Point(9, 0x13);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = ScrollBars.Vertical;
            this.textBoxLog.Size = new Size(0x1d1, 0x83);
            this.textBoxLog.TabIndex = 0;
            this.openFileDialog1.Filter = "Wav PCM files (*.wav)|*.wav|All files|*.*";
            this.openFileDialog2.FileName = "openFileDialog2";
            this.openFileDialog2.Filter = "Wav PCM files (*.wav)|*.wav|All files|*.*";
            this.menuStrip.Items.AddRange(new ToolStripItem[] { this.helpToolStripMenuItem });
            this.menuStrip.Location = new Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.RenderMode = ToolStripRenderMode.System;
            this.menuStrip.Size = new Size(0x1f8, 0x18);
            this.menuStrip.TabIndex = 5;
            /*
            this.helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { this.aboutToolStripMenuItem });
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new Size(40, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new Size(0x98, 0x16);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new EventHandler(this.aboutToolStripMenuItem_Click);
             */
            base.AutoScaleDimensions = new SizeF(6f, 13f);
            base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x1f8, 0x1b9);
            base.Controls.Add(this.groupBoxLog);
            base.Controls.Add(this.btnCalculate);
            base.Controls.Add(this.statusStrip);
            base.Controls.Add(this.menuStrip);
            base.Controls.Add(this.groupBoxResult);
            base.Controls.Add(this.groupBoxInputFiles);
            base.MainMenuStrip = this.menuStrip;
            base.Name = "MainForm";
            this.Text = "Audio Fingerprinting";
            this.groupBoxInputFiles.ResumeLayout(false);
            this.groupBoxInputFiles.PerformLayout();
            this.groupBoxResult.ResumeLayout(false);
            this.groupBoxLog.ResumeLayout(false);
            this.groupBoxLog.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            base.ResumeLayout(false);
            base.PerformLayout();
        }
    }
}