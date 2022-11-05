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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using static MSFSConnector.SimControl;

namespace MSFSConnector
{
    public class SimControl
    {
        /// simconnect connection
        public const int WM_USER_SIMCONNECT = 0x0402;
        private IntPtr _hWnd = new IntPtr(0);
        private SimConnect _simConnect = null;

        private readonly List<SimVar> monitoredVars = new List<SimVar>();
        private readonly List<String> usedEvents = new List<String>();
        private readonly List<String> WASMSimVars = new List<String>();

        public const string ATC_MODEL_VAR = "ATC MODEL";

        public string CurrentAircraftType { get; private set; } = "";

        public event EventHandler DataReceived;

        public bool ConnectedSimConnect { get; private set; } = false;
        public bool ConnectedWASM { get; private set; } = false;

        private const string WASM_CLIENT_NAME = "ArduFlight";
        private const string WASM_CLIENT_DATA_NAME_SIMVAR = WASM_CLIENT_NAME + ".LVars";
        private const string WASM_CLIENT_DATA_NAME_COMMAND = WASM_CLIENT_NAME + ".Command";
        private const string WASM_CLIENT_DATA_NAME_RESPONSE = WASM_CLIENT_NAME + ".Response";



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
                    _simConnect.OnRecvClientData += new SimConnect.RecvClientDataEventHandler(SimConnect_OnGotWASMData);
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
                WASMSimVars.Clear();
            }

            ConnectedSimConnect = false;
            ConnectedWASM = false;
        }

        private void SimConnect_OnConnected(SimConnect sender, SIMCONNECT_RECV_OPEN data)
        {
            Debug.WriteLine("SimConnect Connected");
            ConnectedSimConnect = true;

            // Register to receive aircraft type - this is a key to choose the cockpit layout
            AddVarRequest(ATC_MODEL_VAR, "");

            // Kick-off the WASM module
            WasmModuleClient.AddClient(_simConnect, WASM_CLIENT_NAME);
        }

        private void SimConnect_OnDisconnected(SimConnect sender, SIMCONNECT_RECV data)
        {
            Debug.WriteLine("SimConnect Disconnected");
            monitoredVars.Clear();
            WASMSimVars.Clear();
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

        private void SimConnect_OnGotWASMData(SimConnect sender, SIMCONNECT_RECV_CLIENT_DATA data)
        {
            // Simvars use ID's starting from 10
            if (data.dwRequestID >= 10)
            {
                uint varID = data.dwRequestID - 10;
                if (varID >= WASMSimVars.Count) return;

                var simData = (WASMClientDataValue)(data.dwData[0]);

                DataReceived?.Invoke(this, new SimActivityEventArgs(WASMSimVars[(int)varID], simData.data.ToString("F9")));
            }
            else
            if(data.dwRequestID == (uint)SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_RESPONSE)
            {
                // this is a message from the main channel
                // we expect here only client adding confirmation
                var simData = (WASMResponseString)(data.dwData[0]);

                if (simData.Data == "MF.Clients.Add." + WASM_CLIENT_NAME + ".Finished")
                {
                    Debug.WriteLine("WASM Module Connected");
                    ConnectedWASM = true;

                    // map the custom communication channels
                    (sender).MapClientDataNameToID(WASM_CLIENT_DATA_NAME_SIMVAR, SIMCONNECT_CLIENT_DATA_ID.CLIENT_LVARS);
                    (sender).MapClientDataNameToID(WASM_CLIENT_DATA_NAME_COMMAND, SIMCONNECT_CLIENT_DATA_ID.CLIENT_CMD);
                    (sender).MapClientDataNameToID(WASM_CLIENT_DATA_NAME_RESPONSE, SIMCONNECT_CLIENT_DATA_ID.CLIENT_RESPONSE);

                    (sender).AddToClientDataDefinition(SIMCONNECT_CLIENT_DATA_ID.CLIENT_RESPONSE, 0, WasmModuleClient.MOBIFLIGHT_MESSAGE_SIZE, 0, 0);
                    (sender).RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, WASMResponseString>(SIMCONNECT_CLIENT_DATA_ID.CLIENT_RESPONSE);
                    (sender).RequestClientData(
                        SIMCONNECT_CLIENT_DATA_ID.CLIENT_RESPONSE,
                        SIMCONNECT_CLIENT_DATA_ID.CLIENT_RESPONSE,
                        SIMCONNECT_CLIENT_DATA_ID.CLIENT_RESPONSE,
                        SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET,
                        SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED,
                        0,
                        0,
                        0
                    );

                    WasmModuleClient.SetConfig(_simConnect, "MAX_VARS_PER_FRAME", "30");
                }
                else
                {
                    Debug.WriteLine($"Received WASM MBF RESPONSE: {simData.Data}");
                }
            } else
            if(data.dwRequestID == (uint)SIMCONNECT_CLIENT_DATA_ID.CLIENT_RESPONSE)
            {
                // handling the client-specific messages goes here
                var simData = (WASMResponseString)(data.dwData[0]);
                Debug.WriteLine($"Received WASM CLIENT RESPONSE: {simData.Data}");
            }
        }

        public void AddVarRequest(string simConnectVariable, string simConnectUnit)
        {
            if (_simConnect == null) return;

            // If the variable name starts with '(' - process it with the WASM
            if (simConnectVariable.StartsWith("("))
            {
                if(WASMSimVars.Contains(simConnectVariable))
                {
                    // already registered
                    return;
                }

                int nextID = WASMSimVars.Count + 10;
                WASMSimVars.Add(simConnectVariable);

                _simConnect.AddToClientDataDefinition(
                    (DEFINITION)nextID,
                    (uint)((WASMSimVars.Count - 1) * sizeof(float)),
                    sizeof(float),
                    0,
                    0);

                _simConnect.RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, WASMClientDataValue>((DEFINITION)nextID);

                _simConnect.RequestClientData(
                    SIMCONNECT_CLIENT_DATA_ID.CLIENT_LVARS,
                    (DEFINITION)nextID,
                    (DEFINITION)nextID,
                    SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET,
                    SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED,
                    0,
                    0,
                    0
                );
                WasmModuleClient.SendWasmCmd(_simConnect, "MF.SimVars.Add." + simConnectVariable);
            }
            else
            {
                foreach(SimVar v in monitoredVars)
                {
                    if(v.name.Equals(simConnectVariable))
                    {
                        // already registered
                        return;
                    }
                }

                int nextID = monitoredVars.Count;
                monitoredVars.Add(new SimVar(simConnectVariable, simConnectUnit));

                if (simConnectUnit.Equals(""))
                {
                    // this is a string variable
                    _simConnect.AddToDataDefinition((DEFINITION)nextID, simConnectVariable, simConnectUnit,
                        SIMCONNECT_DATATYPE.STRING256, 0.005f, SimConnect.SIMCONNECT_UNUSED);
                    _simConnect.RegisterDataDefineStruct<Struct1>((DEFINITION)nextID);
                }
                else
                {
                    _simConnect.AddToDataDefinition((DEFINITION)nextID, simConnectVariable, simConnectUnit,
                        SIMCONNECT_DATATYPE.FLOAT64, 0.005f, SimConnect.SIMCONNECT_UNUSED);
                    _simConnect.RegisterDataDefineStruct<double>((DEFINITION)nextID);
                }

                _simConnect?.RequestDataOnSimObject(
                    (DEFINITION)nextID,
                    (DEFINITION)nextID,
                    SimConnect.SIMCONNECT_OBJECT_ID_USER,
                    SIMCONNECT_PERIOD.SECOND,  // refreshing the data once per second
                    SIMCONNECT_DATA_REQUEST_FLAG.CHANGED, 0, 0, 0);
            }
        }

        public void SetVarValue(string simConnectVariable, string value)
        {
            if (simConnectVariable.StartsWith("("))
            {
                if(!ConnectedWASM)
                {
                    Debug.WriteLine($"WASM IS NOT CONNECTED. Can't set a variable {simConnectVariable}");
                    return;
                }
                string simVarCode = $"{value} (>{simConnectVariable.Substring(1)}";
                WASMExecute(simVarCode);
            }
            else
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
                        // The float string representations comes from Arduino - parse the decimal point as a point
                        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double dValue))
                        {
                            _simConnect.SetDataOnSimObject((DEFINITION)simvarID, SimConnect.SIMCONNECT_OBJECT_ID_USER, SIMCONNECT_DATA_SET_FLAG.DEFAULT, dValue);
                        }
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

        public void WASMExecute(string code)        
        {
            Debug.WriteLine($"Executing code: {code}");
            // the "code" is directly fed to the Sim execute_calculator_code
            WasmModuleClient.SendWasmCmd(_simConnect, "MF.SimVars.Set." + code);
            WasmModuleClient.DummyCommand(_simConnect);
        }

        public void RemoveAllRequests()
        {
            if (ConnectedWASM)
            {
                WasmModuleClient.Stop(_simConnect);
            }

            // keep the ATC MODEL listener active
            if (monitoredVars.Count > 1)
            {
                for (int i = 1; i < monitoredVars.Count; i++)
                {
                    _simConnect.ClearDataDefinition((DEFINITION)i);
                }
                monitoredVars.RemoveRange(1, monitoredVars.Count - 1);
            }
            WASMSimVars.Clear();
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
