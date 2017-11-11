using System;
using System.Runtime.InteropServices;

namespace Tektosyne.Win32Api {

    /// <summary>
    /// Contains information about the current state of both physical and virtual memory.</summary>
    /// <remarks>
    /// This type mirrors the <b>MEMORYSTATUS</b> structure defined in the Platform SDK. Please
    /// refer to the Microsoft Platform SDK documentation for details.</remarks>

    [CLSCompliant(false)]
    [StructLayout(LayoutKind.Sequential)]
    public sealed class MemoryStatus {
        #region dwLength

        /// <summary>
        /// The size of the <see cref="MemoryStatus"/> data structure, in bytes.</summary>
        /// <remarks>
        /// You do not need to set this value before calling <see
        /// cref="Kernel.GlobalMemoryStatus"/>; the function sets it.</remarks>

        public uint dwLength;

        #endregion
        #region dwMemoryLoad

        /// <summary>
        /// The approximate percentage of total physical memory that is in use.</summary>

        public uint dwMemoryLoad;

        #endregion
        #region dwTotalPhys

        /// <summary>
        /// The amount of actual physical memory, in bytes.</summary>

        public uint dwTotalPhys;

        #endregion
        #region dwAvailPhys

        /// <summary>
        /// The amount of physical memory currently available, in bytes.</summary>

        public uint dwAvailPhys;

        #endregion
        #region dwTotalPageFile

        /// <summary>
        /// The current size of the committed memory limit, in bytes.</summary>

        public uint dwTotalPageFile;

        #endregion
        #region dwAvailPageFile

        /// <summary>
        /// The maximum amount of memory the current process can commit, in bytes.</summary>

        public uint dwAvailPageFile;

        #endregion
        #region dwTotalVirtual

        /// <summary>
        /// The size of the user-mode portion of the virtual address space of the calling process,
        /// in bytes.</summary>

        public uint dwTotalVirtual;

        #endregion
        #region dwAvailVirtual

        /// <summary>
        /// The amount of unreserved and uncommitted memory currently in the user-mode portion of
        /// the virtual address space of the calling process, in bytes.</summary>

        public uint dwAvailVirtual;

        #endregion
    }
}
