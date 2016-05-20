// ReSharper disable InconsistentNaming
using System;

namespace Dades
{
    /// <summary> Flags to indicate which members contain valid data. </summary>
    /// <remarks>
    ///     The DDS_HEADER_FLAGS_TEXTURE flag, which is defined in Dds.h, is a bitwise-OR combination of the
    ///     <see cref="DDS_FLAGS.DDSD_CAPS" />, <see cref="DDS_FLAGS.DDSD_HEIGHT" />,
    ///     <see cref="DDS_FLAGS.DDSD_WIDTH" />, and <see cref="DDS_FLAGS.DDSD_PIXELFORMAT" /> flags.
    ///     The DDS_HEADER_FLAGS_MIPMAP flag, which is defined in Dds.h, is equal to the
    ///     <see cref="DDS_FLAGS.DDSD_MIPMAPCOUNT" /> flag.
    ///     The DDS_HEADER_FLAGS_VOLUME flag, which is defined in Dds.h, is equal to the
    ///     <see cref="DDS_FLAGS.DDSD_DEPTH" /> flag.
    ///     The DDS_HEADER_FLAGS_PITCH flag, which is defined in Dds.h, is equal to the
    ///     <see cref="DDS_FLAGS.DDSD_PITCH" /> flag.
    ///     The DDS_HEADER_FLAGS_LINEARSIZE flag, which is defined in Dds.h, is equal to the
    ///     <see cref="DDS_FLAGS.DDSD_LINEARSIZE" /> flag.
    /// </remarks>
    [Flags]
    internal enum DDS_FLAGS : uint
    {
        /// <summary> Added for correctness. </summary>
        None = 0,

        /// <summary> Required in every .dds file. </summary>
        DDSD_CAPS = 0x1,

        /// <summary> Required in every .dds file. </summary>
        DDSD_HEIGHT = 0x2,

        /// <summary> Required in every .dds file. </summary>
        DDSD_WIDTH = 0x4,

        /// <summary> Required when pitch is provided for an uncompressed texture. </summary>
        DDSD_PITCH = 0x8,

        /// <summary> Required in every .dds file. </summary>
        DDSD_PIXELFORMAT = 0x1000,

        /// <summary> Required in a mipmapped texture. </summary>
        DDSD_MIPMAPCOUNT = 0x20000,

        /// <summary> Required when pitch is provided for a compressed texture. </summary>
        DDSD_LINEARSIZE = 0x80000,

        /// <summary> Required in a depth texture. </summary>
        DDSD_DEPTH = 0x800000
    }
}