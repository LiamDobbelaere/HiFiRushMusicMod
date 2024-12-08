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
            lblFps = new Label();
            bwGameCapture = new System.ComponentModel.BackgroundWorker();
            rtbLog = new RichTextBox();
            btnStopLogging = new Button();
            lblBpm = new Label();
            lblRank = new Label();
            SuspendLayout();
            // 
            // lblFps
            // 
            lblFps.AutoSize = true;
            lblFps.Location = new Point(332, 184);
            lblFps.Name = "lblFps";
            lblFps.Size = new Size(38, 15);
            lblFps.TabIndex = 0;
            lblFps.Text = "lblFps";
            // 
            // bwGameCapture
            // 
            bwGameCapture.DoWork += bwGameCapture_DoWork;
            // 
            // rtbLog
            // 
            rtbLog.Location = new Point(12, 342);
            rtbLog.Name = "rtbLog";
            rtbLog.Size = new Size(336, 96);
            rtbLog.TabIndex = 1;
            rtbLog.Text = "";
            // 
            // btnStopLogging
            // 
            btnStopLogging.Location = new Point(354, 341);
            btnStopLogging.Name = "btnStopLogging";
            btnStopLogging.Size = new Size(124, 23);
            btnStopLogging.TabIndex = 2;
            btnStopLogging.Text = "Stop logging";
            btnStopLogging.UseVisualStyleBackColor = true;
            btnStopLogging.Click += btnStopLogging_Click;
            // 
            // lblBpm
            // 
            lblBpm.AutoSize = true;
            lblBpm.Font = new Font("Segoe UI", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblBpm.Location = new Point(299, 224);
            lblBpm.Name = "lblBpm";
            lblBpm.Size = new Size(112, 47);
            lblBpm.TabIndex = 3;
            lblBpm.Text = "label1";
            // 
            // lblRank
            // 
            lblRank.AutoSize = true;
            lblRank.Font = new Font("Segoe UI", 26.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblRank.Location = new Point(299, 271);
            lblRank.Name = "lblRank";
            lblRank.Size = new Size(112, 47);
            lblRank.TabIndex = 4;
            lblRank.Text = "label1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(800, 450);
            Controls.Add(lblRank);
            Controls.Add(lblBpm);
            Controls.Add(btnStopLogging);
            Controls.Add(rtbLog);
            Controls.Add(lblFps);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblFps;
        private System.ComponentModel.BackgroundWorker bwGameCapture;
        private RichTextBox rtbLog;
        private Button btnStopLogging;
        private Label lblBpm;
        private Label lblRank;
    }
}
