using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Permissions;

namespace Tektosyne.Win32Api {
    #region Class SafeMemoryHandle

    /// <summary>
    /// Represents a wrapper class for unmanaged memory handles.</summary>
    /// <remarks><para>
    /// <b>SafeMemoryHandle</b> is an abstract base class for <see cref="SafeHandle"/> derivatives
    /// that wrap unmanaged memory handles. All <b>SafeMemoryHandle</b> instances have an invalid
    /// handle value of <see cref="IntPtr.Zero"/> and provide (unsafe!) methods to copy data to and
    /// from arbitrary locations within their unmanaged memory blocks.
    /// </para><para>
    /// The derived classes <see cref="SafeGlobalHandle"/> and <see cref="SafeMapiHandle"/>
    /// implement <b>SafeMemoryHandle</b> for unmanaged memory blocks that are managed by
    /// <see cref="Marshal.AllocHGlobal"/> and by Simple MAPI, respectively.</para></remarks>

    public abstract class SafeMemoryHandle: SafeHandle {
        #region SafeMemoryHandle()

        /// <overloads>
        /// Initializes a new instance of the <see cref="SafeMemoryHandle"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeMemoryHandle"/> class with default
        /// properties.</summary>
        /// <remarks>
        /// The new <b>SafeMemoryHandle</b> instance will release its handle when finalized or
        /// disposed of.</remarks>

        protected SafeMemoryHandle(): base(IntPtr.Zero, true) { }

        #endregion
        #region SafeMemoryHandle(Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeMemoryHandle"/> class with the
        /// specified ownership flag.</summary>
        /// <param name="ownsHandle">
        /// <c>true</c> if the new instance should release its handle when finalized or disposed of;
        /// otherwise, <c>false</c>.</param>

        protected SafeMemoryHandle(bool ownsHandle): base(IntPtr.Zero, ownsHandle) { }

        #endregion
        #region SafeMemoryHandle(IntPtr, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeMemoryHandle"/> class with the
        /// specified existing handle and ownership flag.</summary>
        /// <param name="existingHandle">
        /// An <see cref="IntPtr"/> value indicating the existing handle to use.</param>
        /// <param name="ownsHandle">
        /// <c>true</c> if the new instance should release its handle when finalized or disposed of;
        /// otherwise, <c>false</c>.</param>

        protected SafeMemoryHandle(IntPtr existingHandle, bool ownsHandle):
            base(IntPtr.Zero, ownsHandle) {

            SetHandle(existingHandle);
        }

        #endregion
        #region IsInvalid

        /// <summary>
        /// Gets a value indicating whether the unmanaged memory handle is invalid.</summary>
        /// <value>
        /// <c>true</c> if the unmanaged memory handle was never allocated or has already been
        /// released; otherwise, <c>false</c>.</value>
        /// <remarks>
        /// <b>IsInvalid</b> returns <c>true</c> either if the unmanaged memory handle equals <see
        /// cref="IntPtr.Zero"/>, or if <see cref="SafeHandle.IsClosed"/> is <c>true</c>.</remarks>

        public override sealed bool IsInvalid {
            get { return (handle == IntPtr.Zero || IsClosed); }
        }

        #endregion
        #region GetMemory(Int32, Object)

        /// <overloads>
        /// Copies the unmanaged memory block at the specified offset to a managed <see
        /// cref="Object"/>.</overloads>
        /// <summary>
        /// Copies the unmanaged memory block at the specified offset into the specified managed
        /// <see cref="Object"/>.</summary>
        /// <param name="offset">
        /// The offset from the unmanaged memory handle, in bytes, at which copying begins.</param>
        /// <param name="structure">
        /// The <see cref="Object"/> to receive the data at <paramref name="offset"/>.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="structure"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> is less than zero.</exception>
        /// <exception cref="PropertyValueException">
        /// <see cref="SafeHandle.IsInvalid"/> is <c>true</c>.</exception>
        /// <remarks><para>
        /// <b>GetMemory</b> calls <see cref="Marshal.PtrToStructure"/> to copy the data at the
        /// specified <paramref name="offset"/> within the unmanaged memory block into the specified
        /// <paramref name="structure"/> which must be an instance of a formatted class.
        /// </para><note type="caution">
        /// The specified <paramref name="offset"/> is <b>not</b> checked against the (unknown) size
        /// of the unmanaged memory block. Buffer overruns are possible!</note></remarks>

        public unsafe void GetMemory(int offset, object structure) {
            if (IsInvalid)
                ThrowHelper.ThrowPropertyValueException("IsInvalid", Strings.PropertyTrue);

            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "offset", offset, Strings.ArgumentNegative);

            if (structure == null)
                ThrowHelper.ThrowArgumentNullException("structure");

            Marshal.PtrToStructure((IntPtr) ((byte*) handle + offset), structure);
        }

        #endregion
        #region GetMemory(Int32, Type)

        /// <summary>
        /// Copies the unmanaged memory block at the specified offset to a managed <see
        /// cref="Object"/> of the specified type.</summary>
        /// <param name="offset">
        /// The offset from the unmanaged memory handle, in bytes, at which copying begins.</param>
        /// <param name="type">
        /// The <see cref="Type"/> of the newly allocated <see cref="Object"/> that receives the
        /// data at <paramref name="offset"/>.</param>
        /// <returns>
        /// A new <see cref="Object"/> of the specified <paramref name="type"/> containing the data
        /// at <paramref name="offset"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="type"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> is less than zero.</exception>
        /// <exception cref="PropertyValueException">
        /// <see cref="SafeHandle.IsInvalid"/> is <c>true</c>.</exception>
        /// <remarks><para>
        /// <b>GetMemory</b> calls <see cref="Marshal.PtrToStructure"/> to copy the data at the
        /// specified <paramref name="offset"/> within the unmanaged memory block into a new
        /// instance the specified <paramref name="type"/> which must represent a formatted class.
        /// </para><note type="caution">
        /// The specified <paramref name="offset"/> is <b>not</b> checked against the (unknown) size
        /// of the unmanaged memory block. Buffer overruns are possible!</note></remarks>

        public unsafe object GetMemory(int offset, Type type) {
            if (IsInvalid)
                ThrowHelper.ThrowPropertyValueException("IsInvalid", Strings.PropertyTrue);

            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "offset", offset, Strings.ArgumentNegative);

            if (type == null)
                ThrowHelper.ThrowArgumentNullException("type");

            return Marshal.PtrToStructure((IntPtr) ((byte*) handle + offset), type);
        }

        #endregion
        #region SetMemory

        /// <summary>
        /// Copies the data of the specified managed <see cref="Object"/> into the unmanaged memory
        /// block at the specified offset.</summary>
        /// <param name="structure">
        /// The <see cref="Object"/> containing the data to store at <paramref name="offset"/>.
        /// </param>
        /// <param name="offset">
        /// The offset from the unmanaged memory handle, in bytes, at which copying begins.</param>
        /// <param name="delete">
        /// <c>true</c> to have the <see cref="Marshal.DestroyStructure"/> method called on the
        /// unmanaged memory at <paramref name="offset"/> before copying begins. Note that passing
        /// <c>false</c> can lead to a memory leak.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="structure"/> is a null reference.</exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="offset"/> is less than zero.</exception>
        /// <exception cref="PropertyValueException">
        /// <see cref="SafeHandle.IsInvalid"/> is <c>true</c>.</exception>
        /// <remarks><para>
        /// <b>SetMemory</b> calls <see cref="Marshal.StructureToPtr"/> to copy the data of the
        /// specified <paramref name="structure"/>, which must be an instance of a formatted class,
        /// into the unmanaged memory block at the specified <paramref name="offset"/>.
        /// </para><note type="caution">
        /// The specified <paramref name="offset"/> is <b>not</b> checked against the (unknown) size
        /// of the unmanaged memory block. Buffer overruns are possible!</note></remarks>

        public unsafe void SetMemory(object structure, int offset, bool delete) {
            if (IsInvalid)
                ThrowHelper.ThrowPropertyValueException("IsInvalid", Strings.PropertyTrue);

            if (structure == null)
                ThrowHelper.ThrowArgumentNullException("structure");

            if (offset < 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "offset", offset, Strings.ArgumentNegative);

            Marshal.StructureToPtr(structure, (IntPtr) ((byte*) handle + offset), delete);
        }

        #endregion
    }

    #endregion
    #region Class SafeGlobalHandle

    /// <summary>
    /// Represents a wrapper class for unmanaged memory handles that are released by <see
    /// cref="Marshal.FreeHGlobal"/>.</summary>
    /// <remarks><para>
    /// <b>SafeGlobalHandle</b> provides a <see cref="SafeMemoryHandle"/> whose handle, if valid
    /// and owned, is released using the standard method <see cref="Marshal.FreeHGlobal"/>.
    /// </para><para>
    /// Call <see cref="SafeGlobalHandle.AllocateHandle"/> to allocate an unmanaged memory block
    /// of an arbitrary size, using the standard method <see cref="Marshal.AllocHGlobal"/>.
    /// </para></remarks>

    public sealed class SafeGlobalHandle: SafeMemoryHandle {
        #region SafeGlobalHandle()

        /// <overloads>
        /// Initializes a new instance of the <see cref="SafeGlobalHandle"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeGlobalHandle"/> class with default
        /// properties.</summary>
        /// <remarks>
        /// The new <b>SafeGlobalHandle</b> instance will release its handle when finalized or
        /// disposed of.</remarks>

        public SafeGlobalHandle(): base() { }

        #endregion
        #region SafeGlobalHandle(Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeGlobalHandle"/> class with the
        /// specified ownership flag.</summary>
        /// <param name="ownsHandle">
        /// <c>true</c> if the new instance should release its handle when finalized or disposed of;
        /// otherwise, <c>false</c>.</param>

        public SafeGlobalHandle(bool ownsHandle): base(ownsHandle) { }

        #endregion
        #region SafeGlobalHandle(IntPtr, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeGlobalHandle"/> class with the
        /// specified existing handle and ownership flag.</summary>
        /// <param name="existingHandle">
        /// An <see cref="IntPtr"/> value indicating the existing handle to use.</param>
        /// <param name="ownsHandle">
        /// <c>true</c> if the new instance should release its handle when finalized or disposed of;
        /// otherwise, <c>false</c>.</param>

        public SafeGlobalHandle(IntPtr existingHandle, bool ownsHandle):
            base(existingHandle, ownsHandle) { }

        #endregion
        #region AllocateHandle

        /// <summary>
        /// Allocates an unmanaged memory block of the specified size.</summary>
        /// <param name="byteCount">
        /// The number of bytes to allocate.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="byteCount"/> is equal to or less than zero.</exception>
        /// <exception cref="PropertyValueException">
        /// <see cref="SafeHandle.IsInvalid"/> is <c>false</c>.</exception>
        /// <remarks>
        /// <b>AllocateHandle</b> calls <see cref="Marshal.AllocHGlobal"/> to allocate an unmanaged
        /// memory block whose size equals the specified <paramref name="byteCount"/>. The
        /// allocation and subsequent handle assignment are placed in a constrained execution
        /// region.</remarks>

        public void AllocateHandle(int byteCount) {
            if (!IsInvalid)
                ThrowHelper.ThrowPropertyValueException("IsInvalid", Strings.PropertyFalse);

            if (byteCount <= 0)
                ThrowHelper.ThrowArgumentOutOfRangeException(
                    "byteCount", byteCount, Strings.ArgumentNotPositive);

            RuntimeHelpers.PrepareConstrainedRegions();
            try { }
            finally {
                SetHandle(Marshal.AllocHGlobal(byteCount));
            }
        }

        #endregion
        #region ReleaseHandle

        /// <summary>
        /// Releases the unmanaged memory handle.</summary>
        /// <returns>
        /// <c>true</c> if the unmanaged memory handle was released successfully; otherwise,
        /// <c>false</c>.</returns>
        /// <remarks>
        /// <b>ReleaseHandle</b> calls <see cref="Marshal.FreeHGlobal"/> to release the unmanaged
        /// memory handle, and returns <c>true</c> exactly if no exception occurred. Any exceptions
        /// that occur are never propagated to the caller.</remarks>

        protected override sealed bool ReleaseHandle() {
            try {
                Marshal.FreeHGlobal(handle);
                return true;
            }
            catch {
                return false;
            }
        }

        #endregion
    }

    #endregion
    #region Class SafeMapiHandle

    /// <summary>
    /// Represents a wrapper class for unmanaged memory handles that are released by <see
    /// cref="Mapi.MAPIFreeBuffer"/>.</summary>
    /// <remarks><para>
    /// <b>SafeMapiHandle</b> provides a <see cref="SafeMemoryHandle"/> whose handle, if valid and
    /// owned, is released using the Simple MAPI function <see cref="Mapi.MAPIFreeBuffer"/>.
    /// </para><para>
    /// Simple MAPI buffers are implicitly allocated by several <see cref="Mapi"/> methods. They
    /// cannot be allocated explicitly.</para></remarks>

    public sealed class SafeMapiHandle: SafeMemoryHandle {
        #region SafeMapiHandle()

        /// <overloads>
        /// Initializes a new instance of the <see cref="SafeMapiHandle"/> class.</overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="SafeMapiHandle"/> class with default
        /// properties.</summary>
        /// <remarks>
        /// The new <b>SafeMapiHandle</b> instance will release its handle when finalized or
        /// disposed of.</remarks>

        public SafeMapiHandle(): base() { }

        #endregion
        #region SafeMapiHandle(Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeMapiHandle"/> class with the specified
        /// ownership flag.</summary>
        /// <param name="ownsHandle">
        /// <c>true</c> if the new instance should release its handle when finalized or disposed of;
        /// otherwise, <c>false</c>.</param>

        public SafeMapiHandle(bool ownsHandle): base(ownsHandle) { }

        #endregion
        #region SafeMapiHandle(IntPtr, Boolean)

        /// <summary>
        /// Initializes a new instance of the <see cref="SafeMapiHandle"/> class with the specified
        /// existing handle and ownership flag.</summary>
        /// <param name="existingHandle">
        /// An <see cref="IntPtr"/> value indicating the existing handle to use.</param>
        /// <param name="ownsHandle">
        /// <c>true</c> if the new instance should release its handle when finalized or disposed of;
        /// otherwise, <c>false</c>.</param>

        public SafeMapiHandle(IntPtr existingHandle, bool ownsHandle):
            base(existingHandle, ownsHandle) { }

        #endregion
        #region ReleaseHandle

        /// <summary>
        /// Releases the unmanaged memory handle.</summary>
        /// <returns>
        /// <c>true</c> if the unmanaged memory handle was released successfully; otherwise,
        /// <c>false</c>.</returns>
        /// <remarks>
        /// <b>ReleaseHandle</b> calls <see cref="Mapi.MAPIFreeBuffer"/> to release the unmanaged
        /// memory handle, and returns <c>true</c> exactly if the resulting <see cref="MapiError"/>
        /// code equals <see cref="MapiError.SUCCESS_SUCCESS"/>.</remarks>

        protected override sealed bool ReleaseHandle() {
            MapiError error = Mapi.MAPIFreeBuffer(handle);
            return (error == MapiError.SUCCESS_SUCCESS);
        }

        #endregion
    }

    #endregion
}
