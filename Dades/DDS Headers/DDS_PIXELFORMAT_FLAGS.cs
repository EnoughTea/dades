// ReSharper disable InconsistentNaming
using System;

namespace Dades
{
    /// <summary> Values which indicate what type of data is in the surface. </summary>
    [Flags]
    internal enum DDS_PIXELFORMAT_FLAGS : uint
    {
        /// <summary> Added for correctness. </summary>
        None = 0,

        /// <summary> Texture contains alpha data; dwRGBAlphaBitMask contains valid data. </summary>
        DDPF_ALPHAPIXELS = 0x1,

        /// <summary>
        ///     Used in some older DDS files for alpha channel only uncompressed data
        ///     (dwRGBBitCount contains the alpha channel bitcount; dwABitMask contains valid data).
        /// </summary>
        DDPF_ALPHA = 0x2,

        /// <summary> Texture contains compressed RGB data; dwFourCC contains valid data. </summary>
        DDPF_FOURCC = 0x4,

        /// <summary>
        ///     Texture contains uncompressed RGB data; dwRGBBitCount and the RGB masks
        ///     (dwRBitMask, dwRBitMask, dwRBitMask) contain valid data.
        /// </summary>
        DDPF_RGB = 0x40,

        /// <summary>
        ///     Texture contains uncompressed RGB and alpha data; dwRGBBitCount and all of the masks
        ///     (dwRBitMask, dwRBitMask, dwRBitMask, dwRGBAlphaBitMask) contain valid data.
        /// </summary>
        DDPF_RGBA = DDPF_RGB | DDPF_ALPHAPIXELS,

        /// <summary>
        ///     Used in some older DDS files for YUV uncompressed data
        ///     (dwRGBBitCount contains the YUV bit count; dwRBitMask contains the Y mask, dwGBitMask contains the U mask,
        ///     dwBBitMask contains the V mask).
        /// </summary>
        DDPF_YUV = 0x200,

        /// <summary>
        ///     Used in some older DDS files for single channel color uncompressed data
        ///     (dwRGBBitCount contains the luminance channel bit count; dwRBitMask contains the channel mask).
        ///     Can be combined with DDPF_ALPHAPIXELS for a two channel DDS file.
        /// </summary>
        DDPF_LUMINANCE = 0x20000
    }
}