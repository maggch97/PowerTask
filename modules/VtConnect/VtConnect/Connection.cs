namespace VtConnect
{
    using System;
    using System.ComponentModel;
    using System.Threading.Tasks;

    public abstract class Connection : INotifyPropertyChanged
    {
        public Uri Destination { get; set; }

        public int Columns { get; set; } = 80;
        public int Rows { get; set; } = 25;

        public abstract bool IsConnected
        {
            get;
        }

        public EventHandler<DataReceivedEventArgs> DataReceived;

        public EventHandler<SshFingerprintEventArgs> KeyReceived;

        public abstract event PropertyChangedEventHandler PropertyChanged;

        public abstract bool Connect(Uri destination, NetworkCredentials credentials);

        public abstract void Disconnect();

        public static Connection CreateConnection(Uri destination)
        {
            var result = ConnectionFactory.CreateByScheme(destination.Scheme);

            return result;
        }

        public abstract void SendData(byte[] data);

        public abstract void SetTerminalWindowSize(int columns, int rows, int width, int height);
    }
}
