/**
 *  The bridge to the SimConnect.
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

using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace MSFSConnector
{
    public class SimControl
    {
        /// simconnect connection
        public const int WM_USER_SIMCONNECT = 0x0402;
        private IntPtr _hWnd = new IntPtr(0);
        private SimConnect _simConnect = null;

        private List<SimVar> monitoredVars;
        private List<String> usedEvents;

        public const string ATC_MODEL_VAR = "ATC MODEL";

        public string CurrentAircraftType { get; private set; } = "";

        public event EventHandler DataReceived;

        public bool Connected { get; private set; } = false;

        public SimControl()
        {
            monitoredVars = new List<SimVar>();
            usedEvents = new List<String>();
        }

        public void ReceiveSimConnectMessage()
        {
            _simConnect?.ReceiveMessage();
        }

        public void Connect(IntPtr hWnd)
        {
            _hWnd = hWnd;

            Debug.WriteLine("Connecting to sim...");

            if (_simConnect == null)
            {
                try
                {
                    _simConnect = new SimConnect("Arduino Cockpit", _hWnd, WM_USER_SIMCONNECT, null, 0);

                    _simConnect.OnRecvOpen += new SimConnect.RecvOpenEventHandler(SimConnect_OnConnected);
                    _simConnect.OnRecvQuit += new SimConnect.RecvQuitEventHandler(SimConnect_OnDisconnected);
                    _simConnect.OnRecvException += new SimConnect.RecvExceptionEventHandler(SimConnect_OnError);
                    _simConnect.OnRecvSimobjectData += new SimConnect.RecvSimobjectDataEventHandler(SimConnect_OnGotData);
                    //                    _simConnect.OnRecvEvent += new SimConnect.RecvEventEventHandler(SimConnect_OnRecvEvent);
                }
                catch (COMException ex)
                {
                    throw new ApplicationException("Connection to MSFS failed. Make sure its running.", ex);
                }
            }
        }

        public void Disconnect()
        {
            Debug.WriteLine("Disconnecting from sim...");

            if (_simConnect != null)
            {
                RemoveAllRequests();
                _simConnect.Dispose();
                _simConnect = null;
                monitoredVars.Clear();
                usedEvents.Clear();
            }

            Connected = false;
        }

        private void SimConnect_OnConnected(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            Debug.WriteLine("SimConnect Connected");
            Connected = true;

            // Register to receive aircraft type - this is a key to choose the cockpit layout
            AddRequest(ATC_MODEL_VAR, "");

        }

        private void SimConnect_OnDisconnected(SimConnect sender, SIMCONNECT_RECV data)
        {
            Debug.WriteLine("SimConnect Disconnected");
            monitoredVars.Clear();
            usedEvents.Clear();
            Disconnect();
        }

        private void SimConnect_OnError(SimConnect sender, SIMCONNECT_RECV_EXCEPTION data)
        {
            SIMCONNECT_EXCEPTION eException = (SIMCONNECT_EXCEPTION)data.dwException;
            Debug.WriteLine("SimConnect_OnError: " + eException.ToString());
        }

        private void SimConnect_OnGotData(SimConnect sender, SIMCONNECT_RECV_SIMOBJECT_DATA data)
        {
            if (data.dwRequestID < monitoredVars.Count)
            {

                string value;
                if (monitoredVars[(int)data.dwRequestID].unit.Equals(""))
                {
                    // this is a string
                    Struct1 result = (Struct1)data.dwData[0];
                    value = result.sValue;

                    // Aircraft model variable is always #0
                    if (data.dwRequestID == 0)
                    {
                        CurrentAircraftType = value;
                    }
                }
                else
                {
                    // this is a double
                    double dValue = (double)data.dwData[0];
                    value = dValue.ToString("F9");
                }

                DataReceived?.Invoke(this, new SimActivityEventArgs(monitoredVars[(int)data.dwRequestID].name, value));
            }
        }

        public void AddRequest(string simConnectVariable, string simConnectUnit)
        {
            if (_simConnect == null) return;

            int nextID = monitoredVars.Count;
            monitoredVars.Add(new SimVar(simConnectVariable, simConnectUnit));

            if (simConnectUnit.Equals(""))
            {
                // this is a string variable
                _simConnect.AddToDataDefinition((DEFINITION)nextID, simConnectVariable, simConnectUnit,
                    SIMCONNECT_DATATYPE.STRING256, 0.01f, SimConnect.SIMCONNECT_UNUSED);
                _simConnect.RegisterDataDefineStruct<Struct1>((DEFINITION)nextID);
            }
            else
            {
                _simConnect.AddToDataDefinition((DEFINITION)nextID, simConnectVariable, simConnectUnit,
                    SIMCONNECT_DATATYPE.FLOAT64, 0.01f, SimConnect.SIMCONNECT_UNUSED);
                _simConnect.RegisterDataDefineStruct<double>((DEFINITION)nextID);
            }

            _simConnect?.RequestDataOnSimObject(
                (DEFINITION)nextID,
                (DEFINITION)nextID,
                SimConnect.SIMCONNECT_OBJECT_ID_USER,
                SIMCONNECT_PERIOD.SECOND,  // refreshing the data once per second
                SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0);
        }

        public void SetDataValue(string simConnectVariable, string value)
        {
            int simvarID = monitoredVars.FindIndex(p => p.name.Equals(simConnectVariable));
            if (simvarID >= 0)
            {
                if (monitoredVars[simvarID].unit == "")
                {
                    // Send a String value
                    Struct1 sValueStruct = new Struct1()
                    {
                        sValue = value
                    };
                    _simConnect.SetDataOnSimObject((DEFINITION)simvarID, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_DATA_SET_FLAG.DEFAULT, sValueStruct);
                }
                else
                {
                    // Send a double value
                    if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double dValue))
                    {
                        _simConnect.SetDataOnSimObject((DEFINITION)simvarID, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_DATA_SET_FLAG.DEFAULT, dValue);
                    }
                }
            }
        }

        public int RegisterEvent(string eventName)
        {
            if (_simConnect == null) return -1;

            int nextEventID = usedEvents.Count;
            _simConnect.MapClientEventToSimEvent((DEFINITION)nextEventID, eventName);
            usedEvents.Add(eventName);
            return nextEventID;
        }

        public void TransmitEvent(uint eventID, uint data)
        {

            _simConnect.TransmitClientEvent(SimConnect.SIMCONNECT_OBJECT_ID_USER,
                (DEFINITION)eventID, data,
                (DEFINITION)1, SIMCONNECT_EVENT_FLAG.GROUPID_IS_PRIORITY);
        }

        public void RemoveAllRequests()
        {
            // keep the ATC MODEL listener active
            if (monitoredVars.Count > 1)
            {
                for (int i = 1; i < monitoredVars.Count; i++)
                {
                    _simConnect.ClearDataDefinition((DEFINITION)i);
                }
                monitoredVars.RemoveRange(1, monitoredVars.Count - 1);
            }
            usedEvents.Clear();
        }

        /**
         * Helper classes and other definitions
         */

        public class SimVar
        {
            public string name;
            public string unit;

            public SimVar(string name, string unit)
            {
                this.name = name;
                this.unit = unit;
            }
        }

        public class SimActivityEventArgs : EventArgs
        {
            public SimActivityEventArgs(string variable, string value)
            {
                this.Variable = variable;
                this.Value = value;
            }

            public string Variable { get; set; }

            public string Value { get; set; }

        }

        public enum DEFINITION
        {
            Dummy = 0
        };

        // String properties must be packed inside of such struct
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        struct Struct1
        {
            // this is how you declare a fixed size string
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public String sValue;
        };
    }
}
