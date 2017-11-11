using System;

namespace Tektosyne {

    /// <summary>
    /// Provides a pseudo-random number generator.</summary>
    /// <remarks><para>
    /// <b>MersenneTwister</b> is an implementation of the random number generator MT19937,
    /// developed by Takuji Nishimura and Makoto Matsumoto. The various <see cref="Double"/>
    /// algorithms were added by Isaku Wada.
    /// </para><para>
    /// This C# implementation was created by Christoph Nahr, based on the above authors’ revised C
    /// implementation dating from 26 January 2002.
    /// </para><para>
    /// The following copyright statement is reproduced from the original C program, as required by
    /// the copyright conditions. The original C program containing this copyright statement is
    /// available at the <a href="http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html">Mersenne
    /// Twister</a> home page.
    /// </para><para>
    /// <strong>Copyright (C) 1997 - 2002, Makoto Matsumoto and Takuji Nishimura, All rights
    /// reserved.</strong>
    /// </para><para>
    /// Redistribution and use in source and binary forms, with or without modification, are
    /// permitted provided that the following conditions are met:
    /// </para><list type="number"><item>
    /// Redistributions of source code must retain the above copyright notice, this list of
    /// conditions and the following disclaimer.
    /// </item><item>
    /// Redistributions in binary form must reproduce the above copyright notice, this list of
    /// conditions and the following disclaimer in the documentation and/or other materials provided
    /// with the distribution.
    /// </item><item>
    /// The names of its contributors may not be used to endorse or promote products derived from
    /// this software without specific prior written permission.
    /// </item></list><para>
    /// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS
    /// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
    /// MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE
    /// COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
    /// EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
    /// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
    /// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
    /// TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
    /// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.</para></remarks>

    /*
     * All source code comments by the original authors have been removed.
     * Please consult the original C program and other resources available
     * at http://www.math.sci.hiroshima-u.ac.jp/~m-mat/MT/emt.html
     * for details on the MT19937 algorithm.
     */

    [Serializable, CLSCompliant(false)]
    public class MersenneTwister {
        #region MersenneTwister()

        /// <overloads>
        /// Initializes a new instance of the <see cref="MersenneTwister"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="MersenneTwister"/> class, using a
        /// time-dependent default seed value.</summary>
        /// <remarks>
        /// The new instance of the <see cref="MersenneTwister"/> class is initialized with the
        /// current value of the <see cref="Environment.TickCount"/> property of the <see
        /// cref="Environment"/> class.</remarks>

        public MersenneTwister():
            this(unchecked((uint) Environment.TickCount)) { }

        #endregion
        #region MersenneTwister(UInt32)

        /// <summary>
        /// Initializes a new instance of the <see cref="MersenneTwister"/> class, using the
        /// specified seed value.</summary>
        /// <param name="seed">
        /// A <see cref="UInt32"/> value used to initialize the pseudo-random number sequence.
        /// </param>

        public MersenneTwister(uint seed) {
            unchecked {
                mt[0]= seed;
                for (mti = 1; mti < N; mti++)
                    mt[mti] = (1812433253U * (mt[mti-1] ^ (mt[mti-1] >> 30)) + mti);
            }
        }

        #endregion
        #region MersenneTwister(UInt32[])

        /// <summary>
        /// Initializes a new instance of the <see cref="MersenneTwister"/> class, using the
        /// specified array of initialization keys.</summary>
        /// <param name="keys">
        /// An <see cref="Array"/> of <see cref="UInt32"/> values used to initialize the
        /// pseudo-random number sequence.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="keys"/> is a null reference or an empty array.</exception>

        public MersenneTwister(uint[] keys): this(19650218U) {

            if (keys == null || keys.Length == 0)
                ThrowHelper.ThrowArgumentNullException("keys");

            unchecked {
                uint i = 1, j = 0;
                int k = Math.Max(N, keys.Length);

                for (; k > 0; k--) {
                    mt[i] = (mt[i] ^ ((mt[i-1] ^ (mt[i-1] >> 30)) * 1664525U)) + keys[j] + j;
                    if (++i >= N) { mt[0] = mt[N-1]; i = 1; }
                    if (++j >= keys.Length) j = 0;
                }

                for (k = N-1; k > 0; k--) {
                    mt[i] = (mt[i] ^ ((mt[i-1] ^ (mt[i-1] >> 30)) * 1566083941U)) - i;
                    if (++i >= N) { mt[0] = mt[N-1]; i = 1; }
                }

                mt[0] = 0x80000000U;
            }
        }

        #endregion
        #region Private Fields

        private const int N = 624;
        private const int M = 397;
        private const uint MATRIX_A   = 0x9908b0dfU;
        private const uint UPPER_MASK = 0x80000000U;
        private const uint LOWER_MASK = 0x7fffffffU;

        private uint[] mt = new uint[N];
        private uint mti = N+1;

        #endregion
        #region Default

        /// <summary>
        /// A default-initialized <see cref="MersenneTwister"/>.</summary>
        /// <remarks><para>
        /// <b>Default</b> is a read-only instance of the <see cref="MersenneTwister"/> class that
        /// was created using a time-dependent default seed value.
        /// </para><note type="caution">
        /// The read-only instance returned by <b>Default</b> is <em>not</em> thread-safe because
        /// any call to a <see cref="Next"/> method changes its internal state. Either provide your
        /// own synchronization or create separate instances when different threads need to access a
        /// <see cref="MersenneTwister"/>.</note></remarks>

        public static readonly MersenneTwister Default = new MersenneTwister();

        #endregion
        #region Next()

        /// <overloads>
        /// Returns a random <see cref="Int32"/> value.</overloads>
        /// <summary>
        /// Returns a non-negative random <see cref="Int32"/> value.</summary>
        /// <returns>
        /// A pseudo-random <see cref="Int32"/> value, greater than or equal to zero and less than
        /// or equal to <see cref="Int32.MaxValue"/>.</returns>
        /// <remarks>
        /// The <b>Next</b> methods have the same return type and similar (but closed) ranges as the
        /// <see cref="Random.Next"/> methods of the <see cref="Random"/> class.</remarks>

        public int Next() {
            return (int) (NextUnsigned() >> 1);
        }

        #endregion
        #region Next(Int32)

        /// <summary>
        /// Returns a non-negative random <see cref="Int32"/> value within the specified upper
        /// bound.</summary>
        /// <param name="maxValue">
        /// The upper bound of the random number to be generated.</param>
        /// <returns>
        /// A pseudo-random <see cref="Int32"/> value, greater than or equal to zero and less than
        /// or equal to the specified <paramref name="maxValue"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="maxValue"/> is less than zero.</exception>
        /// <remarks>
        /// The <b>Next</b> methods have the same return type and similar (but closed) ranges as the
        /// <see cref="Random.Next"/> methods of the <see cref="Random"/> class.</remarks>

        public int Next(int maxValue) {

            if (maxValue < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "maxValue", maxValue, Strings.ArgumentNegative);

            return (int) (NextUnsigned() % ((uint) maxValue + 1));
        }

        #endregion
        #region Next(Int32, Int32)

        /// <summary>
        /// Returns a random <see cref="Int32"/> value within the specified lower and upper bounds.
        /// </summary>
        /// <param name="minValue">
        /// The lower bound of the random number to be generated.</param>
        /// <param name="maxValue">
        /// The upper bound of the random number to be generated.</param>
        /// <returns>
        /// A pseudo-random <see cref="Int32"/> value, greater than or equal to <paramref
        /// name="minValue"/> and less than or equal to <paramref name="maxValue"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>
        /// <remarks>
        /// The <b>Next</b> methods have the same return type and similar (but closed) ranges as the
        /// <see cref="Random.Next"/> methods of the <see cref="Random"/> class.</remarks>

        public int Next(int minValue, int maxValue) {

            if (minValue > maxValue)
                ThrowHelper.ThrowArgumentOutOfRangeExceptionWithFormat(
                    "minValue", minValue, Strings.ArgumentGreaterValue, "maxValue");

            return minValue + (int) (NextUnsigned() % ((uint) (maxValue - minValue) + 1));
        }

        #endregion
        #region NextDouble

        /// <summary>
        /// Returns a random <see cref="Double"/> value in the half-open interval [0, 1).</summary>
        /// <returns>
        /// A pseudo-random <see cref="Double"/> value, greater than or equal to zero and less than
        /// one.</returns>
        /// <remarks>
        /// <b>NextDouble</b> has the same return type and the same range as the <see
        /// cref="Random.NextDouble"/> method of the <see cref="Random"/> class.</remarks>

        public double NextDouble() {
            return NextUnsigned() * (1.0 / 4294967296.0);
        }

        #endregion
        #region NextDoubleClosed

        /// <summary>
        /// Returns a random <see cref="Double"/> value in the closed interval [0, 1].</summary>
        /// <returns>
        /// A pseudo-random <see cref="Double"/> value, greater than or equal to zero and less than
        /// or equal to one.</returns>

        public double NextDoubleClosed() {
            return NextUnsigned() * (1.0 / 4294967295.0);
        }

        #endregion
        #region NextDoubleOpen

        /// <summary>
        /// Returns a random <see cref="Double"/> value in the open interval (0, 1).</summary>
        /// <returns>
        /// A pseudo-random <see cref="Double"/> value, greater than zero and less than one.
        /// </returns>

        public double NextDoubleOpen() {
            return (NextUnsigned() + 0.5) * (1.0 / 4294967296.0);
        }

        #endregion
        #region NextDouble53Bits

        /// <summary>
        /// Returns a random <see cref="Double"/> value with a 53-bit resolution in the half-open
        /// interval [0, 1).</summary>
        /// <returns>
        /// A pseudo-random <see cref="Double"/> value with a 53-bit resolution, greater than or
        /// equal to zero and less than one.</returns>
        /// <remarks>
        /// <b>NextDouble53Bits</b> calls <see cref="NextUnsigned"/> twice to compute its return
        /// value, taking 27 bits from the first result and 26 bits from the second result.
        /// </remarks>

        public double NextDouble53Bits() {
            uint a = NextUnsigned() >> 5;
            uint b = NextUnsigned() >> 6;
            return (a * 67108864.0 + b) * (1.0 / 9007199254740992.0);
        }

        #endregion
        #region NextUnsigned

        /// <summary>
        /// Returns a random <see cref="UInt32"/> value.</summary>
        /// <returns>
        /// A pseudo-random <see cref="UInt32"/> value, greater than or equal to zero and less than
        /// or equal to <see cref="UInt32.MaxValue"/>.</returns>
        /// <remarks>
        /// <b>NextUnsigned</b> is the basic random number generation method of the <see
        /// cref="MersenneTwister"/> class. All other RNG methods call <b>NextUnsigned</b> to
        /// compute their return values.</remarks>

        public unsafe uint NextUnsigned() {
            unchecked {
                uint y;

                if (mti >= N) {
                    uint kk;

                    // stack allocation for speed
                    uint* mag01 = stackalloc uint[2];
                    mag01[0] = 0x0U; mag01[1] = MATRIX_A;

                    for (kk = 0; kk < N-M; kk++) {
                        y = (mt[kk] & UPPER_MASK) | (mt[kk+1] & LOWER_MASK);
                        mt[kk] = mt[kk+M] ^ (y >> 1) ^ mag01[y & 0x1U];
                    }

                    for (; kk < N-1; kk++) {
                        y = (mt[kk] & UPPER_MASK) | (mt[kk+1] & LOWER_MASK);
                        mt[kk] = mt[kk + (M-N)] ^ (y >> 1) ^ mag01[y & 0x1U];
                    }

                    y = (mt[N-1] & UPPER_MASK) | (mt[0] & LOWER_MASK);
                    mt[N-1] = mt[M-1] ^ (y >> 1) ^ mag01[y & 0x1U];

                    mti = 0;
                }

                y = mt[mti++];

                y ^= (y >> 11);
                y ^= (y << 7) & 0x9d2c5680U;
                y ^= (y << 15) & 0xefc60000U;
                y ^= (y >> 18);

                return y;
            }
        }

        #endregion
    }
}
