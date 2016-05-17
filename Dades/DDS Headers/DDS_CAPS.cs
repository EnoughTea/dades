// ReSharper disable InconsistentNaming
using System;

namespace Dades
{
    /// <summary> Specifies the complexity of the surfaces stored. </summary>
    /// <remarks>
    ///     The DDS_SURFACE_FLAGS_MIPMAP flag, which is defined in Dds.h, is a bitwise-OR combination of the
    ///     <see cref="DDSCAPS_COMPLEX" /> and  <see cref="DDSCAPS_MIPMAP" /> flags.
    ///     The DDS_SURFACE_FLAGS_TEXTURE flag, which is defined in Dds.h, is equal to the
    ///     <see cref="DDSCAPS_TEXTURE" /> flag.
    ///     The DDS_SURFACE_FLAGS_CUBEMAP flag, which is defined in Dds.h, is equal to the
    ///     <see cref="DDSCAPS_COMPLEX" /> flag.
    /// </remarks>
    [Flags]
    internal enum DDS_CAPS : uint
    {
        /// <summary> Added for correctness. </summary>
        None = 0,

        /// <summary>
        ///     Optional; must be used on any file that contains more than one surface
        ///     (a mipmap, a cubic environment map, or mipmapped volume texture).
        /// </summary>
        DDSCAPS_COMPLEX = 0x8,

        /// <summary> Optional; should be used for a mipmap. </summary>
        DDSCAPS_MIPMAP = 0x400000,

        /// <summary> Required. </summary>
        DDSCAPS_TEXTURE = 0x1000
    }
}