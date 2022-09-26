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
            this.panel1 = new System.Windows.Forms.Panel();
            this.LogTextBox = new System.Windows.Forms.TextBox();
            this.switchesTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelSW1011 = new System.Windows.Forms.Label();
            this.labelSW67 = new System.Windows.Forms.Label();
            this.labelSW45 = new System.Windows.Forms.Label();
            this.labelSW89 = new System.Windows.Forms.Label();
            this.labelSW23 = new System.Windows.Forms.Label();
            this.pictureBox14 = new System.Windows.Forms.PictureBox();
            this.pictureBox13 = new System.Windows.Forms.PictureBox();
            this.pictureBox12 = new System.Windows.Forms.PictureBox();
            this.pictureBox11 = new System.Windows.Forms.PictureBox();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.labelSW01 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.labelENC1 = new System.Windows.Forms.Label();
            this.labelENC2 = new System.Windows.Forms.Label();
            this.labelENC3 = new System.Windows.Forms.Label();
            this.labelENC4 = new System.Windows.Forms.Label();
            this.labelSW16 = new System.Windows.Forms.Label();
            this.labelSW17 = new System.Windows.Forms.Label();
            this.labelSW18 = new System.Windows.Forms.Label();
            this.labelSW19 = new System.Windows.Forms.Label();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.switchesTableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
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
            this.flowLayoutPanel2.Size = new System.Drawing.Size(896, 47);
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
            // panel1
            // 
            this.panel1.Controls.Add(this.LogTextBox);
            this.panel1.Controls.Add(this.switchesTableLayoutPanel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 47);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(896, 350);
            this.panel1.TabIndex = 14;
            // 
            // LogTextBox
            // 
            this.LogTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.LogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogTextBox.Location = new System.Drawing.Point(0, 148);
            this.LogTextBox.Margin = new System.Windows.Forms.Padding(0);
            this.LogTextBox.MinimumSize = new System.Drawing.Size(150, 150);
            this.LogTextBox.Multiline = true;
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ReadOnly = true;
            this.LogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.LogTextBox.Size = new System.Drawing.Size(896, 202);
            this.LogTextBox.TabIndex = 15;
            // 
            // switchesTableLayoutPanel
            // 
            this.switchesTableLayoutPanel.AutoSize = true;
            this.switchesTableLayoutPanel.ColumnCount = 14;
            this.switchesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.switchesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.switchesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.switchesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.switchesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.switchesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.switchesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.switchesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.switchesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.switchesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.switchesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.switchesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.switchesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 37F));
            this.switchesTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 14.28572F));
            this.switchesTableLayoutPanel.Controls.Add(this.labelSW1011, 5, 1);
            this.switchesTableLayoutPanel.Controls.Add(this.labelSW67, 1, 1);
            this.switchesTableLayoutPanel.Controls.Add(this.labelSW45, 5, 0);
            this.switchesTableLayoutPanel.Controls.Add(this.labelSW89, 3, 1);
            this.switchesTableLayoutPanel.Controls.Add(this.labelSW23, 3, 0);
            this.switchesTableLayoutPanel.Controls.Add(this.pictureBox14, 12, 1);
            this.switchesTableLayoutPanel.Controls.Add(this.pictureBox13, 10, 1);
            this.switchesTableLayoutPanel.Controls.Add(this.pictureBox12, 8, 1);
            this.switchesTableLayoutPanel.Controls.Add(this.pictureBox11, 6, 1);
            this.switchesTableLayoutPanel.Controls.Add(this.pictureBox10, 12, 0);
            this.switchesTableLayoutPanel.Controls.Add(this.pictureBox9, 10, 0);
            this.switchesTableLayoutPanel.Controls.Add(this.pictureBox8, 8, 0);
            this.switchesTableLayoutPanel.Controls.Add(this.pictureBox7, 6, 0);
            this.switchesTableLayoutPanel.Controls.Add(this.pictureBox6, 4, 1);
            this.switchesTableLayoutPanel.Controls.Add(this.pictureBox5, 2, 1);
            this.switchesTableLayoutPanel.Controls.Add(this.pictureBox4, 4, 0);
            this.switchesTableLayoutPanel.Controls.Add(this.pictureBox3, 2, 0);
            this.switchesTableLayoutPanel.Controls.Add(this.pictureBox2, 0, 0);
            this.switchesTableLayoutPanel.Controls.Add(this.labelSW01, 1, 0);
            this.switchesTableLayoutPanel.Controls.Add(this.pictureBox1, 0, 1);
            this.switchesTableLayoutPanel.Controls.Add(this.labelENC1, 7, 0);
            this.switchesTableLayoutPanel.Controls.Add(this.labelENC2, 9, 0);
            this.switchesTableLayoutPanel.Controls.Add(this.labelENC3, 11, 0);
            this.switchesTableLayoutPanel.Controls.Add(this.labelENC4, 13, 0);
            this.switchesTableLayoutPanel.Controls.Add(this.labelSW16, 7, 1);
            this.switchesTableLayoutPanel.Controls.Add(this.labelSW17, 9, 1);
            this.switchesTableLayoutPanel.Controls.Add(this.labelSW18, 11, 1);
            this.switchesTableLayoutPanel.Controls.Add(this.labelSW19, 13, 1);
            this.switchesTableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.switchesTableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.switchesTableLayoutPanel.Name = "switchesTableLayoutPanel";
            this.switchesTableLayoutPanel.RowCount = 3;
            this.switchesTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.switchesTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 64F));
            this.switchesTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.switchesTableLayoutPanel.Size = new System.Drawing.Size(896, 148);
            this.switchesTableLayoutPanel.TabIndex = 14;
            // 
            // labelSW1011
            // 
            this.labelSW1011.AutoSize = true;
            this.labelSW1011.Location = new System.Drawing.Point(294, 64);
            this.labelSW1011.Name = "labelSW1011";
            this.labelSW1011.Size = new System.Drawing.Size(57, 13);
            this.labelSW1011.TabIndex = 20;
            this.labelSW1011.Text = "SW 10/11";
            // 
            // labelSW67
            // 
            this.labelSW67.AutoSize = true;
            this.labelSW67.Location = new System.Drawing.Point(40, 64);
            this.labelSW67.Name = "labelSW67";
            this.labelSW67.Size = new System.Drawing.Size(45, 13);
            this.labelSW67.TabIndex = 19;
            this.labelSW67.Text = "SW 6/7";
            // 
            // labelSW45
            // 
            this.labelSW45.AutoSize = true;
            this.labelSW45.Location = new System.Drawing.Point(294, 0);
            this.labelSW45.Name = "labelSW45";
            this.labelSW45.Size = new System.Drawing.Size(45, 13);
            this.labelSW45.TabIndex = 18;
            this.labelSW45.Text = "SW 4/5";
            // 
            // labelSW89
            // 
            this.labelSW89.AutoSize = true;
            this.labelSW89.Location = new System.Drawing.Point(167, 64);
            this.labelSW89.Name = "labelSW89";
            this.labelSW89.Size = new System.Drawing.Size(45, 13);
            this.labelSW89.TabIndex = 17;
            this.labelSW89.Text = "SW 8/9";
            // 
            // labelSW23
            // 
            this.labelSW23.AutoSize = true;
            this.labelSW23.Location = new System.Drawing.Point(167, 0);
            this.labelSW23.Name = "labelSW23";
            this.labelSW23.Size = new System.Drawing.Size(45, 13);
            this.labelSW23.TabIndex = 16;
            this.labelSW23.Text = "SW 2/3";
            // 
            // pictureBox14
            // 
            this.pictureBox14.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox14.Image = global::MSFS2020_Ardunio_Cockpit.Properties.Resources.pushb;
            this.pictureBox14.Location = new System.Drawing.Point(768, 67);
            this.pictureBox14.Name = "pictureBox14";
            this.pictureBox14.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pictureBox14.Size = new System.Drawing.Size(31, 58);
            this.pictureBox14.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox14.TabIndex = 15;
            this.pictureBox14.TabStop = false;
            // 
            // pictureBox13
            // 
            this.pictureBox13.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox13.Image = global::MSFS2020_Ardunio_Cockpit.Properties.Resources.pushb;
            this.pictureBox13.Location = new System.Drawing.Point(641, 67);
            this.pictureBox13.Name = "pictureBox13";
            this.pictureBox13.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pictureBox13.Size = new System.Drawing.Size(31, 58);
            this.pictureBox13.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox13.TabIndex = 14;
            this.pictureBox13.TabStop = false;
            // 
            // pictureBox12
            // 
            this.pictureBox12.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox12.Image = global::MSFS2020_Ardunio_Cockpit.Properties.Resources.pushb;
            this.pictureBox12.Location = new System.Drawing.Point(514, 67);
            this.pictureBox12.Name = "pictureBox12";
            this.pictureBox12.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pictureBox12.Size = new System.Drawing.Size(31, 58);
            this.pictureBox12.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox12.TabIndex = 13;
            this.pictureBox12.TabStop = false;
            // 
            // pictureBox11
            // 
            this.pictureBox11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox11.Image = global::MSFS2020_Ardunio_Cockpit.Properties.Resources.pushb;
            this.pictureBox11.Location = new System.Drawing.Point(384, 67);
            this.pictureBox11.Name = "pictureBox11";
            this.pictureBox11.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pictureBox11.Size = new System.Drawing.Size(34, 58);
            this.pictureBox11.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox11.TabIndex = 12;
            this.pictureBox11.TabStop = false;
            // 
            // pictureBox10
            // 
            this.pictureBox10.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox10.Image = global::MSFS2020_Ardunio_Cockpit.Properties.Resources.encoder;
            this.pictureBox10.Location = new System.Drawing.Point(768, 3);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pictureBox10.Size = new System.Drawing.Size(31, 58);
            this.pictureBox10.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox10.TabIndex = 11;
            this.pictureBox10.TabStop = false;
            // 
            // pictureBox9
            // 
            this.pictureBox9.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox9.Image = global::MSFS2020_Ardunio_Cockpit.Properties.Resources.encoder;
            this.pictureBox9.Location = new System.Drawing.Point(641, 3);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pictureBox9.Size = new System.Drawing.Size(31, 58);
            this.pictureBox9.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox9.TabIndex = 10;
            this.pictureBox9.TabStop = false;
            // 
            // pictureBox8
            // 
            this.pictureBox8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox8.Image = global::MSFS2020_Ardunio_Cockpit.Properties.Resources.encoder;
            this.pictureBox8.Location = new System.Drawing.Point(514, 3);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pictureBox8.Size = new System.Drawing.Size(31, 58);
            this.pictureBox8.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox8.TabIndex = 9;
            this.pictureBox8.TabStop = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox7.Image = global::MSFS2020_Ardunio_Cockpit.Properties.Resources.encoder;
            this.pictureBox7.Location = new System.Drawing.Point(384, 3);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.pictureBox7.Size = new System.Drawing.Size(34, 58);
            this.pictureBox7.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox7.TabIndex = 8;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox6.Image = global::MSFS2020_Ardunio_Cockpit.Properties.Resources._3switch;
            this.pictureBox6.Location = new System.Drawing.Point(257, 67);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pictureBox6.Size = new System.Drawing.Size(31, 58);
            this.pictureBox6.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox6.TabIndex = 7;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox5.Image = global::MSFS2020_Ardunio_Cockpit.Properties.Resources._3switch;
            this.pictureBox5.Location = new System.Drawing.Point(130, 67);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pictureBox5.Size = new System.Drawing.Size(31, 58);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox5.TabIndex = 6;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox4.Image = global::MSFS2020_Ardunio_Cockpit.Properties.Resources._3switch;
            this.pictureBox4.Location = new System.Drawing.Point(257, 3);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pictureBox4.Size = new System.Drawing.Size(31, 58);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox4.TabIndex = 5;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox3.Image = global::MSFS2020_Ardunio_Cockpit.Properties.Resources._3switch;
            this.pictureBox3.Location = new System.Drawing.Point(130, 3);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pictureBox3.Size = new System.Drawing.Size(31, 58);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox3.TabIndex = 4;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox2.Image = global::MSFS2020_Ardunio_Cockpit.Properties.Resources._3switch;
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pictureBox2.Size = new System.Drawing.Size(31, 58);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // labelSW01
            // 
            this.labelSW01.AutoSize = true;
            this.labelSW01.Location = new System.Drawing.Point(40, 0);
            this.labelSW01.Name = "labelSW01";
            this.labelSW01.Size = new System.Drawing.Size(45, 13);
            this.labelSW01.TabIndex = 1;
            this.labelSW01.Text = "SW 0/1";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::MSFS2020_Ardunio_Cockpit.Properties.Resources._3switch;
            this.pictureBox1.Location = new System.Drawing.Point(3, 67);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.pictureBox1.Size = new System.Drawing.Size(31, 58);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // labelENC1
            // 
            this.labelENC1.AutoSize = true;
            this.labelENC1.Location = new System.Drawing.Point(424, 0);
            this.labelENC1.Name = "labelENC1";
            this.labelENC1.Size = new System.Drawing.Size(82, 13);
            this.labelENC1.TabIndex = 21;
            this.labelENC1.Text = "ENC 1 / SW 12";
            // 
            // labelENC2
            // 
            this.labelENC2.AutoSize = true;
            this.labelENC2.Location = new System.Drawing.Point(551, 0);
            this.labelENC2.Name = "labelENC2";
            this.labelENC2.Size = new System.Drawing.Size(82, 13);
            this.labelENC2.TabIndex = 22;
            this.labelENC2.Text = "ENC 2 / SW 13";
            // 
            // labelENC3
            // 
            this.labelENC3.AutoSize = true;
            this.labelENC3.Location = new System.Drawing.Point(678, 0);
            this.labelENC3.Name = "labelENC3";
            this.labelENC3.Size = new System.Drawing.Size(82, 13);
            this.labelENC3.TabIndex = 23;
            this.labelENC3.Text = "ENC 3 / SW 14";
            // 
            // labelENC4
            // 
            this.labelENC4.AutoSize = true;
            this.labelENC4.Location = new System.Drawing.Point(805, 0);
            this.labelENC4.Name = "labelENC4";
            this.labelENC4.Size = new System.Drawing.Size(82, 13);
            this.labelENC4.TabIndex = 24;
            this.labelENC4.Text = "ENC 4 / SW 15";
            // 
            // labelSW16
            // 
            this.labelSW16.AutoSize = true;
            this.labelSW16.Location = new System.Drawing.Point(424, 64);
            this.labelSW16.Name = "labelSW16";
            this.labelSW16.Size = new System.Drawing.Size(40, 13);
            this.labelSW16.TabIndex = 25;
            this.labelSW16.Text = "SW 16";
            // 
            // labelSW17
            // 
            this.labelSW17.AutoSize = true;
            this.labelSW17.Location = new System.Drawing.Point(551, 64);
            this.labelSW17.Name = "labelSW17";
            this.labelSW17.Size = new System.Drawing.Size(40, 13);
            this.labelSW17.TabIndex = 26;
            this.labelSW17.Text = "SW 17";
            // 
            // labelSW18
            // 
            this.labelSW18.AutoSize = true;
            this.labelSW18.Location = new System.Drawing.Point(678, 64);
            this.labelSW18.Name = "labelSW18";
            this.labelSW18.Size = new System.Drawing.Size(40, 13);
            this.labelSW18.TabIndex = 27;
            this.labelSW18.Text = "SW 18";
            // 
            // labelSW19
            // 
            this.labelSW19.AutoSize = true;
            this.labelSW19.Location = new System.Drawing.Point(805, 64);
            this.labelSW19.Name = "labelSW19";
            this.labelSW19.Size = new System.Drawing.Size(40, 13);
            this.labelSW19.TabIndex = 28;
            this.labelSW19.Text = "SW 19";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(896, 397);
            this.Controls.Add(this.panel1);
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
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.switchesTableLayoutPanel.ResumeLayout(false);
            this.switchesTableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox14)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox13)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox12)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox11)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.Label ComLabel;
        private System.Windows.Forms.ComboBox COMComboBox;
        private System.Windows.Forms.CheckBox ConnectCheckBox;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label SerialConnectedLabel;
        private System.Windows.Forms.Label SimConnectedLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox LogTextBox;
        private System.Windows.Forms.TableLayoutPanel switchesTableLayoutPanel;
        private System.Windows.Forms.Label labelSW1011;
        private System.Windows.Forms.Label labelSW67;
        private System.Windows.Forms.Label labelSW45;
        private System.Windows.Forms.Label labelSW89;
        private System.Windows.Forms.Label labelSW23;
        private System.Windows.Forms.PictureBox pictureBox14;
        private System.Windows.Forms.PictureBox pictureBox13;
        private System.Windows.Forms.PictureBox pictureBox12;
        private System.Windows.Forms.PictureBox pictureBox11;
        private System.Windows.Forms.PictureBox pictureBox10;
        private System.Windows.Forms.PictureBox pictureBox9;
        private System.Windows.Forms.PictureBox pictureBox8;
        private System.Windows.Forms.PictureBox pictureBox7;
        private System.Windows.Forms.PictureBox pictureBox6;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label labelSW01;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelENC1;
        private System.Windows.Forms.Label labelENC2;
        private System.Windows.Forms.Label labelENC3;
        private System.Windows.Forms.Label labelENC4;
        private System.Windows.Forms.Label labelSW16;
        private System.Windows.Forms.Label labelSW17;
        private System.Windows.Forms.Label labelSW18;
        private System.Windows.Forms.Label labelSW19;
    }
}

