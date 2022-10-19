﻿/**
 *  Cockpit controller. The main orchestration class.
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

using ArduinoConnector;
using MSFSConnector;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace MSFS2020_Ardunio_Cockpit
{
    internal class CockpitController
    {
        // handles msfs connection
        private SimControl simControl;

        // provides the presets per selected aircraft model
        private PresetsManager presetsManager;

        // handles connection to arduino device
        private ArduinoControl arduinoControl;
        private bool firmwareIsValid = false;
        private readonly string FIRMWARE_RESPONSE = "SFSCP0";

        // this array stores the Sim Variable names associated with the corresponding knobs
        private int[] knobToVarMapping = { -1, -1, -1, -1 };
        // knobs debounce time: 500 ms
        private readonly long KNOB_DEBOUNCE_TICKS = 500 * TimeSpan.TicksPerMillisecond;
        private long[] knobLastChangeTicks = { 0, 0, 0, 0 };

        private char[] switchesState = new char[CockpitPreset.SWITCHES_COUNT];

        private readonly MainWindow mainWindow_ref;

        // the connection persistance thread
        private Thread reconnectionThread;
        private bool keepReconnecting = true;

        public CockpitController(MainWindow mainWindow)
        {
            mainWindow_ref = mainWindow;

            arduinoControl = new ArduinoControl();
            arduinoControl.MessageRecieved += ArduinoMessageReceived;

            presetsManager = new PresetsManager();

            simControl = new SimControl();
            simControl.DataReceived += SimControlDataReceived;

            reconnectionThread = new Thread(ReConnectionThreadCode);
            reconnectionThread.IsBackground = true;
            reconnectionThread.Start();
        }

        public void CloseAll()
        {
            keepReconnecting = false;
            reconnectionThread.Interrupt();
            DisconnectAll();
            reconnectionThread.Join();
        }

        public void ConnectToggle()
        {
            if (mainWindow_ref.ConnectCheckBoxChecked())
            {
                ConnectToArduino(true);
                ConnectToSim(true);
            }
            else
            {
                // disconnect everything
                DisconnectAll();

                // deactivate the current preset
                presetsManager.DeactivatePreset();
                mainWindow_ref.SetSwitchLabels(presetsManager.GetPresetSwitchLabels());

                // gray out the indicators
                mainWindow_ref.SetSerialConnectedLabel(CONNECTED_STATE.STATE_INACTIVE);
                mainWindow_ref.SetMSFSConnectedLabel(CONNECTED_STATE.STATE_INACTIVE);
            }
        }

        public void ProcessMessage(ref Message m)
        {
            try
            {
                if (m.Msg == SimControl.WM_USER_SIMCONNECT)
                {
                    simControl.ReceiveSimConnectMessage();
                }
            }
            catch
            {
                simControl.Disconnect();
            }
        }

        private void SimControlDataReceived(object sender, EventArgs e)
        {
            var simActivity = (SimControl.SimActivityEventArgs)e;

            if (simActivity.Variable.Equals(SimControl.ATC_MODEL_VAR))
            {
                // A new aircraft was selected in the sim. We need to reload the cockpit layout
                presetsManager.EmergePresetForAircraft(simActivity.Value);

                mainWindow_ref.Invoke(new MethodInvoker(delegate
                {
                    mainWindow_ref.SetPresetName(presetsManager.GetPresetName());
                }));

                // feed the preset to Arduino if it is ready
                if (arduinoControl.Connected() && firmwareIsValid)
                {
                    ActivateCockpitPreset();
                }
            }
            else
            {
                int itemID = presetsManager.GetItemIDForSimVar(simActivity.Variable);
                if (itemID >= 0)
                {
                    ScreenFieldItem item = presetsManager.GetScreenFieldItem(itemID);
                    string itemIDStr = (itemID + 1).ToString("D2");
                    if (item.simvarType == SIMVAR_TYPE.TYPE_STRING)
                    {
                        arduinoControl.SendMessage('T', itemIDStr + simActivity.Value.PadLeft(item.textWidth));
                    }
                    else
                    if ((item.simvarType == SIMVAR_TYPE.TYPE_NUMBER) || (item.simvarType == SIMVAR_TYPE.TYPE_P0_NUMBER))
                    {

                        // round the value first
                        if (double.TryParse(simActivity.Value, NumberStyles.Float, null, out double dValue))
                        {
                            dValue = Math.Round(dValue, item.decimalPlaces);
                            simActivity.Value = dValue.ToString("F" + item.decimalPlaces.ToString(), CultureInfo.InvariantCulture);
                        }

                        if (item.knobSpec != "")
                        {
                            // debounce the knobs changes, ignore value changes from simvar for some time
                            if ((DateTime.Now.Ticks - knobLastChangeTicks[item.knobSpec[0] - '0']) > KNOB_DEBOUNCE_TICKS)
                            {
                                if (dValue < 0)
                                {
                                    arduinoControl.SendMessage('D', item.knobSpec[0] +
                                        "-" +
                                        simActivity.Value.Substring(1).PadLeft(4, '0'));
                                }
                                else
                                {
                                    arduinoControl.SendMessage('D', item.knobSpec[0] + 
                                        simActivity.Value.PadLeft(5, '0'));
                                }
                            }
                        }
                        else
                        {
                            if (item.simvarType == SIMVAR_TYPE.TYPE_NUMBER)
                            {
                                arduinoControl.SendMessage('T', itemIDStr + simActivity.Value.PadLeft(item.textWidth, ' '));
                            }
                            else
                            {
                                if (dValue < 0)
                                {
                                    // move the negative sign to the first place before padding with zeroes
                                    arduinoControl.SendMessage('T', itemIDStr +
                                        '-' +
                                        simActivity.Value.Substring(1).PadLeft(item.textWidth - 1, '0'));
                                }
                                else
                                {
                                    arduinoControl.SendMessage('T', itemIDStr + simActivity.Value.PadLeft(item.textWidth, '0'));
                                }
                            }
                        }
                    }
                    else
                    if (item.simvarType == SIMVAR_TYPE.TYPE_BOOLEAN)
                    {
                        // the value is false if equals to 0.0, otherwise - true
                        bool boolValue = false;
                        try
                        {
                            boolValue = (Convert.ToDouble(simActivity.Value) != 0.0);
                        }
                        finally
                        {
                            if (boolValue)
                            {
                                arduinoControl.SendMessage('I', itemIDStr + item.screenItemDefinition);
                                arduinoControl.SendMessage('T', itemIDStr + item.text);
                            }
                            else
                            {
                                // replace the color in the item definition
                                string alt_screenItemDefinition = item.screenItemDefinition.Substring(0, 6) + item.altColor + item.screenItemDefinition.Substring(10, 3) + '*';
                                arduinoControl.SendMessage('I', itemIDStr + alt_screenItemDefinition);
                                arduinoControl.SendMessage('T', itemIDStr + item.altText);
                            }
                        }
                    }
                }
            }
        }

        private void ArduinoMessageReceived(object sender, EventArgs e)
        {
            var message = (ArduinoControl.MessageEventArgs)e;

            Debug.WriteLine("R: " + message.MessageType + message.Data);
            if (!firmwareIsValid)
            {
                // expecting proper version response before starting communications
                if ((message.MessageType == 'M') && (message.Data.Equals(FIRMWARE_RESPONSE)))
                {
                    firmwareIsValid = true;

                    // do we have any preset available for activation?
                    if (presetsManager.GetScreenFieldItemsCount() > 0)
                    {
                        ActivateCockpitPreset();
                    }
                }
                else
                {
                    mainWindow_ref.Invoke(new MethodInvoker(delegate
                    {
                        keepReconnecting = false;
                        MessageBox.Show("No compatible Arduino found.", "Failed to connect", MessageBoxButtons.OK,
                                        MessageBoxIcon.Error);
                        mainWindow_ref.AppendLogMessage($"No compatible Arduino found. Received: {message.MessageType}{message.Data} Expected: {FIRMWARE_RESPONSE}");

                        mainWindow_ref.SetSerialConnectedLabel(CONNECTED_STATE.STATE_FAILED);
                        mainWindow_ref.EnableConnectCheckBox(false);
                    }));
                }
                return;
            }
            switch (message.MessageType)
            {
                case 'E':
                    // Error at the Arduino side
                    Debug.WriteLine(message.Data);
                    mainWindow_ref.AppendLogMessage(message.Data);
                    break;
                case 'K':
                    // KNvvvvv
                    if (message.Data.Length > 1)
                    {
                        int knobID = message.Data[0] - '0';

                        if ((knobID >= 0) && (knobID <= 3))
                        {
                            if (knobToVarMapping[knobID] != -1)
                            {
                                ScreenFieldItem item = presetsManager.GetScreenFieldItem(knobToVarMapping[knobID]);
                                if (item.simEventID == -1)
                                {
                                    simControl.SetVarValue(item.simVariable, message.Data.Substring(1));
                                }
                                else
                                {
                                    uint value = 0;
                                    if (!Double.TryParse(message.Data.Substring(1), NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
                                    {
                                        // malformed number
                                        break;
                                    }

                                    // a separate hack for KOHLSMAN_SET
                                    // we assume it is 
                                    if (item.simEvent.Equals("KOHLSMAN_SET"))
                                    {
                                        if (item.unitOfMeasure == "millibars")
                                        {
                                            value = (uint)(d * 16);
                                        }
                                        else
                                        {
                                            value = (uint)(d * 541.8224);
                                        }
                                    }
                                    else
                                    {
                                        value = (uint)d;
                                    }
                                    simControl.TransmitEvent((uint)item.simEventID, value);
                                }
                                knobLastChangeTicks[knobID] = DateTime.Now.Ticks;
                            }
                        }
                    }
                    break;
                case 'S':
                    for (int i = 0; i < CockpitPreset.SWITCHES_COUNT; i++)
                    {
                        if (switchesState[i] != message.Data[i])
                        {
                            if (switchesState[i] == '1')
                            {
                                int eventOFFID = presetsManager.GetSwitchEventOFFID(i);
                                if (eventOFFID > -1)
                                {
                                    simControl.TransmitEvent((uint)eventOFFID, presetsManager.GetSwitchEventOFFValue(i));
                                }
                            }
                            else
                            if (message.Data[i] == '1')
                            {
                                int eventONID = presetsManager.GetSwitchEventONID(i);
                                if (eventONID > -1)
                                {
                                    simControl.TransmitEvent((uint)eventONID, presetsManager.GetSwitchEventONValue(i));
                                }
                            }
                            switchesState[i] = message.Data[i];
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        private void ReConnectionThreadCode()
        {
            // send ping to Arduino only once
            bool pingSent = false;

            // if any connection was lost, try to reconnect as long as "Connect" check box is active
            while (keepReconnecting)
            {
                if (mainWindow_ref.ConnectCheckBoxChecked())
                {
                    if (!arduinoControl.Connected())
                    {
                        // make sure to switch to the UI thread
                        mainWindow_ref.Invoke(new MethodInvoker(delegate { ConnectToArduino(false); }));
                    }
                    else
                    {
                        if (!firmwareIsValid)
                        {
                            // Check for proper version
                            if (!pingSent)
                            {
                                arduinoControl.SendMessage('P', "");
                            }
                        }
                    }

                    if (!simControl.ConnectedSimConnect)
                    {
                        // make sure to switch to the UI thread
                        mainWindow_ref.Invoke(new MethodInvoker(delegate { ConnectToSim(false); }));
                    }
                }
                try
                {
                    Thread.Sleep(5000);
                }
                catch (System.Threading.ThreadInterruptedException)
                {
                    if (!keepReconnecting) break;
                }
            }
        }
        private void ConnectToArduino(bool verbose)
        {
            string serialPort = mainWindow_ref.GetSelectedSerialPort();
            if (serialPort == null)
            {
                if (verbose)
                {
                    MessageBox.Show($"Please select the Arduino's Serial port", "Failed to connect", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                mainWindow_ref.SetSerialConnectedLabel(CONNECTED_STATE.STATE_FAILED);
                return;
            }
            try
            {
                arduinoControl.Connect(serialPort);
                mainWindow_ref.SetSerialConnectedLabel(CONNECTED_STATE.STATE_OK);
            }
            catch (Exception exception)
            {
                if (verbose)
                {
                    MessageBox.Show($"Failed to connect to Arduino", "Failed to connect", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                    Console.WriteLine(exception);
                }
                mainWindow_ref.SetSerialConnectedLabel(CONNECTED_STATE.STATE_FAILED);
            }
        }

        private void ConnectToSim(bool verbose)
        {
            try
            {
                simControl.Connect(mainWindow_ref.Handle);
                mainWindow_ref.SetMSFSConnectedLabel(CONNECTED_STATE.STATE_OK);
            }
            catch (Exception exception)
            {
                if (verbose)
                {
                    MessageBox.Show($"Failed to connect to simulator\n{exception.Message}", "Failed to connect", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                    Console.WriteLine(exception);
                }
                mainWindow_ref.SetMSFSConnectedLabel(CONNECTED_STATE.STATE_FAILED);
            }
        }

        private void DisconnectAll()
        {
            if (simControl.ConnectedSimConnect)
            {
                try
                {
                    simControl.Disconnect();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
            if (arduinoControl.Connected())
            {
                try
                {
                    arduinoControl.Disconnect();
                    firmwareIsValid = false;
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private void ActivateCockpitPreset()
        {
            // Unsubscribe from the previous sim variables (if any)
            simControl.RemoveAllRequests();

            // clear old knob mappings
            for (int i = 0; i < 4; i++)
            {
                knobToVarMapping[i] = -1;
            }

            // C - start configuration
            arduinoControl.SendMessage('C', presetsManager.GetScreenFieldItemsCount().ToString("D2"));
            // B - background color
            arduinoControl.SendMessage('B', presetsManager.GetBGColor());

            for (int i = 0; i < presetsManager.GetScreenFieldItemsCount(); i++)
            {
                ScreenFieldItem item = presetsManager.GetScreenFieldItem(i);
                string itemID = (i + 1).ToString("D2");
                // I - definition of the items (one line per item)
                arduinoControl.SendMessage('I', itemID + item.screenItemDefinition);

                if (item.knobSpec != "")
                {
                    // knob-bound items are initialized a bit differently
                    int knobID = item.knobSpec[0] - '0';
                    if ((knobID >= 0) && (knobID <= 3))
                    {
                        //HACK: Getting knob step from the simvar id is not implemented yet

                        // record the knob to SimVar name mapping
                        knobToVarMapping[knobID] = i;

                        // KNFFmmmmmmMMMMMMCDSSSS
                        arduinoControl.SendMessage('K', item.knobSpec[0] + itemID + item.knobSpec.Substring(1) + item.decimalPlaces + item.knobStep);
                        // DNVVVVV
                        arduinoControl.SendMessage('D', item.knobSpec[0] + item.text.PadLeft(5, '0'));
                    }
                }
                else
                {
                    // T - sending the initial text value
                    arduinoControl.SendMessage('T', itemID + item.text);
                }
                // Give Arduino some time to process the preset items
                Thread.Sleep(100);
            }

            // S - submit all layout changes
            arduinoControl.SendMessage('S', "");

            // having the layout processed - register for the sim events
            for (int i = 0; i < presetsManager.GetScreenFieldItemsCount(); i++)
            {
                ScreenFieldItem item = presetsManager.GetScreenFieldItem(i);
                // request the variable values monitoring in the sim
                if (!item.simVariable.Equals(""))
                {
                    simControl.AddVarRequest(item.simVariable, item.unitOfMeasure);
                    if (!item.simEvent.Equals(""))
                    {
                        item.simEventID = simControl.RegisterEvent(item.simEvent);
                    }
                }

            }

            // register the switches events
            for (int i = 0; i < CockpitPreset.SWITCHES_COUNT; i++)
            {
                string eventON = presetsManager.GetSwitchEventON(i);
                string eventOFF = presetsManager.GetSwitchEventOFF(i);
                if (!eventON.Equals(""))
                {
                    presetsManager.SetSwitchEventONID(i, simControl.RegisterEvent(eventON));
                }
                if (!eventOFF.Equals(""))
                {
                    presetsManager.SetSwitchEventOFFID(i, simControl.RegisterEvent(eventOFF));
                }
            }

            mainWindow_ref.SetSwitchLabels(presetsManager.GetPresetSwitchLabels());
        }

        public void ShowCurrentPresetJSON()
        {
            if(presetsManager.pID > -1)
            {
                mainWindow_ref.AppendLogMessage(JsonConvert.SerializeObject(presetsManager.presets[presetsManager.pID]));
            }           
        }
    }
}
