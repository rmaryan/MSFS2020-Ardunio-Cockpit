/**
 *  The bridge to the Arduino MEGA, connected to the USB/Serial interface.
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
using System.Diagnostics;
using System.IO.Ports;
using System.Threading;

namespace ArduinoConnector
{
    class ArduinoControl
    {
        private bool _continue;
        private static SerialPort _serialPort;
        private Thread _readThread;
        private readonly string CMD_DELIMITER = "\n";

        public event EventHandler MessageRecieved;

        public void Connect(string comPort)
        {
            _readThread = new Thread(Read);

            _serialPort = new SerialPort();

            _serialPort.PortName = comPort;
            _serialPort.BaudRate = 2400;
            _serialPort.Parity = Parity.None;
            _serialPort.DataBits = 8;
            _serialPort.StopBits = StopBits.One;
            _serialPort.Handshake = Handshake.None;
            _serialPort.DtrEnable = true;
            _serialPort.RtsEnable = true;

            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 5000;

            try
            {
                _serialPort.Open();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                throw;
            }

            _continue = true;
            _readThread.Start();
        }

        public void Disconnect()
        {
            SendMessage('R', "");
            Thread.Sleep(100);
            _continue = false;
            _readThread.Join();
            _serialPort.Close();
        }

        public bool Connected()
        {
            if (_serialPort == null)
            {
                return false;
            }
            return _serialPort.IsOpen;
        }

        public void SendMessage(char type, string data)
        {
            //!!!
            Debug.WriteLine("S: "+ type + data);
            _serialPort.Write(type + data + CMD_DELIMITER);
        }

        public void Read()
        {
            while (_continue)
            {
                try
                {
                    string message = _serialPort.ReadTo(CMD_DELIMITER);

                    OnMessageRecieved(new MessageEventArgs(message[0], message.Remove(0, 1)));

                }
                catch (TimeoutException) { }
                catch (ThreadInterruptedException) { }
                catch (InvalidOperationException)
                {
                    // port lost connection
                    _continue = false;
                    break;
                }
            }
        }

        protected virtual void OnMessageRecieved(MessageEventArgs e)
        {
            EventHandler handler = MessageRecieved;
            handler?.Invoke(this, e);
        }

        public class MessageEventArgs : EventArgs
        {
            public MessageEventArgs(char type, string data)
            {
                this.MessageType = type;
                this.Data = data;
            }

            public char MessageType { get; set; }
            public string Data { get; set; }

        }
    }
}
