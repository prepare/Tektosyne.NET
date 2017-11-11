using System;
using System.Runtime.InteropServices;

namespace Tektosyne.Win32Api {

    /// <summary>
    /// Contains information about the current state of both physical and virtual memory, including
    /// extended memory.</summary>
    /// <remarks>
    /// This type mirrors the <b>MEMORYSTATUSEX</b> structure defined in the Platform SDK. Please
    /// refer to the Microsoft Platform SDK documentation for details.</remarks>

    [CLSCompliant(false)]
    [StructLayout(LayoutKind.Sequential)]
    public sealed class MemoryStatusEx {
        #region dwLength

        /// <summary>
        /// The size of the <see cref="MemoryStatusEx"/> structure, in bytes.</summary>
        /// <remarks>
        /// You must set this member before calling <see cref="Kernel.GlobalMemoryStatusEx"/>.
        /// </remarks>

        public uint dwLength;

        #endregion
        #region dwMemoryLoad

        /// <summary>
        /// The approximate percentage of total physical memory that is in use.</summary>

        public uint dwMemoryLoad;

        #endregion
        #region ulTotalPhys

        /// <summary>
        /// The amount of actual physical memory, in bytes.</summary>

        public ulong ulTotalPhys;

        #endregion
        #region ulAvailPhys

        /// <summary>
        /// The amount of physical memory currently available, in bytes.</summary>

        public ulong ulAvailPhys;

        #endregion
        #region ulTotalPageFile

        /// <summary>
        /// The current committed memory limit for the system or the current process, whichever is
        /// smaller, in bytes.</summary>

        public ulong ulTotalPageFile;

        #endregion
        #region ulAvailPageFile

        /// <summary>
        /// The maximum amount of memory the current process can commit, in bytes.</summary>

        public ulong ulAvailPageFile;

        #endregion
        #region ulTotalVirtual

        /// <summary>
        /// The size of the user-mode portion of the virtual address space of the calling process,
        /// in bytes.</summary>

        public ulong ulTotalVirtual;

        #endregion
        #region ulAvailVirtual

        /// <summary>
        /// The amount of unreserved and uncommitted memory currently in the user-mode portion of
        /// the virtual address space of the calling process, in bytes.</summary>

        public ulong ulAvailVirtual;

        #endregion
        #region ulAvailExtendedVirtual

        /// <summary>Reserved; always zero.</summary>

        public ulong ulAvailExtendedVirtual;

        #endregion
    }
}
