namespace VtConnect
{
    using System;

    public class SshFingerprintEventArgs : EventArgs
    {
        public byte[] Fingerprint { get; set; }
        public bool Proceed { get; set; }
    }
}