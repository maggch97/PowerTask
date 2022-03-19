namespace VtConnect
{
    using System;

    public class DataReceivedEventArgs : EventArgs, IEquatable<DataReceivedEventArgs>
    {
        public byte [] Data { get; internal set; }

        public bool Equals(DataReceivedEventArgs other)
        {
            return ReferenceEquals(this, other);
        }
    }
}
