namespace VtConnect.Telnet
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    [UriScheme("telnet")]
    public class TelnetConnection : Connection
    {
        public override bool IsConnected
        {
            get { return !Disconnecting && Client != null && Stream != null && Client.Connected;  }
        }

        private TcpClient Client { get; set; }
        private NetworkStream Stream { get; set; }
        private byte [] CarryOver { get; set; }
        private byte [] ReceiveBuffer { get; set; }
        private bool Disconnecting { get; set; }

        private Dictionary<int, bool> ServerWill { get; set; } = new Dictionary<int, bool>();

        public override event PropertyChangedEventHandler PropertyChanged;

        public override bool Connect(Uri destination, NetworkCredentials credentials)
        {
            Destination = destination;

            int port = Destination.IsDefaultPort ? 23 : Destination.Port;

            Client = new TcpClient();

            try
            {
                Client.Connect(destination.Host, port);

                Stream = Client.GetStream();

                ReceiveBuffer = new byte[16384];
                Stream.BeginRead(ReceiveBuffer, 0, ReceiveBuffer.Length, OnDataReceived, null);

                SendInitialCapabilities();

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsConnected"));
            }
            catch
            {
                Stream = null;
                Client = null;
                return false;
            }

            return true;
        }

        public override void SendData(byte[] data)
        {
            Stream.Write(data, 0, data.Length);
            Stream.Flush();
        }

        public override void SetTerminalWindowSize(int columns, int rows, int width, int height)
        {
            Columns = columns;
            Rows = rows;

            if (IsConnected)
            {
                SendAboutWindowSize();
                Stream.Flush();
            }
        }

        public override void Disconnect()
        {
            Disconnecting = true;

            if (Stream != null)
            {
                Stream.Close();
                Stream = null;
            }

            if (Client != null)
            {
                Client.Close();
                Client = null;
            }

            Disconnecting = false;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsConnected"));
        }

        private void OnDataReceived(IAsyncResult ar)
        {
            if (!IsConnected)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsConnected"));
                return;
            }

            var bytesRead = Stream.EndRead(ar);
            var data = CarryOver == null ? ReceiveBuffer.Take(bytesRead).ToArray() : CarryOver.Concat(ReceiveBuffer.Take(bytesRead)).ToArray();
            CarryOver = null;

            var index = 0;
            var startOfRun = 0;
            while(index < data.Length)
            {
                if (data[index] == (byte)ETelnetCommand.IAC)
                {
                    if(startOfRun != index)
                        DataReceived.Invoke(
                            this,
                            new DataReceivedEventArgs
                            {
                                Data = data
                                    .Skip(startOfRun)
                                    .Take(index - startOfRun)
                                    .ToArray()
                            }
                        );

                    if ((index + 1) < data.Length)
                    {
                        switch ((ETelnetCommand)(data[index + 1]))
                        {
                            case ETelnetCommand.Do:      // TELNET Do
                            case ETelnetCommand.Dont:    // TELNET Don't
                            case ETelnetCommand.Will:    // TELNET Will
                            case ETelnetCommand.Wont:    // TELNET Won't
                                if ((index + 2) < data.Length)
                                {
                                    switch(data[index + 1])
                                    {
                                        case (byte)ETelnetCommand.Do:
                                            TelnetDo(data[index + 2]);
                                            break;
                                        case (byte)ETelnetCommand.Dont:
                                            TelnetDont(data[index + 2]);
                                            break;
                                        case (byte)ETelnetCommand.Will:
                                            TelnetWill(data[index + 2]);
                                            break;
                                        case (byte)ETelnetCommand.Wont:
                                            TelnetWont(data[index + 2]);
                                            break;
                                        default:
                                            System.Diagnostics.Debug.WriteLine("All will go wrong now");
                                            break;
                                    }

                                    index += 3;
                                    startOfRun = index;
                                }
                                else
                                {
                                    CarryOver = data.Skip(index).ToArray();
                                    index += 2;
                                    startOfRun = index;
                                }
                                break;

                            case ETelnetCommand.SB:
                                var subcommandRunsTo = index + 2;
                                while((subcommandRunsTo + 1) < data.Length)
                                {
                                    if(
                                        data[subcommandRunsTo] == (byte)ETelnetCommand.IAC && 
                                        data[subcommandRunsTo + 1] == (byte)ETelnetCommand.SE
                                    )
                                    {
                                        ProcessSubcommand(data.Skip(index + 2).Take(subcommandRunsTo - index - 2).ToArray());
                                        index = subcommandRunsTo + 2;
                                        startOfRun = index;
                                        break;
                                    }

                                    subcommandRunsTo++;
                                }

                                if ((subcommandRunsTo + 1) < data.Length)
                                {
                                    index = subcommandRunsTo + 2;
                                    startOfRun = index;
                                }
                                else
                                {
                                    CarryOver = data.Skip(index).ToArray();
                                    index = data.Length;
                                    startOfRun = index;
                                }

                                break;

                            case ETelnetCommand.IAC:
                                DataReceived.Invoke(this, new DataReceivedEventArgs { Data = new byte [] { 0xFF } });
                                index += 2;
                                startOfRun = index;
                                break;

                            default:
                                System.Diagnostics.Debug.WriteLine("Shouldn't be here");
                                startOfRun = index;
                                index += 2;

                                break;
                        }
                    }
                    else
                    {
                        CarryOver = data.Skip(index).ToArray();
                        startOfRun = index;
                        index += 1;
                    }
                }
                else
                    index++;
            }

            if (startOfRun != index)
                DataReceived.Invoke(this, new DataReceivedEventArgs { Data = data.Skip(startOfRun).Take(index - startOfRun).ToArray() });

            Stream.Flush();

            Stream.BeginRead(ReceiveBuffer, 0, ReceiveBuffer.Length, OnDataReceived, null);
        }

        private void ProcessSubcommand(byte[] data)
        {
            if (data.Length < 2)
                throw new Exception("And we will all go down together");

            if (data[1] != (byte)ETelnetCode.Is)
                return;
            
            switch(data[0])
            {
                case (byte)ETelnetCode.BinaryTransmission:
                    throw new NotImplementedException();
                case (byte)ETelnetCode.TerminalType:
                    SendTerminalType();
                    break;
                case (byte)ETelnetCode.TerminalSpeed:
                    SendTerminalSpeed();
                    break;
                case (byte)ETelnetCode.TerminalEnvironmentOption:
                    SendNewEnvironmentOption();
                    break;
                default:
                    throw new Exception("Should never be here");
            }
        }

        private void TelnetDo(int code)
        {
            System.Diagnostics.Debug.WriteLine("Telnet do : " + code.ToString());

            switch(code)
            {
                case (byte)ETelnetCode.NegotiateAboutWindowSize:
                    SendAboutWindowSize();
                    break;

                case (byte)ETelnetCode.TerminalType:
                case (byte)ETelnetCode.TerminalSpeed:
                case (byte)ETelnetCode.TerminalEnvironmentOption:
                case (byte)ETelnetCode.SuppressGoAhead:
                    break;

                default:
                    SendWont(code);
                    break;
            }
        }

        private void SendInitialCapabilities()
        {
            SendWill((byte)ETelnetCode.NegotiateAboutWindowSize);
            SendWill((byte)ETelnetCode.TerminalSpeed);
            SendWill((byte)ETelnetCode.TerminalType);
            SendWill((byte)ETelnetCode.TerminalEnvironmentOption);
            SendDo((byte)ETelnetCode.Echo);
            SendWill((byte)ETelnetCode.SuppressGoAhead);
            SendDo((byte)ETelnetCode.SuppressGoAhead);

            Stream.Flush();
        }

        private void SendDo(int code)
        {
            var buffer = new byte[]
            {
                (byte)ETelnetCommand.IAC,
                (byte)ETelnetCommand.Do,
                (byte)code
            };

            Stream.Write(buffer, 0, buffer.Length);
        }

        private void SendDont(int code)
        {
            var buffer = new byte[]
            {
                (byte)ETelnetCommand.IAC,
                (byte)ETelnetCommand.Dont,
                (byte)code
            };

            Stream.Write(buffer, 0, buffer.Length);
        }

        private void SendWill(int code)
        {
            var buffer = new byte[]
            {
                (byte)ETelnetCommand.IAC,
                (byte)ETelnetCommand.Will,
                (byte)code
            };

            Stream.Write(buffer, 0, buffer.Length);
        }

        private void SendWont(int code)
        {
            var buffer = new byte[]
            {
                (byte)ETelnetCommand.IAC,
                (byte)ETelnetCommand.Wont,
                (byte)code
            };

            Stream.Write(buffer, 0, buffer.Length);
        }

        private void SendSuboption(byte [] buffer)
        {
            var header = new byte [] {
                (byte)ETelnetCommand.IAC,
                (byte)ETelnetCommand.SB
            };

            var suboptionEnd = new byte[]
            {
                (byte)ETelnetCommand.IAC,
                (byte)ETelnetCommand.SE
            };

            var data = header.Concat(buffer).Concat(suboptionEnd).ToArray();

            Stream.Write(data, 0, data.Length);
        }

        private void SendTerminalSpeed()
        {
            var header = new byte[] {
                (byte)ETelnetCode.TerminalSpeed,
                (byte)ETelnetCode.Send
            };

            SendSuboption(header.Concat(Encoding.ASCII.GetBytes("38400,38400")).ToArray());
        }

        private void SendNewEnvironmentOption()
        {
            var data = new byte[] {
                (byte)ETelnetCode.TerminalEnvironmentOption,
                (byte)ETelnetCode.Send
            };

            SendSuboption(data);
        }

        private void SendAboutWindowSize()
        {
            var data = new byte[] {
                (byte)ETelnetCode.NegotiateAboutWindowSize,
                (byte)((Columns >> 8) & 0xFF),
                (byte)(Columns & 0xFF),
                (byte)((Rows >> 8) & 0xFF),
                (byte)(Rows & 0xFF)
            };

            SendSuboption(data);
        }

        private void SendTerminalType()
        {
            var header = new byte[] {
                (byte)ETelnetCode.TerminalType,
                (byte)ETelnetCode.Send
            };
            SendSuboption(header.Concat(Encoding.ASCII.GetBytes("XTERM")).ToArray());
        }

        private void TelnetDont(int code)
        {
            System.Diagnostics.Debug.WriteLine("Telnet dont : " + code.ToString());
        }

        private void TelnetWill(int code)
        {
            System.Diagnostics.Debug.WriteLine("Telnet will : " + code.ToString());

            ServerWill[code] = true;
        }

        private void TelnetWont(int code)
        {
            System.Diagnostics.Debug.WriteLine("Telnet wont : " + code.ToString());

            ServerWill[code] = false;
        }
    }
}
