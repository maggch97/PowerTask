namespace VtConnect.Serial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [UriScheme("serial")]
    public class SerialConnection : Connection
    {
        private RJCP.IO.Ports.SerialPortStream Stream { get; set; }
        private byte[] ReceiveBuffer { get; set; }

        public override bool IsConnected
        {
            get { return Stream != null && Stream.IsOpen; }
        }

        public override bool Connect(Uri destination, NetworkCredentials credentials)
        {
            var port = destination.Host;

            var names = RJCP.IO.Ports.SerialPortStream.GetPortNames();

            Stream = new RJCP.IO.Ports.SerialPortStream(port, 9600);
            Stream.Open();

            Stream.BeginRead(ReceiveBuffer, 0, ReceiveBuffer.Length, OnDataReceived, null);

            return true;
        }

        private void OnDataReceived(IAsyncResult ar)
        {
            var bytesRead = Stream.EndRead(ar);
            var data = ReceiveBuffer.ToArray();

            DataReceived.Invoke(this, new DataReceivedEventArgs { Data = data });
            Stream.BeginRead(ReceiveBuffer, 0, ReceiveBuffer.Length, OnDataReceived, null);
        }

        public override void Disconnect()
        {
            Stream.Close();
            Stream = null;
        }

        public override void SendData(byte[] data)
        {
            Stream.Write(data, 0, data.Length);
        }

        public override void SetTerminalWindowSize(int columns, int rows, int width, int height)
        {
            //throw new NotImplementedException();
        }
    }
}
