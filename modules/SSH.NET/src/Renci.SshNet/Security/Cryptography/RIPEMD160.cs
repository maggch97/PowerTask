using System.Security.Cryptography;

namespace Renci.SshNet.Security.Cryptography
{
    /// <summary>
    /// Cryptographic hash function based upon the Merkle–Damgård construction.
    /// </summary>
    public sealed class RIPEMD160 : HashAlgorithm
    {
        private IHashProvider _hashProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RIPEMD160"/> class.
        /// </summary>
        public RIPEMD160()
        {
            _hashProvider = new RIPEMD160HashProvider();
        }

        /// <summary>
        /// Gets the size, in bits, of the computed hash code.
        /// </summary>
        /// <returns>
        /// The size, in bits, of the computed hash code.
        /// </returns>
        public override int HashSize
        {
            get
            {
                return _hashProvider.HashSize;
            }
        }

        /// <summary>
        /// Routes data written to the object into the hash algorithm for computing the hash.
        /// </summary>
        /// <param name="array">The input to compute the hash code for.</param>
        /// <param name="ibStart">The offset into the byte array from which to begin using data.</param>
        /// <param name="cbSize">The number of bytes in the byte array to use as data.</param>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            _hashProvider.HashCore(array, ibStart, cbSize);
        }

        /// <summary>
        /// Finalizes the hash computation after the last data is processed by the cryptographic stream object.
        /// </summary>
        /// <returns>
        /// The computed hash code.
        /// </returns>
        protected override byte[] HashFinal()
        {
            return _hashProvider.HashFinal();
        }

        /// <summary>
        /// Initializes an implementation of the <see cref="HashAlgorithm"/> class.
        /// </summary>
        public override void Initialize()
        {
            _hashProvider.Reset();
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="RIPEMD160"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _hashProvider.Dispose();
                _hashProvider = null;
            }
        }
    }
}
