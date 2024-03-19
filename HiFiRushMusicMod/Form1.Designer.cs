namespace HiFiRushMusicMod
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pbxScreenGrab = new PictureBox();
            tmrScreenGrabber = new System.Windows.Forms.Timer(components);
            tmrResetter = new System.Windows.Forms.Timer(components);
            tmrMemoryBPM = new System.Windows.Forms.Timer(components);
            bpmLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)pbxScreenGrab).BeginInit();
            SuspendLayout();
            // 
            // pbxScreenGrab
            // 
            pbxScreenGrab.Location = new Point(12, 12);
            pbxScreenGrab.Name = "pbxScreenGrab";
            pbxScreenGrab.Size = new Size(283, 210);
            pbxScreenGrab.SizeMode = PictureBoxSizeMode.StretchImage;
            pbxScreenGrab.TabIndex = 0;
            pbxScreenGrab.TabStop = false;
            // 
            // tmrScreenGrabber
            // 
            tmrScreenGrabber.Enabled = true;
            tmrScreenGrabber.Interval = 16;
            tmrScreenGrabber.Tick += tmrScreenGrabber_Tick;
            // 
            // tmrResetter
            // 
            tmrResetter.Enabled = true;
            tmrResetter.Interval = 10000;
            tmrResetter.Tick += tmrResetter_Tick;
            // 
            // tmrMemoryBPM
            // 
            tmrMemoryBPM.Enabled = true;
            tmrMemoryBPM.Interval = 1000;
            tmrMemoryBPM.Tick += tmrMemoryBPM_Tick;
            // 
            // bpmLabel
            // 
            bpmLabel.AutoSize = true;
            bpmLabel.Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bpmLabel.Location = new Point(12, 235);
            bpmLabel.Name = "bpmLabel";
            bpmLabel.Size = new Size(105, 45);
            bpmLabel.TabIndex = 1;
            bpmLabel.Text = "label1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(bpmLabel);
            Controls.Add(pbxScreenGrab);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pbxScreenGrab).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pbxScreenGrab;
        private System.Windows.Forms.Timer tmrScreenGrabber;
        private System.Windows.Forms.Timer tmrResetter;
        private System.Windows.Forms.Timer tmrMemoryBPM;
        private Label bpmLabel;
    }
}
