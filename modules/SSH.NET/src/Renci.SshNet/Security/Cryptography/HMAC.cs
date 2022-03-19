using System;
using System.Security.Cryptography;

namespace Renci.SshNet.Security.Cryptography
{
    /// <summary>
    /// Provides HMAC algorithm implementation.
    /// </summary>
    public abstract class HMAC : KeyedHashAlgorithm
    {
        private IHashProvider _hashProvider;
        private byte[] _innerPadding;
        private byte[] _outerPadding;
        private readonly int _hashSize;

        /// <summary>
        /// Holds value indicating whether the inner padding was already written.
        /// </summary>
        private bool _innerPaddingWritten;

        /// <summary>
        /// Gets the size of the block.
        /// </summary>
        /// <value>
        /// The size of the block.
        /// </value>
        protected abstract int BlockSize { get; }

        /// <summary>
        /// Gets the size, in bits, of the computed hash code.
        /// </summary>
        /// <value>
        /// The size, in bits, of the computed hash code.
        /// </value>
        public override int HashSize {
            get { return _hashSize; }
        }

        /// <summary>
        /// Initializes a <see cref="HMAC"/> with the specified hash algorithm.
        /// </summary>
        /// <param name="hashProvider">The hash provider.</param>
        /// <exception cref="ArgumentNullException"><paramref name="hashProvider"/> is <c>null</c>.</exception>
        private HMAC(IHashProvider hashProvider)
        {
            if (hashProvider == null)
                throw new ArgumentNullException("hashProvider");

            _hashProvider = hashProvider;
            _hashSize = _hashProvider.HashSize;
        }

        /// <summary>
        /// Initializes a <see cref="HMAC"/> with the specified hash algorithm, key and size of the computed
        /// hash code.
        /// </summary>
        /// <param name="hashProvider">The hash provider.</param>
        /// <param name="key">The key.</param>
        /// <param name="hashSize">The size, in bits, of the computed hash code.</param>
        /// <exception cref="ArgumentNullException"><paramref name="hashProvider"/> is <c>null</c>.</exception>
        internal HMAC(IHashProvider hashProvider, byte[] key, int hashSize)
            : this(hashProvider, key)
        {
            _hashSize = hashSize;
        }

        /// <summary>
        /// Initializes a <see cref="HMAC"/> with the specified hash algorithm and key.
        /// </summary>
        /// <param name="hashProvider">The hash provider.</param>
        /// <param name="key">The key.</param>
        /// <exception cref="ArgumentNullException"><paramref name="hashProvider"/> is <c>null</c>.</exception>
        internal HMAC(IHashProvider hashProvider, byte[] key)
            : this(hashProvider)
        {
            SetKey(key);
        }

        /// <summary>
        /// Gets or sets the key to use in the hash algorithm.
        /// </summary>
        /// <returns>
        /// The key to use in the hash algorithm.
        /// </returns>
        public override byte[] Key
        {
            get
            {
                return base.Key;
            }
            set
            {
                SetKey(value);
            }
        }

        /// <summary>
        /// Initializes an implementation of the <see cref="T:System.Security.Cryptography.HashAlgorithm" /> class.
        /// </summary>
        public override void Initialize()
        {
            _hashProvider.Reset();
            _innerPaddingWritten = false;
        }

        /// <summary>
        /// Hashes the core.
        /// </summary>
        /// <param name="rgb">The RGB.</param>
        /// <param name="ib">The ib.</param>
        /// <param name="cb">The cb.</param>
        protected override void HashCore(byte[] rgb, int ib, int cb)
        {
            if (!_innerPaddingWritten)
            {
                // write the inner padding
                _hashProvider.TransformBlock(_innerPadding, 0, BlockSize, _innerPadding, 0);

                // ensure we only write inner padding once
                _innerPaddingWritten = true;
            }

            _hashProvider.HashCore(rgb, ib, cb);
        }

        /// <summary>
        /// Finalizes the hash computation after the last data is processed by the cryptographic stream object.
        /// </summary>
        /// <returns>
        /// The computed hash code.
        /// </returns>
        protected override byte[] HashFinal()
        {
            // finalize the original hash
            var hashValue = _hashProvider.ComputeHash(new byte[0]);

            // write the outer padding
            _hashProvider.TransformBlock(_outerPadding, 0, BlockSize, _outerPadding, 0);

            // write the inner hash and finalize the hash
            _hashProvider.TransformFinalBlock(hashValue, 0, hashValue.Length);

            var hash = _hashProvider.Hash;

            return GetTruncatedHash(hash);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged ResourceMessages.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (_hashProvider != null)
            {
                _hashProvider.Dispose();
                _hashProvider = null;
            }
        }

        private byte[] GetTruncatedHash(byte[] hash)
        {
            var hashSizeBytes = HashSize / 8;
            if (hash.Length == hashSizeBytes)
            {
                return hash;
            }

            var truncatedHash = new byte[hashSizeBytes];
            Buffer.BlockCopy(hash, 0, truncatedHash, 0, hashSizeBytes);
            return truncatedHash;
        }

        private void SetKey(byte[] value)
        {
            var shortenedKey = GetShortenedKey(value);

            _innerPadding = new byte[BlockSize];
            _outerPadding = new byte[BlockSize];

            // Compute inner and outer padding.
            for (var i = 0; i < shortenedKey.Length; i++)
            {
                _innerPadding[i] = (byte)(0x36 ^ shortenedKey[i]);
                _outerPadding[i] = (byte)(0x5C ^ shortenedKey[i]);
            }
            for (var i = shortenedKey.Length; i < BlockSize; i++)
            {
                _innerPadding[i] = 0x36;
                _outerPadding[i] = 0x5C;
            }

            // no need to explicitly clone as this is already done in the setter
            base.Key = shortenedKey;
        }

        /// <summary>
        /// Return a key that fits the <see cref="BlockSize"/> of the <see cref="HashAlgorithm"/>.
        /// </summary>
        /// <param name="key">The key to shorten, if necessary.</param>
        /// <returns>
        /// A hash of <paramref name="key"/> if <paramref name="key"/> is longer than the <see cref="BlockSize"/> of the
        /// <see cref="HashAlgorithm"/>; otherwise, <paramref name="key"/>.
        /// </returns>
        private byte[] GetShortenedKey(byte[] key)
        {
            if (key.Length > BlockSize)
            {
                return _hashProvider.ComputeHash(key);
            }

            return key;
        }
    }
}
