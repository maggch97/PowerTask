namespace Renci.SshNet.Security.Cryptography
{
    /// <summary>
    /// Computes a Hash-based Message Authentication Code (HMAC) by using the <see cref="RIPEMD160"/> hash function.
    /// </summary>
    public class HMACRIPEMD160 : HMAC
    {
        /// <summary>
        /// Initializes a <see cref="HMACRIPEMD160"/> with the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        public HMACRIPEMD160(byte[] key)
            : base(new RIPEMD160HashProvider(), key)
        {
        }

        /// <summary>
        /// Gets or sets the block size, in bytes, to use in the hash value.
        /// </summary>
        /// <value>
        /// The block size to use in the hash value. For <see cref="HMACRIPEMD160"/> this is 64 bytes.
        /// </value>
        protected override int BlockSize
        {
            get { return 64; }
        }
    }
}
