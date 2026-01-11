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
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;

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
            _readThread = new Thread(Read)
            {
                IsBackground = true,
                Name = "ArduinoSerialReadThread"
            };

            _serialPort = new SerialPort
            {
                PortName = comPort,
                BaudRate = 2400,
                Parity = Parity.None,
                DataBits = 8,
                StopBits = StopBits.One,
                Handshake = Handshake.None,
                DtrEnable = true,
                RtsEnable = true,
                NewLine = CMD_DELIMITER,
                // Use blocking read; closing the port will abort the blocked read with an IOException
                ReadTimeout = 500000,
                WriteTimeout = 5000
            };

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
            try
            {
                // try to notify the device first (best effort)
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    SendMessage('R', "");
                    Thread.Sleep(100);
                }
            }
            catch { /* ignore send failures */ }

            // request thread to stop and close the port to unblock the reader
            _continue = false;
            try
            {
                if (_serialPort != null)
                {
                    _serialPort.Close(); // will cause blocking Read/ReadLine to throw and exit the read loop
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error closing serial port: {ex}");
            }

            // wait for the read thread to finish
            try
            {
                _readThread?.Join(500);
            }
            catch (ThreadStateException) { }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error joining read thread: {ex}");
            }
        }

        public bool Connected()
        {
            return _serialPort != null && _serialPort.IsOpen;
        }

        public void SendMessage(char type, string data)
        {
            try
            {
                Debug.WriteLine("S: " + type + data);
                _serialPort.Write(type + data + CMD_DELIMITER);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SendMessage failed: {ex}");
            }
        }

        public void Read()
        {
            // wait for Arduino to reset
            Thread.Sleep(1000);
            Debug.WriteLine("Serial Read Thread started");
            var sb = new System.Text.StringBuilder();
            while (_continue)
            {
                try
                {
                    // blocking read until newline; avoids repeated TimeoutExceptions
                    string message = _serialPort.ReadLine();

                    if (!string.IsNullOrEmpty(message))
                    {
                        Debug.WriteLine("R: " + message);
                        OnMessageRecieved(new MessageEventArgs(message[0], message.Remove(0, 1)));
                    }
                }
                catch (TimeoutException)
                {
                    // unlikely to happen - let's just wait a bit an continue
                    Thread.Sleep(1000);
                    continue;
                }
                catch (IOException)
                {
                    // port closed or IO error -> wait a bit and start again
                    Thread.Sleep(1000);
                    continue;
                }
                catch (InvalidOperationException)
                {
                    // port not open -> exit
                    _continue = false;
                    break;
                }
                catch (ThreadInterruptedException)
                {
                    _continue = false;
                    break;
                }
                catch (Exception ex)
                {
                    // unexpected exception - log and break to avoid spinning
                    Debug.WriteLine($"Unexpected serial read exception: {ex}");
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
