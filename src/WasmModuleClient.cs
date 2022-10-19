/**
 * The MIT License (MIT) Copyright © 2021 Sebastian Moebius, MobiFlight
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), 
 * to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using Microsoft.FlightSimulator.SimConnect;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace MSFSConnector
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ClientDataString
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public byte[] data;

        public ClientDataString(string strData)
        {
            byte[] txtBytes = Encoding.ASCII.GetBytes(strData);
            var ret = new byte[1024];
            Array.Copy(txtBytes, ret, txtBytes.Length);
            data = ret;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WASMClientDataValue
    {
        public float data;
    }

    public struct WASMResponseString
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
        public String Data;
    }

    public enum SIMCONNECT_CLIENT_DATA_ID
    {
        MOBIFLIGHT_LVARS,
        MOBIFLIGHT_CMD,
        MOBIFLIGHT_RESPONSE,
        CLIENT_LVARS,
        CLIENT_CMD,
        CLIENT_RESPONSE
    }

    static class WasmModuleClient
    {
        private const string WASM_MBF_NAME = "MobiFlight";
        private const string WASM_MBF_DATA_NAME_COMMAND = WASM_MBF_NAME + ".Command";
        private const string WASM_MBF_DATA_NAME_RESPONSE = WASM_MBF_NAME + ".Response";

        public const int MOBIFLIGHT_MESSAGE_SIZE = 1024;


        public static void AddClient(SimConnect simConnect, string clientName)
        {
            simConnect.MapClientDataNameToID(WASM_MBF_DATA_NAME_COMMAND, SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_CMD);
            simConnect.MapClientDataNameToID(WASM_MBF_DATA_NAME_RESPONSE, SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_RESPONSE);

            simConnect.AddToClientDataDefinition(SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_RESPONSE, 0, MOBIFLIGHT_MESSAGE_SIZE, 0, 0);
            simConnect.RegisterStruct<SIMCONNECT_RECV_CLIENT_DATA, WASMResponseString>(SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_RESPONSE);
            simConnect.RequestClientData(
                SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_RESPONSE,
                SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_RESPONSE,
                SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_RESPONSE,
                SIMCONNECT_CLIENT_DATA_PERIOD.ON_SET,
                SIMCONNECT_CLIENT_DATA_REQUEST_FLAG.CHANGED,
                0,
                0,
                0
            );

            // Registering the custom client in the main commands channel
            WasmModuleClient.SendWasmCmd(simConnect, "MF.DummyCmd", SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_CMD);
            WasmModuleClient.SendWasmCmd(simConnect, "MF.Clients.Add."+ clientName, SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_CMD);
        }

        public static void Ping(SimConnect simConnect)
        {
            if (simConnect == null) return;

            SendWasmCmd(simConnect, "MF.Ping");
            DummyCommand(simConnect);
        }

        public static void Stop(SimConnect simConnect)
        {
            if (simConnect == null) return;
               
            SendWasmCmd(simConnect, "MF.SimVars.Clear");
        }

        public static void GetLVarList(SimConnect simConnect)
        {
            if (simConnect == null) return;

            SendWasmCmd(simConnect, "MF.LVars.List");
            DummyCommand(simConnect);
        }

        public static void DummyCommand(SimConnect simConnect)
        {
            if (simConnect == null) return;

            SendWasmCmd(simConnect, "MF.DummyCmd");
        }

        public static void SetConfig(SimConnect simConnect, String ConfigName, String ConfigValue)
        {
            if (simConnect == null) return;

            SendWasmCmd(simConnect, $"MF.Config.{ConfigName}.Set.{ConfigValue}");
            DummyCommand(simConnect);
        }

        public static void SendWasmCmd(SimConnect simConnect, String command, SIMCONNECT_CLIENT_DATA_ID clientID = SIMCONNECT_CLIENT_DATA_ID.CLIENT_CMD)
        {
            // commands are sent to the custom client commands channel unless other channel is specified
            if (simConnect == null) return;

            SIMCONNECT_CLIENT_DATA_ID defID = SIMCONNECT_CLIENT_DATA_ID.CLIENT_RESPONSE;

            if (clientID == SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_CMD)
            {
                defID = SIMCONNECT_CLIENT_DATA_ID.MOBIFLIGHT_RESPONSE;
            }

            simConnect.SetClientData(
                clientID,
               defID,
               SIMCONNECT_CLIENT_DATA_SET_FLAG.DEFAULT, 0,
               new ClientDataString(command)
            );
        }
    }
}
