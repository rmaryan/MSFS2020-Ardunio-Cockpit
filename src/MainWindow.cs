/**
 *  The main window.
 *
 *  Copyright (C) 2022 by Mar'yan Rachynskyy
 *  rmaryan@gmail.com
 *
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Drawing;
using System.Windows.Forms;

namespace MSFS2020_Ardunio_Cockpit
{
    public enum CONNECTED_STATE
    {
        STATE_OK,
        STATE_FAILED,
        STATE_INACTIVE
    }

    public partial class MainWindow : Form
    {
        private CockpitController cockpitController = null;
        private const uint SWITCH_LABELS_COUNT = 14;
        private Label[] switchLabels;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            if (cockpitController != null)
            {
                cockpitController.ProcessMessage(ref m);
            }

            base.WndProc(ref m);
        }

        private void GenerateSerialPortsList()
        {
            string[] serialPortNames = System.IO.Ports.SerialPort.GetPortNames();

            foreach (string name in serialPortNames)
            {
                COMComboBox.Items.Add(name);
            }
            if (COMComboBox.Items.Count > 0)
            {
                COMComboBox.SelectedIndex = 0;
            }
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {
            GenerateSerialPortsList();

            switchLabels = new Label[SWITCH_LABELS_COUNT];
            switchLabels[0] = labelSW01;
            switchLabels[1] = labelSW23;
            switchLabels[2] = labelSW45;
            switchLabels[3] = labelSW67;
            switchLabels[4] = labelSW89;
            switchLabels[5] = labelSW1011;
            switchLabels[6] = labelENC1;
            switchLabels[7] = labelENC2;
            switchLabels[8] = labelENC3;
            switchLabels[9] = labelENC4;
            switchLabels[10] = labelSW16;
            switchLabels[11] = labelSW17;
            switchLabels[12] = labelSW18;
            switchLabels[13] = labelSW19;

            cockpitController = new CockpitController(this);
        }


        private void ConnectCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                cockpitController.ConnectToggle();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            try
            {
                cockpitController.CloseAll();
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }

        }

        public string GetSelectedSerialPort()
        {
            return COMComboBox.Text;
        }

        public bool ConnectCheckBoxChecked()
        {
            return ConnectCheckBox.Checked;
        }

        public void EnableConnectCheckBox(bool enabled = true)
        {
            ConnectCheckBox.Enabled = enabled;
        }

        public void AppendLogMessage(string message)
        {
            LogTextBox.Invoke(new MethodInvoker(delegate { LogTextBox.AppendText($"{message}{Environment.NewLine}"); }));
        }

        public void SetPresetName(string name)
        {
            LogTextBox.Invoke(new MethodInvoker(delegate { presetNameLabel.Text = name; }));
        }

        public void SetSerialConnectedLabel(CONNECTED_STATE state)
        {
            LogTextBox.Invoke(new MethodInvoker(delegate
            {
                switch (state)
                {
                    case CONNECTED_STATE.STATE_OK:
                        SerialConnectedLabel.Text = "+ Serial";
                        SerialConnectedLabel.ForeColor = Color.Green;
                        break;
                    case CONNECTED_STATE.STATE_FAILED:
                        SerialConnectedLabel.Text = "X Serial";
                        SerialConnectedLabel.ForeColor = Color.Red;
                        break;
                    default:
                        SerialConnectedLabel.Text = "  Serial";
                        SerialConnectedLabel.ForeColor = SystemColors.ControlText;
                        break;
                }
            }));
        }
        public void SetMSFSConnectedLabel(CONNECTED_STATE state)
        {
            LogTextBox.Invoke(new MethodInvoker(delegate
            {
                switch (state)
                {
                    case CONNECTED_STATE.STATE_OK:
                        SimConnectedLabel.Text = "+ MSFS";
                        SimConnectedLabel.ForeColor = Color.Green;
                        break;
                    case CONNECTED_STATE.STATE_FAILED:
                        SimConnectedLabel.Text = "X MSFS";
                        SimConnectedLabel.ForeColor = Color.Red;
                        break;
                    default:
                        SimConnectedLabel.Text = "  MSFS";
                        SimConnectedLabel.ForeColor = SystemColors.ControlText;
                        break;
                }
            }));
        }
        public void SetSwitchLabels(string[] switchLabelText)
        {
            LogTextBox.Invoke(new MethodInvoker(delegate
            {
                for (int i = 0; i < SWITCH_LABELS_COUNT; i++)
                {
                    switchLabels[i].Text = switchLabelText[i];
                }
            }));
        }
    }
}
