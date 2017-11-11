using System;
using System.Runtime.InteropServices;

namespace Tektosyne.Win32Api {

    /// <summary>
    /// Interfaces the Windows system library "kernel32.dll".</summary>

    [CLSCompliant(false)]
    public static class Kernel {
        #region GetSystemTimeAdjustment

        /// <summary>
        /// Determines the periodic time adjustments that the system applies to its time-of-day
        /// clock at each clock interrupt.</summary>
        /// <param name="lpTimeAdjustment">
        /// The number of 100-nanosecond units added to the time-of-day clock at each periodic time
        /// adjustment.</param>
        /// <param name="lpTimeIncrement">
        /// The interval between periodic time adjustments, in 100-nanosecond units. This interval
        /// is the time period between a system’s clock interrupts.</param>
        /// <param name="lpTimeAdjustmentDisabled">
        /// Indicates whether periodic time adjustment is in effect.</param>
        /// <returns>
        /// <c>true</c> if the function succeeds; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method maps to the <b>GetSystemTimeAdjustment</b> function defined in the Windows
        /// system library "kernel32.dll". Please refer to the Microsoft Platform SDK documentation
        /// for details.</remarks>

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetSystemTimeAdjustment(
            out uint lpTimeAdjustment,
            out uint lpTimeIncrement,
            [MarshalAs(UnmanagedType.Bool)]
            out bool lpTimeAdjustmentDisabled);

        #endregion
        #region GlobalMemoryStatus

        /// <summary>
        /// Obtains information about the system’s current usage of both physical and virtual
        /// memory.</summary>
        /// <param name="lpBuffer">
        /// An unmanaged <see cref="MemoryStatus"/> object to receive information about current
        /// memory availability.</param>
        /// <remarks>
        /// This method maps to the <b>GlobalMemoryStatus</b> function defined in the Windows system
        /// library "kernel32.dll". Please refer to the Microsoft Platform SDK documentation for
        /// details.</remarks>

        [DllImport("kernel32.dll")]
        public static extern void GlobalMemoryStatus(
            [Out, MarshalAs(UnmanagedType.LPStruct)]
            MemoryStatus lpBuffer);

        #endregion
        #region GlobalMemoryStatusEx

        /// <summary>
        /// Obtains information about the system’s current usage of both physical and virtual
        /// memory.</summary>
        /// <param name="lpBuffer">
        /// An unmanaged <see cref="MemoryStatusEx"/> object to receive information about current
        /// memory availability.</param>
        /// <returns>
        /// <c>true</c> if the function succeeds; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method maps to the <b>GlobalMemoryStatusEx</b> function defined in the Windows
        /// system library "kernel32.dll". Please refer to the Microsoft Platform SDK documentation
        /// for details.</remarks>

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalMemoryStatusEx(
            [Out, MarshalAs(UnmanagedType.LPStruct)]
            MemoryStatusEx lpBuffer);

        #endregion
        #region MoveMemory

        /// <summary>
        /// Moves a block of memory from one location to another.</summary>
        /// <param name="destination">
        /// The starting address of the destination of the move.</param>
        /// <param name="source">
        /// The starting address of the block of memory to move.</param>
        /// <param name="length">
        /// The size of the block of memory to move, in bytes.</param>
        /// <remarks><para>
        /// This method maps to the <b>RtlMoveMemory</b> (alias <b>MoveMemory</b>) function defined
        /// in the Windows system library "kernel32.dll". Please refer to the Microsoft Platform SDK
        /// documentation for details.
        /// </para><para>
        /// The source and destination blocks may overlap.</para></remarks>

        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
        public static extern void MoveMemory(IntPtr destination, IntPtr source, int length);

        #endregion
        #region QueryPerformanceCounter

        /// <summary>
        /// Retrieves the current value of the high-resolution performance counter.</summary>
        /// <param name="lpPerformanceCount">
        /// The current performance-counter value, in counts.</param>
        /// <returns>
        /// <c>true</c> if the function succeeds; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method maps to the <b>QueryPerformanceCounter</b> function defined in the Windows
        /// system library "kernel32.dll". Please refer to the Microsoft Platform SDK documentation
        /// for details.</remarks>

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryPerformanceCounter(
            out long lpPerformanceCount);

        #endregion
        #region QueryPerformanceFrequency

        /// <summary>
        /// Retrieves the frequency of the high-resolution performance counter, if one exists. The
        /// frequency cannot change while the system is running.</summary>
        /// <param name="lpFrequency">
        /// The current performance-counter frequency, in counts per second. If the installed
        /// hardware does not support a high-resolution performance counter, this parameter can be
        /// zero.</param>
        /// <returns>
        /// <c>true</c> if the installed hardware supports a high-resolution performance counter;
        /// otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// This method maps to the <b>QueryPerformanceFrequency</b> function defined in the Windows
        /// system library "kernel32.dll". Please refer to the Microsoft Platform SDK documentation
        /// for details.</remarks>

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool QueryPerformanceFrequency(
            out long lpFrequency);

        #endregion
        #region SetSystemTimeAdjustment

        /// <summary>
        /// Enables or disables periodic time adjustments to the system’s time-of-day clock.
        /// </summary>
        /// <param name="dwTimeAdjustment">
        /// The number of 100-nanosecond units added to the time-of-day clock at each clock
        /// interrupt if periodic time adjustment is enabled.</param>
        /// <param name="bTimeAdjustmentDisabled">
        /// The time adjustment mode that the system is to use. Periodic system time adjustments can
        /// be disabled or enabled.</param>
        /// <returns>
        /// <c>true</c> if the function succeeds; otherwise, <c>false</c>. One way the function can
        /// fail is if the caller does not possess the SE_SYSTEMTIME_NAME privilege.</returns>
        /// <remarks>
        /// This method maps to the <b>SetSystemTimeAdjustment</b> function defined in the Windows
        /// system library "kernel32.dll". Please refer to the Microsoft Platform SDK documentation
        /// for details.</remarks>

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetSystemTimeAdjustment(
            uint dwTimeAdjustment,
            [MarshalAs(UnmanagedType.Bool)]
            bool bTimeAdjustmentDisabled);

        #endregion
    }
}
