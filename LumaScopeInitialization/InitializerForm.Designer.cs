namespace LumaScopeInitialization
{
    partial class InitializerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label4 = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.pollingProgressBar = new System.Windows.Forms.ProgressBar();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(25, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Status:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.Location = new System.Drawing.Point(97, 32);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(76, 20);
            this.statusLabel.TabIndex = 8;
            this.statusLabel.Text = "Unknown";
            // 
            // pollingProgressBar
            // 
            this.pollingProgressBar.Location = new System.Drawing.Point(29, 74);
            this.pollingProgressBar.Maximum = 20;
            this.pollingProgressBar.Name = "pollingProgressBar";
            this.pollingProgressBar.Size = new System.Drawing.Size(551, 18);
            this.pollingProgressBar.Step = 1;
            this.pollingProgressBar.TabIndex = 291;
            this.pollingProgressBar.Tag = "ManualStageControl.htm#";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // InitializerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 125);
            this.Controls.Add(this.pollingProgressBar);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.label4);
            this.Name = "InitializerForm";
            this.Text = "Lumascope Initializer";
            this.Load += new System.EventHandler(this.UsbMsgForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ProgressBar pollingProgressBar;
        private System.Windows.Forms.Timer timer1;
    }
}