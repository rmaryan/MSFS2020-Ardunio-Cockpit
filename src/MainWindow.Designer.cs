namespace MSFS2020_Ardunio_Cockpit
{
    partial class MainWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.ComLabel = new System.Windows.Forms.Label();
            this.COMComboBox = new System.Windows.Forms.ComboBox();
            this.ConnectCheckBox = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.SerialConnectedLabel = new System.Windows.Forms.Label();
            this.SimConnectedLabel = new System.Windows.Forms.Label();
            this.LogTextBox = new System.Windows.Forms.TextBox();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoSize = true;
            this.flowLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel2.Controls.Add(this.ComLabel);
            this.flowLayoutPanel2.Controls.Add(this.COMComboBox);
            this.flowLayoutPanel2.Controls.Add(this.ConnectCheckBox);
            this.flowLayoutPanel2.Controls.Add(this.flowLayoutPanel1);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Padding = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel2.Size = new System.Drawing.Size(521, 47);
            this.flowLayoutPanel2.TabIndex = 1;
            this.flowLayoutPanel2.WrapContents = false;
            // 
            // ComLabel
            // 
            this.ComLabel.AutoSize = true;
            this.ComLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.ComLabel.Location = new System.Drawing.Point(15, 15);
            this.ComLabel.Margin = new System.Windows.Forms.Padding(10, 10, 0, 10);
            this.ComLabel.Name = "ComLabel";
            this.ComLabel.Size = new System.Drawing.Size(97, 17);
            this.ComLabel.TabIndex = 6;
            this.ComLabel.Text = "Arduino Serial Port:";
            // 
            // COMComboBox
            // 
            this.COMComboBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.COMComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.flowLayoutPanel2.SetFlowBreak(this.COMComboBox, true);
            this.COMComboBox.FormattingEnabled = true;
            this.COMComboBox.Location = new System.Drawing.Point(122, 10);
            this.COMComboBox.Margin = new System.Windows.Forms.Padding(10, 5, 10, 20);
            this.COMComboBox.Name = "COMComboBox";
            this.COMComboBox.Size = new System.Drawing.Size(121, 21);
            this.COMComboBox.TabIndex = 0;
            // 
            // ConnectCheckBox
            // 
            this.ConnectCheckBox.AutoSize = true;
            this.ConnectCheckBox.Location = new System.Drawing.Point(263, 15);
            this.ConnectCheckBox.Margin = new System.Windows.Forms.Padding(10, 10, 5, 10);
            this.ConnectCheckBox.Name = "ConnectCheckBox";
            this.ConnectCheckBox.Size = new System.Drawing.Size(66, 17);
            this.ConnectCheckBox.TabIndex = 7;
            this.ConnectCheckBox.Text = "Connect";
            this.ConnectCheckBox.UseVisualStyleBackColor = true;
            this.ConnectCheckBox.CheckedChanged += new System.EventHandler(this.ConnectCheckBox_CheckedChanged);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.SerialConnectedLabel);
            this.flowLayoutPanel1.Controls.Add(this.SimConnectedLabel);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(334, 5);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(48, 31);
            this.flowLayoutPanel1.TabIndex = 8;
            // 
            // SerialConnectedLabel
            // 
            this.SerialConnectedLabel.AutoSize = true;
            this.SerialConnectedLabel.Location = new System.Drawing.Point(3, 0);
            this.SerialConnectedLabel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this.SerialConnectedLabel.Name = "SerialConnectedLabel";
            this.SerialConnectedLabel.Size = new System.Drawing.Size(39, 13);
            this.SerialConnectedLabel.TabIndex = 0;
            this.SerialConnectedLabel.Text = "  Serial";
            // 
            // SimConnectedLabel
            // 
            this.SimConnectedLabel.AutoSize = true;
            this.SimConnectedLabel.Location = new System.Drawing.Point(3, 18);
            this.SimConnectedLabel.Name = "SimConnectedLabel";
            this.SimConnectedLabel.Size = new System.Drawing.Size(42, 13);
            this.SimConnectedLabel.TabIndex = 1;
            this.SimConnectedLabel.Text = "  MSFS";
            // 
            // LogTextBox
            // 
            this.LogTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.LogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogTextBox.Location = new System.Drawing.Point(0, 47);
            this.LogTextBox.Margin = new System.Windows.Forms.Padding(30);
            this.LogTextBox.MinimumSize = new System.Drawing.Size(150, 150);
            this.LogTextBox.Multiline = true;
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ReadOnly = true;
            this.LogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.LogTextBox.Size = new System.Drawing.Size(521, 340);
            this.LogTextBox.TabIndex = 11;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 387);
            this.Controls.Add(this.LogTextBox);
            this.Controls.Add(this.flowLayoutPanel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.Text = "MSFS2020 Ardunio Cockpit Connector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label ComLabel;
        private System.Windows.Forms.ComboBox COMComboBox;
        private System.Windows.Forms.TextBox LogTextBox;
        private System.Windows.Forms.CheckBox ConnectCheckBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label SerialConnectedLabel;
        private System.Windows.Forms.Label SimConnectedLabel;
    }
}

