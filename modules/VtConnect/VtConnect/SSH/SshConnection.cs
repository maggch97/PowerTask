namespace VtConnect.SSH
{
    using Renci.SshNet;
    using Renci.SshNet.Common;
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using VtConnect.Exceptions;

    [UriScheme("ssh")]
    public class SshConnection : Connection, IDisposable, IEquatable<SshConnection>
    {
        private AuthenticationMethod AuthenticationMethod { get; set; }
        private ConnectionInfo ConnectionInfo { get; set; }
        private SshClient Client { get; set; }
        private ShellStream ClientStream { get; set; }

        public override bool IsConnected
        {
            get
            {
                return !DisposedValue && ClientStream != null && Client != null && Client.IsConnected;
            }
        }

        public override event PropertyChangedEventHandler PropertyChanged;

        /// <exception cref="SocketException">Socket connection to the SSH server or proxy server could not be established, or an error occurred while resolving the hostname.</exception>
        /// <exception cref="SshConnectionException">SSH session could not be established.</exception>
        /// <exception cref="SshAuthenticationException">Authentication of SSH session failed.</exception>
        /// <exception cref="ProxyException">Failed to establish proxy connection.</exception>
        public override bool Connect(Uri destination, NetworkCredentials credentials)
        {
            Destination = destination;

            if(credentials is UsernamePasswordCredentials)
            {
                var upCredentials = credentials as UsernamePasswordCredentials;
                AuthenticationMethod = new PasswordAuthenticationMethod(upCredentials.Username, upCredentials.Password);
            }
            else
                throw new UnhandledCredentialTypeException("Unhandled credential type " + credentials.GetType().ToString());

            int port = Destination.IsDefaultPort ? 22 : Destination.Port;

            ConnectionInfo = new ConnectionInfo(destination.Host, port, credentials.Username, AuthenticationMethod);
            Client = new SshClient(ConnectionInfo);

            try
            {
                Client.Connect();

                ClientStream = Client.CreateShellStream("xterm", (uint)Columns, (uint)Rows, 800, 600, 16384);
                if (ClientStream == null)
                    throw new VtConnectException("Failed to create client stream");

                ClientStream.DataReceived += ClientStream_DataReceived;
                ClientStream.ErrorOccurred += ClientStream_ErrorOccurred;
                Client.ErrorOccurred += ClientErrorOccurred;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsConnected"));
            }
            catch (Exception e)
            {
                Client.Disconnect();
                Client.Dispose();
                Client = null;

                ConnectionInfo = null;
                AuthenticationMethod = null;

                System.Diagnostics.Debug.WriteLine(e.Message);
                return false;
            }

            return true;
        }

        private void ClientErrorOccurred(object sender, ExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Exception.Message);

            if(e.Exception is Renci.SshNet.Common.SshConnectionException)
            {
                try
                {
                    ClientStream.Close();
                    ClientStream.Dispose();
                }
                finally
                {
                    ClientStream = null;
                }

                try
                {
                    Client.Disconnect();
                    Client.Dispose();
                }
                finally
                {
                    ConnectionInfo = null;
                    AuthenticationMethod = null;
                    Client = null;

                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsConnected"));
                }
            }
            else
                throw new NotImplementedException();
        }

        public override void SendData(byte[] data)
        {
            ClientStream.Write(data, 0, data.Length);
            ClientStream.Flush();
        }

        private void ClientStream_ErrorOccurred(object sender, Renci.SshNet.Common.ExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Exception.Message);

            try
            {
                ClientStream.Close();
                ClientStream.Dispose();
            
                Client.Disconnect();
                Client.Dispose();
            }
            finally
            {
                ConnectionInfo = null;
                AuthenticationMethod = null;
                ClientStream = null;
                Client = null;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsConnected"));
            }
        }

        public override void Disconnect()
        {
            if (!IsConnected)
                return;

            ConnectionInfo = null;
            AuthenticationMethod = null;

            ClientStream.Close();
            ClientStream = null;

            Client.Disconnect();
            Client = null;

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("IsConnected"));
        }

        private void ClientStream_DataReceived(object sender, Renci.SshNet.Common.ShellDataEventArgs e)
        {
            DataReceived.Invoke(
                this,
                new DataReceivedEventArgs
                {
                    Data = e.Data.ToArray()
                }
            );
        }

        private bool DisposedValue { get; set; } // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                if (disposing)
                {
                    try
                    {
                        ClientStream.Close();
                        ClientStream.Dispose();

                        Client.Disconnect();
                        Client.Dispose();
                    }
                    finally
                    {
                        ConnectionInfo = null;
                        AuthenticationMethod = null;
                        ClientStream = null;
                        Client = null;
                    }
                }

                DisposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }

        public bool Equals(SshConnection other)
        {
            return ReferenceEquals(this, other);
        }

        public override void SetTerminalWindowSize(int columns, int rows, int width, int height)
        {
            Columns = columns;
            Rows = rows;

            if(IsConnected)
                ClientStream.SendWindowChangeRequest((uint)columns, (uint)rows, (uint)width, (uint)height);

        }
    }
}
