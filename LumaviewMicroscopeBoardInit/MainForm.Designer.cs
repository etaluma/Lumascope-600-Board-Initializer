namespace LumaviewMicroscopeBoardInit
{
    partial class MainForm
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
            this.msgBasedInitButton = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pollingProgressBar = new System.Windows.Forms.ProgressBar();
            this.pollingStatusLabel = new System.Windows.Forms.Label();
            this.pollingBasedInitButton = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.autoCloseCheckBox = new System.Windows.Forms.CheckBox();
            this.ledGroupBox = new System.Windows.Forms.GroupBox();
            this.led3NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.led2NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.led1NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.startStreamingButton = new System.Windows.Forms.Button();
            this.stopStreamingButton = new System.Windows.Forms.Button();
            this.tmrUpdateImage = new System.Windows.Forms.Timer(this.components);
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.ledGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.led3NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.led2NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.led1NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.SuspendLayout();
            // 
            // msgBasedInitButton
            // 
            this.msgBasedInitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.msgBasedInitButton.Location = new System.Drawing.Point(23, 28);
            this.msgBasedInitButton.Margin = new System.Windows.Forms.Padding(6);
            this.msgBasedInitButton.Name = "msgBasedInitButton";
            this.msgBasedInitButton.Size = new System.Drawing.Size(266, 31);
            this.msgBasedInitButton.TabIndex = 0;
            this.msgBasedInitButton.Text = "Initialize Lumascope Board (Dialog)";
            this.msgBasedInitButton.UseVisualStyleBackColor = true;
            this.msgBasedInitButton.Click += new System.EventHandler(this.msgBasedInitButton_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pollingProgressBar);
            this.groupBox1.Controls.Add(this.pollingStatusLabel);
            this.groupBox1.Controls.Add(this.pollingBasedInitButton);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(16, 272);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(312, 146);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Initialize with Polling for Status";
            // 
            // pollingProgressBar
            // 
            this.pollingProgressBar.Location = new System.Drawing.Point(23, 84);
            this.pollingProgressBar.Maximum = 20;
            this.pollingProgressBar.Name = "pollingProgressBar";
            this.pollingProgressBar.Size = new System.Drawing.Size(266, 18);
            this.pollingProgressBar.Step = 1;
            this.pollingProgressBar.TabIndex = 290;
            this.pollingProgressBar.Tag = "ManualStageControl.htm#";
            // 
            // pollingStatusLabel
            // 
            this.pollingStatusLabel.AutoSize = true;
            this.pollingStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pollingStatusLabel.Location = new System.Drawing.Point(24, 114);
            this.pollingStatusLabel.Name = "pollingStatusLabel";
            this.pollingStatusLabel.Size = new System.Drawing.Size(86, 16);
            this.pollingStatusLabel.TabIndex = 8;
            this.pollingStatusLabel.Text = "Not yet found";
            // 
            // pollingBasedInitButton
            // 
            this.pollingBasedInitButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pollingBasedInitButton.Location = new System.Drawing.Point(23, 33);
            this.pollingBasedInitButton.Name = "pollingBasedInitButton";
            this.pollingBasedInitButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.pollingBasedInitButton.Size = new System.Drawing.Size(266, 40);
            this.pollingBasedInitButton.TabIndex = 5;
            this.pollingBasedInitButton.Text = "Initialize Lumascope Board (Polling)";
            this.pollingBasedInitButton.UseVisualStyleBackColor = true;
            this.pollingBasedInitButton.Click += new System.EventHandler(this.pollingBasedInitButton_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.autoCloseCheckBox);
            this.groupBox2.Controls.Add(this.msgBasedInitButton);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(16, 147);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(312, 110);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Initialize - Launch Dialog (Windows Msg-Based)";
            // 
            // autoCloseCheckBox
            // 
            this.autoCloseCheckBox.AutoSize = true;
            this.autoCloseCheckBox.Location = new System.Drawing.Point(27, 71);
            this.autoCloseCheckBox.Name = "autoCloseCheckBox";
            this.autoCloseCheckBox.Size = new System.Drawing.Size(250, 20);
            this.autoCloseCheckBox.TabIndex = 1;
            this.autoCloseCheckBox.Text = "Dialog auto-close upon init. complete.";
            this.autoCloseCheckBox.UseVisualStyleBackColor = true;
            // 
            // ledGroupBox
            // 
            this.ledGroupBox.Controls.Add(this.led3NumericUpDown);
            this.ledGroupBox.Controls.Add(this.label3);
            this.ledGroupBox.Controls.Add(this.led2NumericUpDown);
            this.ledGroupBox.Controls.Add(this.label2);
            this.ledGroupBox.Controls.Add(this.led1NumericUpDown);
            this.ledGroupBox.Controls.Add(this.label1);
            this.ledGroupBox.Enabled = false;
            this.ledGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ledGroupBox.Location = new System.Drawing.Point(16, 433);
            this.ledGroupBox.Name = "ledGroupBox";
            this.ledGroupBox.Size = new System.Drawing.Size(312, 99);
            this.ledGroupBox.TabIndex = 3;
            this.ledGroupBox.TabStop = false;
            this.ledGroupBox.Text = "LEDs";
            // 
            // led3NumericUpDown
            // 
            this.led3NumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.led3NumericUpDown.Location = new System.Drawing.Point(219, 49);
            this.led3NumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.led3NumericUpDown.Name = "led3NumericUpDown";
            this.led3NumericUpDown.Size = new System.Drawing.Size(70, 29);
            this.led3NumericUpDown.TabIndex = 8;
            this.led3NumericUpDown.ValueChanged += new System.EventHandler(this.led3NumericUpDown_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(230, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "LED 3:";
            // 
            // led2NumericUpDown
            // 
            this.led2NumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.led2NumericUpDown.Location = new System.Drawing.Point(129, 49);
            this.led2NumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.led2NumericUpDown.Name = "led2NumericUpDown";
            this.led2NumericUpDown.Size = new System.Drawing.Size(70, 29);
            this.led2NumericUpDown.TabIndex = 6;
            this.led2NumericUpDown.ValueChanged += new System.EventHandler(this.led2NumericUpDown_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(139, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "LED 2:";
            // 
            // led1NumericUpDown
            // 
            this.led1NumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.led1NumericUpDown.Location = new System.Drawing.Point(27, 49);
            this.led1NumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.led1NumericUpDown.Name = "led1NumericUpDown";
            this.led1NumericUpDown.Size = new System.Drawing.Size(70, 29);
            this.led1NumericUpDown.TabIndex = 4;
            this.led1NumericUpDown.ValueChanged += new System.EventHandler(this.led1NumericUpDown_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(41, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "LED 1:";
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pbImage
            // 
            this.pbImage.Location = new System.Drawing.Point(379, 49);
            this.pbImage.Margin = new System.Windows.Forms.Padding(2);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(667, 587);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbImage.TabIndex = 4;
            this.pbImage.TabStop = false;
            // 
            // startStreamingButton
            // 
            this.startStreamingButton.Enabled = false;
            this.startStreamingButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.startStreamingButton.Location = new System.Drawing.Point(16, 546);
            this.startStreamingButton.Name = "startStreamingButton";
            this.startStreamingButton.Size = new System.Drawing.Size(312, 40);
            this.startStreamingButton.TabIndex = 5;
            this.startStreamingButton.Text = "Start Video Streaming";
            this.startStreamingButton.UseVisualStyleBackColor = true;
            this.startStreamingButton.Click += new System.EventHandler(this.startStreamingButton_Click);
            // 
            // stopStreamingButton
            // 
            this.stopStreamingButton.Enabled = false;
            this.stopStreamingButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stopStreamingButton.Location = new System.Drawing.Point(16, 596);
            this.stopStreamingButton.Name = "stopStreamingButton";
            this.stopStreamingButton.Size = new System.Drawing.Size(312, 40);
            this.stopStreamingButton.TabIndex = 6;
            this.stopStreamingButton.Text = "Stop Video Streaming";
            this.stopStreamingButton.UseVisualStyleBackColor = true;
            this.stopStreamingButton.Click += new System.EventHandler(this.stopStreamingButton_Click);
            // 
            // tmrUpdateImage
            // 
            this.tmrUpdateImage.Interval = 10;
            this.tmrUpdateImage.Tick += new System.EventHandler(this.tmrUpdateImage_Tick);
            // 
            // richTextBox
            // 
            this.richTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richTextBox.Location = new System.Drawing.Point(14, 10);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.ReadOnly = true;
            this.richTextBox.Size = new System.Drawing.Size(314, 118);
            this.richTextBox.TabIndex = 7;
            this.richTextBox.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1092, 653);
            this.Controls.Add(this.richTextBox);
            this.Controls.Add(this.stopStreamingButton);
            this.Controls.Add(this.startStreamingButton);
            this.Controls.Add(this.pbImage);
            this.Controls.Add(this.ledGroupBox);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "MainForm";
            this.Text = "Lumascope Initializer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ledGroupBox.ResumeLayout(false);
            this.ledGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.led3NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.led2NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.led1NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button msgBasedInitButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button pollingBasedInitButton;
        private System.Windows.Forms.Label pollingStatusLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox ledGroupBox;
        private System.Windows.Forms.NumericUpDown led1NumericUpDown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown led2NumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown led3NumericUpDown;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ProgressBar pollingProgressBar;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox autoCloseCheckBox;
        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.Button startStreamingButton;
        private System.Windows.Forms.Button stopStreamingButton;
        private System.Windows.Forms.Timer tmrUpdateImage;
        private System.Windows.Forms.RichTextBox richTextBox;
    }
}