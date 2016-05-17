using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Runtime.InteropServices;

namespace Dades
{
    /// <summary> Contains extension methods for <see cref="Stream" /> </summary>
    internal static class StreamExtensions
    {
        /// <summary> Reads explicit or sequential one byte alignment struct from stream. </summary>
        /// <typeparam name="T">Explicit or sequential with one byte alignment struct type.</typeparam>
        /// <param name="self">The stream to read from.</param>
        /// <returns>Resulting struct.</returns>
        /// <exception cref="ArgumentException">
        ///     Structs must have an explicit or tightly packed sequential layout.
        /// </exception>
        public static T ReadStruct<T>(this Stream self) where T : struct
        {
            Contract.Requires(self != null);

            var layout = typeof (T).StructLayoutAttribute;
            if (layout == null || layout.Value != LayoutKind.Explicit &&
                (layout.Value != LayoutKind.Sequential || layout.Pack != 1)) {
                throw new ArgumentException("Structs must have an explicit or tightly packed sequential layout.");
            }

            var size = Marshal.SizeOf(typeof (T));
            var buffer = new byte[size];
            self.Read(buffer, 0, size);

            var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try {
                return (T) Marshal.PtrToStructure(pinnedBuffer.AddrOfPinnedObject(), typeof (T));
            }
            finally {
                pinnedBuffer.Free();
            }
        }
    }
}
