using System;
using System.Diagnostics.Contracts;

namespace Dades
{
    /// <summary> Contains helper methods for dds format manipulation. </summary>
    /// <remarks>
    ///     BC1-3 block flipping code was adapted from http://www.racer.nl/tech/dds.htm. Thank you, guys :)
    /// </remarks>
    public static class DdsTools
    {
        /// <summary>
        ///     Computes the linear size for the specified dimensions using this file's pixel format.
        /// </summary>
        /// <param name="width">The width to calculate linear size for.</param>
        /// <param name="height">The height to calculate linear size for.</param>
        /// <param name="formatD3D">
        ///     D3D resource data format. Can be used instead of <paramref name="formatDxgi" />.
        /// </param>
        /// <param name="formatDxgi">
        ///     DXGI resource data format. Can be used instead of <paramref name="formatD3D" />.
        /// </param>
        /// <param name="defaultPitchOrLinearSize">Value to return for unknown packed formats.</param>
        /// <returns> Calculated linear size. </returns>
        [Pure]
        public static int ComputeLinearSize(int width, int height, D3DFormat formatD3D, DxgiFormat formatDxgi,
                                            int defaultPitchOrLinearSize = 0)
        {
            // Why can't we just use dwPitchOrLinearSize? Because the D3DX library and other similar libraries
            // can supply incorrect value for this field from time to time.

            var bitsPerPixel = GetBitsPerPixel(formatD3D, formatDxgi);
            if (IsBlockCompressedFormat(formatD3D) || IsBlockCompressedFormat(formatDxgi)) {
                return ComputeBCLinearSize(width, height, bitsPerPixel * 2);
            }

            // These packed formats get a special handling...
            if (formatD3D == D3DFormat.R8G8_B8G8 || formatD3D == D3DFormat.G8R8_G8B8 ||
                formatD3D == D3DFormat.UYVY || formatD3D == D3DFormat.YUY2) {
                return ((width + 1) >> 1) * 4 * height;
            }

            // ... but if it is some other packed format, then let's trust dwPitch.
            // In theory it may break for some packed format, but I don't have time to test all those formats:
            if (IsPackedFormat(formatDxgi)) {
                return defaultPitchOrLinearSize * height;   // TODO: Check how some obscure packed formats work with it.
            }

            // If we're here, it must be a generic uncompressed format.
            return ComputeUncompressedPitch(width, bitsPerPixel) * height;
        }

        /// <summary>
        ///     Computes the pitch (number of bytes per scan line) for the specified dimension using this
        ///     file's pixel format.
        /// </summary>
        /// <param name="dimension">The dimension to calculate pitch for.</param>
        /// <param name="formatD3D">
        ///     D3D resource data format. Can be used instead of <paramref name="formatDxgi" />.
        /// </param>
        /// <param name="formatDxgi">
        ///     DXGI resource data format. Can be used instead of <paramref name="formatD3D" />.
        /// </param>
        /// <param name="defaultPitchOrLinearSize">Value to return for unknown packed formats.</param>
        /// <returns> Calculated pitch (number of bytes per scan line). </returns>
        [Pure]
        public static int ComputePitch(int dimension, D3DFormat formatD3D, DxgiFormat formatDxgi,
                                       int defaultPitchOrLinearSize = 0)
        {
            Contract.Requires(dimension > 0);

            // Why can't we just use dwPitchOrLinearSize? Because the D3DX library and other similar libraries
            // can supply incorrect value for this field from time to time.

            var bitsPerPixel = GetBitsPerPixel(formatD3D, formatDxgi);
            if (IsBlockCompressedFormat(formatD3D) || IsBlockCompressedFormat(formatDxgi)) {
                return ComputeBCPitch(dimension, bitsPerPixel * 2);
            }

            // These packed formats get a special handling...
            if (formatD3D == D3DFormat.R8G8_B8G8 || formatD3D == D3DFormat.G8R8_G8B8 ||
                formatD3D == D3DFormat.UYVY || formatD3D == D3DFormat.YUY2) {
                return Math.Max(1, ((dimension + 1) >> 1)) * 4;
            }

            // ... but if it is some other packed format, then let's trust dwPitch.
            // In theory it may break for some packed format, but I don't have time to test all those formats:
            if (IsPackedFormat(formatDxgi)) {
                return defaultPitchOrLinearSize;    // TODO: Check how some obscure packed formats work with it.
            }

            // If we're here, it must be a generic uncompressed format.
            return ComputeUncompressedPitch(dimension, bitsPerPixel);
        }

        /// <summary> Computes the linear size for the generic uncompressed format. </summary>
        /// <param name="width">The width to calculate linear size for.</param>
        /// <param name="height">The height to calculate linear size for.</param>
        /// <param name="bitsPerPixel">The amount bits per pixel.</param>
        /// <returns> Calculated linear size. </returns>
        [Pure]
        public static int ComputeUncompressedLinearSize(int width, int height, int bitsPerPixel)
        {
            return ComputeUncompressedPitch(width, bitsPerPixel) * height;
        }

        /// <summary> Gets the amount of bits per pixel for the specified format. </summary>
        /// <param name="format">The format to test.</param>
        /// <exception cref="ArgumentException">Throws when format is unknown.</exception>
        /// <returns> The amount of bits per pixel for the specified format.</returns>
        [Pure]
        public static int GetBitsPerPixel(DxgiFormat format)
        {
            switch (format) {
                case DxgiFormat.R1_UNorm:
                    return 1;

                case DxgiFormat.BC1_Typeless:
                case DxgiFormat.BC1_UNorm:
                case DxgiFormat.BC1_UNorm_SRGB:
                case DxgiFormat.BC4_SNorm:
                case DxgiFormat.BC4_Typeless:
                case DxgiFormat.BC4_UNorm:
                    return 4;

                case DxgiFormat.A8_UNorm:
                case DxgiFormat.R8_SInt:
                case DxgiFormat.R8_SNorm:
                case DxgiFormat.R8_Typeless:
                case DxgiFormat.R8_UInt:
                case DxgiFormat.R8_UNorm:
                case DxgiFormat.P8:
                case DxgiFormat.AI44:
                case DxgiFormat.IA44:
                case DxgiFormat.BC2_Typeless:
                case DxgiFormat.BC2_UNorm:
                case DxgiFormat.BC2_UNorm_SRGB:
                case DxgiFormat.BC3_Typeless:
                case DxgiFormat.BC3_UNorm:
                case DxgiFormat.BC3_UNorm_SRGB:
                case DxgiFormat.BC5_SNorm:
                case DxgiFormat.BC5_Typeless:
                case DxgiFormat.BC5_UNorm:
                case DxgiFormat.BC6H_SF16:
                case DxgiFormat.BC6H_Typeless:
                case DxgiFormat.BC6H_UF16:
                case DxgiFormat.BC7_Typeless:
                case DxgiFormat.BC7_UNorm:
                case DxgiFormat.BC7_UNorm_SRGB:
                    return 8;

                case DxgiFormat.A8P8:
                case DxgiFormat.AYUV:
                case DxgiFormat.Y210:
                case DxgiFormat.Y216:
                case DxgiFormat.Y410:
                case DxgiFormat.Y416:
                case DxgiFormat.YUY2:
                case DxgiFormat.NV11:
                case DxgiFormat.NV12:
                case DxgiFormat.P010:
                case DxgiFormat.P016:
                case DxgiFormat.Opaque_420:
                case DxgiFormat.B4G4R4A4_UNorm:
                case DxgiFormat.B5G5R5A1_UNorm:
                case DxgiFormat.B5G6R5_UNorm:
                case DxgiFormat.D16_UNorm:
                case DxgiFormat.G8R8_G8B8_UNorm:
                case DxgiFormat.R8G8_B8G8_UNorm:
                case DxgiFormat.R8G8_SInt:
                case DxgiFormat.R8G8_SNorm:
                case DxgiFormat.R8G8_Typeless:
                case DxgiFormat.R8G8_UInt:
                case DxgiFormat.R8G8_UNorm:
                case DxgiFormat.R16_Float:
                case DxgiFormat.R16_SInt:
                case DxgiFormat.R16_SNorm:
                case DxgiFormat.R16_Typeless:
                case DxgiFormat.R16_UInt:
                case DxgiFormat.R16_UNorm:
                    return 16;

                case DxgiFormat.R8G8B8A8_SInt:
                case DxgiFormat.R8G8B8A8_SNorm:
                case DxgiFormat.R8G8B8A8_Typeless:
                case DxgiFormat.R8G8B8A8_UInt:
                case DxgiFormat.R8G8B8A8_UNorm:
                case DxgiFormat.R8G8B8A8_UNorm_SRGB:
                case DxgiFormat.B8G8R8A8_Typeless:
                case DxgiFormat.B8G8R8A8_UNorm:
                case DxgiFormat.B8G8R8A8_UNorm_SRGB:
                case DxgiFormat.B8G8R8X8_Typeless:
                case DxgiFormat.B8G8R8X8_UNorm:
                case DxgiFormat.B8G8R8X8_UNorm_SRGB:
                case DxgiFormat.R24_UNorm_X8_Typeless:
                case DxgiFormat.X24_Typeless_G8_UInt:
                case DxgiFormat.R24G8_Typeless:
                case DxgiFormat.D24_UNorm_S8_UInt:
                case DxgiFormat.D32_Float:
                case DxgiFormat.R9G9B9E5_SHAREDEXP:
                case DxgiFormat.R10G10B10A2_Typeless:
                case DxgiFormat.R10G10B10A2_UNorm:
                case DxgiFormat.R10G10B10A2_UInt:
                case DxgiFormat.R11G11B10_Float:
                case DxgiFormat.R16G16_Typeless:
                case DxgiFormat.R16G16_Float:
                case DxgiFormat.R16G16_SInt:
                case DxgiFormat.R16G16_UInt:
                case DxgiFormat.R16G16_SNorm:
                case DxgiFormat.R16G16_UNorm:
                case DxgiFormat.R32_Float:
                case DxgiFormat.R32_SInt:
                case DxgiFormat.R32_Typeless:
                case DxgiFormat.R32_UInt:
                case DxgiFormat.R10G10B10_XR_BIAS_A2_UNorm:
                    return 32;

                case DxgiFormat.R16G16B16A16_Typeless:
                case DxgiFormat.R16G16B16A16_UNorm:
                case DxgiFormat.R16G16B16A16_UInt:
                case DxgiFormat.R16G16B16A16_SInt:
                case DxgiFormat.R16G16B16A16_Float:
                case DxgiFormat.R16G16B16A16_SNorm:
                    return 48;

                case DxgiFormat.R32G32_Typeless:
                case DxgiFormat.R32G32_Float:
                case DxgiFormat.R32G32_SInt:
                case DxgiFormat.R32G32_UInt:
                case DxgiFormat.R32G8X24_Typeless:
                case DxgiFormat.D32_Float_S8X24_UInt:
                case DxgiFormat.R32_Float_X8X24_Typeless:
                case DxgiFormat.X32_Typeless_G8X24_UInt:
                    return 64;

                case DxgiFormat.R32G32B32_Typeless:
                case DxgiFormat.R32G32B32_SInt:
                case DxgiFormat.R32G32B32_Float:
                case DxgiFormat.R32G32B32_UInt:
                    return 96;

                case DxgiFormat.R32G32B32A32_Typeless:
                case DxgiFormat.R32G32B32A32_UInt:
                case DxgiFormat.R32G32B32A32_SInt:
                case DxgiFormat.R32G32B32A32_Float:
                    return 128;

                case DxgiFormat.Unknown:
                    return 0;

                default:
                    throw new ArgumentException($"Can't get bpp for unknown format '{format}'", nameof(format));
            }
        }

        /// <summary> Gets the amount of bits per pixel for the specified format. </summary>
        /// <param name="format">The format to test.</param>
        /// <returns> The amount of bits per pixel for the specified format.</returns>
        /// <exception cref="ArgumentException">Throws when format is unknown.</exception>
        [Pure]
        public static int GetBitsPerPixel(D3DFormat format)
        {
            switch (format) {
                case D3DFormat.A1:
                    return 1;

                case D3DFormat.DXT1:
                case D3DFormat.BC4S:
                case D3DFormat.BC4U:
                case D3DFormat.ATI1:
                    return 4;

                case D3DFormat.R3G3B2:
                case D3DFormat.A8:
                case D3DFormat.P8:
                case D3DFormat.L8:
                case D3DFormat.A4L4:
                case D3DFormat.S8_Lockable:
                case D3DFormat.DXT2:
                case D3DFormat.DXT3:
                case D3DFormat.DXT4:
                case D3DFormat.DXT5:
                case D3DFormat.BC5S:
                case D3DFormat.BC5U:
                case D3DFormat.ATI2:
                    return 8;

                case D3DFormat.R5G6B5:
                case D3DFormat.X1R5G5B5:
                case D3DFormat.A1R5G5B5:
                case D3DFormat.A4R4G4B4:
                case D3DFormat.A8R3G3B2:
                case D3DFormat.X4R4G4B4:
                case D3DFormat.A8P8:
                case D3DFormat.A8L8:
                case D3DFormat.V8U8:
                case D3DFormat.L6V5U5:
                case D3DFormat.UYVY:
                case D3DFormat.YUY2:
                case D3DFormat.R8G8_B8G8:
                case D3DFormat.G8R8_G8B8:
                case D3DFormat.D16:
                case D3DFormat.D16_Lockable:
                case D3DFormat.D15S1:
                case D3DFormat.L16:
                case D3DFormat.Index16:
                case D3DFormat.R16F:
                case D3DFormat.CxV8U8:
                    return 16;

                case D3DFormat.R8G8B8:
                    return 24;

                case D3DFormat.A8R8G8B8:
                case D3DFormat.X8R8G8B8:
                case D3DFormat.A2B10G10R10:
                case D3DFormat.A8B8G8R8:
                case D3DFormat.X8B8G8R8:
                case D3DFormat.G16R16:
                case D3DFormat.A2R10G10B10:
                case D3DFormat.X8L8V8U8:
                case D3DFormat.DQ8W8V8U8:
                case D3DFormat.V16U16:
                case D3DFormat.A2W10V10U10:
                case D3DFormat.D32:
                case D3DFormat.D32_Lockable:
                case D3DFormat.D32F_Lockable:
                case D3DFormat.D24S8:
                case D3DFormat.D24X8:
                case D3DFormat.D24X4S4:
                case D3DFormat.D24FS8:
                case D3DFormat.Index32:
                case D3DFormat.G16R16F:
                case D3DFormat.R32F:
                case D3DFormat.A2B10G10R10_XR_Bias:
                case D3DFormat.Multi2_ARGB8:
                    return 32;

                case D3DFormat.A16B16G16R16:
                case D3DFormat.A16B16G16R16F:
                case D3DFormat.Q16W16V16U16:
                case D3DFormat.G32R32F:
                    return 64;

                case D3DFormat.A32B32G32R32F:
                    return 128;

                case D3DFormat.Unknown:
                case D3DFormat.ForceDword:
                case D3DFormat.VertexData:
                case D3DFormat.BinaryBuffer:
                    return 0;

                default:
                    throw new ArgumentException("Can't get bpp for unknown format '" + format + "'", "format");
            }
        }

        /// <summary>
        ///     Determines whether the specified format is one of the block-compressed
        ///     (also known as DXT) formats.
        /// </summary>
        /// <param name="format">The format to test.</param>
        /// <returns>
        ///     <c>true</c> if the specified format is a block-compressed (also known as DXT) format;
        ///     otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsBlockCompressedFormat(DxgiFormat format)
        {
            switch (format) {
                case DxgiFormat.BC1_Typeless:
                case DxgiFormat.BC1_UNorm:
                case DxgiFormat.BC1_UNorm_SRGB:
                case DxgiFormat.BC2_Typeless:
                case DxgiFormat.BC2_UNorm:
                case DxgiFormat.BC2_UNorm_SRGB:
                case DxgiFormat.BC3_Typeless:
                case DxgiFormat.BC3_UNorm:
                case DxgiFormat.BC3_UNorm_SRGB:
                case DxgiFormat.BC4_Typeless:
                case DxgiFormat.BC4_UNorm:
                case DxgiFormat.BC4_SNorm:
                case DxgiFormat.BC5_Typeless:
                case DxgiFormat.BC5_UNorm:
                case DxgiFormat.BC5_SNorm:
                case DxgiFormat.BC6H_Typeless:
                case DxgiFormat.BC6H_SF16:
                case DxgiFormat.BC6H_UF16:
                case DxgiFormat.BC7_Typeless:
                case DxgiFormat.BC7_UNorm:
                case DxgiFormat.BC7_UNorm_SRGB:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        ///     Determines whether the specified format is one of the block-compressed
        ///     (also known as DXT) formats.
        /// </summary>
        /// <param name="format">The format to test.</param>
        /// <returns>
        ///     <c>true</c> if the specified format is a block-compressed (also known as DXT) format;
        ///     otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public static bool IsBlockCompressedFormat(D3DFormat format)
        {
            switch (format) {
                case D3DFormat.DXT1:
                case D3DFormat.DXT2:
                case D3DFormat.DXT3:
                case D3DFormat.DXT4:
                case D3DFormat.DXT5:
                case D3DFormat.BC4U:
                case D3DFormat.BC4S:
                case D3DFormat.BC5S:
                case D3DFormat.BC5U:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary> Determines whether the specified format is one of the packed formats.
        ///  Note that 'packed' doesn't mean block-compressed. </summary>
        /// <param name="format">The format to test.</param>
        /// <returns> <c>true</c> if the specified format is packed; otherwise, <c>false</c>. </returns>
        [Pure]
        public static bool IsPackedFormat(DxgiFormat format)
        {
            switch (format) {
                case DxgiFormat.YUY2:
                case DxgiFormat.Y210:
                case DxgiFormat.Y216:
                case DxgiFormat.Y410:
                case DxgiFormat.Y416:
                case DxgiFormat.Opaque_420:
                case DxgiFormat.AI44:
                case DxgiFormat.AYUV:
                case DxgiFormat.IA44:
                case DxgiFormat.NV11:
                case DxgiFormat.NV12:
                case DxgiFormat.P010:
                case DxgiFormat.P016:
                case DxgiFormat.R8G8_B8G8_UNorm:
                case DxgiFormat.G8R8_G8B8_UNorm:
                    return true;

                default:
                    return false;
            }
        }

        /// <summary> Maps D3D texture format to a corresponding DXGI format. </summary>
        /// <param name="format">The format to map.</param>
        /// <returns> DXGI format corresponding to the specified D3D format. </returns>
        [Pure]
        public static DxgiFormat D3DToDxgiFormat(D3DFormat format)
        {
            switch (format) {
                case D3DFormat.A8:
                    return DxgiFormat.A8_UNorm;
                case D3DFormat.A8R8G8B8:
                    return DxgiFormat.B8G8R8A8_UNorm;
                case D3DFormat.X8R8G8B8:
                    return DxgiFormat.B8G8R8X8_UNorm;
                case D3DFormat.R5G6B5:
                    return DxgiFormat.B5G6R5_UNorm;
                case D3DFormat.A1R5G5B5:
                    return DxgiFormat.B5G5R5A1_UNorm;
                case D3DFormat.A4R4G4B4:
                    return DxgiFormat.B4G4R4A4_UNorm;
                case D3DFormat.A2B10G10R10:
                    return DxgiFormat.R10G10B10A2_UNorm;
                case D3DFormat.A8B8G8R8:
                    return DxgiFormat.R8G8B8A8_UNorm;
                case D3DFormat.G16R16:
                    return DxgiFormat.R16G16_UNorm;
                case D3DFormat.A16B16G16R16:
                    return DxgiFormat.R16G16B16A16_UNorm;
                case D3DFormat.L8:
                    return DxgiFormat.R8_UNorm;
                case D3DFormat.A8L8:
                    return DxgiFormat.R8G8_UNorm;
                case D3DFormat.V8U8:
                    return DxgiFormat.R8G8_SNorm;
                case D3DFormat.DQ8W8V8U8:
                    return DxgiFormat.R8G8B8A8_SNorm;
                case D3DFormat.V16U16:
                    return DxgiFormat.R16G16_SNorm;
                case D3DFormat.R8G8_B8G8:
                    return DxgiFormat.G8R8_G8B8_UNorm;
                case D3DFormat.G8R8_G8B8:
                    return DxgiFormat.R8G8_B8G8_UNorm;
                case D3DFormat.D16:
                case D3DFormat.D16_Lockable:
                    return DxgiFormat.D16_UNorm;
                case D3DFormat.D32F_Lockable:
                    return DxgiFormat.D32_Float;
                case D3DFormat.D24S8:
                    return DxgiFormat.D24_UNorm_S8_UInt;
                case D3DFormat.L16:
                    return DxgiFormat.R16_UNorm;
                case D3DFormat.Index16:
                    return DxgiFormat.R16_UInt;
                case D3DFormat.Index32:
                    return DxgiFormat.R32_UInt;
                case D3DFormat.Q16W16V16U16:
                    return DxgiFormat.R16G16B16A16_SNorm;
                case D3DFormat.R16F:
                    return DxgiFormat.R16_Float;
                case D3DFormat.G16R16F:
                    return DxgiFormat.R16G16_Float;
                case D3DFormat.A16B16G16R16F:
                    return DxgiFormat.R16G16B16A16_Float;
                case D3DFormat.R32F:
                    return DxgiFormat.R32_Float;
                case D3DFormat.G32R32F:
                    return DxgiFormat.R32G32_Float;
                case D3DFormat.A32B32G32R32F:
                    return DxgiFormat.R32G32B32A32_Float;

                case D3DFormat.DXT1:
                    return DxgiFormat.BC1_UNorm;
                case D3DFormat.DXT2:
                case D3DFormat.DXT3:
                    return DxgiFormat.BC2_UNorm;
                case D3DFormat.DXT4:
                case D3DFormat.DXT5:
                    return DxgiFormat.BC3_UNorm;

                case D3DFormat.BC4S:
                    return DxgiFormat.BC4_SNorm;

                case D3DFormat.BC4U:
                case D3DFormat.ATI1:
                    return DxgiFormat.BC4_UNorm;

                case D3DFormat.BC5S:
                    return DxgiFormat.BC5_SNorm;

                case D3DFormat.BC5U:
                case D3DFormat.ATI2:
                    return DxgiFormat.BC5_UNorm;

                default:
                    return DxgiFormat.Unknown;
            }
        }

        /// <summary> Computes the amount of bytes per block line. </summary>
        /// <param name="dimension">The texture width or height.</param>
        /// <param name="bytesPerBlock">The amount of bytes per compression block.</param>
        /// <returns> Computed pitch. </returns>
        [Pure]
        internal static int ComputeBCPitch(int dimension, int bytesPerBlock)
        {
            return Math.Max(1, (dimension + 3) / 4) * bytesPerBlock;
        }

        /// <summary> Computes the amount of bytes needed to hold a surface with the specified dimensions. </summary>
        /// <param name="width">The target width.</param>
        /// <param name="height">The target height.</param>
        /// <param name="bytesPerBlock">The amount bytes per compression block.</param>
        /// <returns> Computed linear size. </returns>
        [Pure]
        internal static int ComputeBCLinearSize(int width, int height, int bytesPerBlock)
        {
            return ComputeBCPitch(width, bytesPerBlock) * Math.Max(1, (height + 3) / 4);
        }

        /// <summary> Computes the pitch for the generic uncompressed format. </summary>
        /// <param name="dimension">The texture width or height.</param>
        /// <param name="bitsPerPixel">The amount of bits per pixel.</param>
        /// <returns> Computed pitch. </returns>
        [Pure]
        internal static int ComputeUncompressedPitch(int dimension, int bitsPerPixel)
        {
            return Math.Max(1, (dimension * bitsPerPixel + 7) / 8);
        }

        /// <summary> Performs in-place vertical flip of the specified block-compressed surface. </summary>
        /// <remarks> Currently only BC1, BC2 and BC3 formats are supported.</remarks>
        /// <param name="surfaceData">The surface data.</param>
        /// <param name="width">The surface width.</param>
        /// <param name="height">The surface height.</param>
        /// <param name="bytesPerBlock">The bytes per block.</param>
        /// <param name="format">The BC format.</param>
        internal static void FlipBCSurface(byte[] surfaceData, int width, int height, int bytesPerBlock,
                                           DxgiFormat format)
        {
            Contract.Requires(IsBlockCompressedFormat(format));

            var blocksPerRow = Math.Max(1, (width + 3) / 4);
            var blocksPerCol = Math.Max(1, (height + 3) / 4);

            for (var sourceColumn = 0; sourceColumn < blocksPerCol / 2; sourceColumn++) {
                var targetColumn = blocksPerCol - sourceColumn - 1;
                for (var row = 0; row < blocksPerRow; row++) {
                    var source = (sourceColumn * blocksPerRow + row) * bytesPerBlock;
                    var target = (targetColumn * blocksPerRow + row) * bytesPerBlock;

                    FlipAndSwapBCBlocks(surfaceData, source, target, format);
                }
            }
        }

        /// <summary> Does a vertical flip of a BC1 block. </summary>
        /// <remarks>
        ///     A BC1 block layout is:
        ///     [0-1] color0.
        ///     [2-3] color1.
        ///     [4-7] color bitmap, 2 bits per pixel.
        ///     So each of the 4-7 bytes represents one line, flipping a block is just flipping those bytes.
        /// </remarks>
        /// <param name="data">The surface data.</param>
        /// <param name="blockOffset">The block offset.</param>
        private static void FlipBC1Block(byte[] data, int blockOffset)
        {
            var tmp = data[blockOffset + 4];
            data[blockOffset + 4] = data[blockOffset + 7];
            data[blockOffset + 7] = tmp;

            tmp = data[blockOffset + 5];
            data[blockOffset + 5] = data[blockOffset + 6];
            data[blockOffset + 6] = tmp;
        }

        /// <summary> Does a vertical flip of a BC2 block. </summary>
        /// <remarks>
        ///     A BC2 block layout is:
        ///     [0-7]  alpha bitmap, 4 bits per pixel.
        ///     [8-15] a BC1 block.
        ///     We can flip the alpha bits at the byte level (2 bytes per line).
        /// </remarks>
        /// <param name="data">The surface data.</param>
        /// <param name="blockOffset">The block offset.</param>
        private static void FlipBC2Block(byte[] data, int blockOffset)
        {
            var tmp = data[blockOffset + 0];
            data[blockOffset + 0] = data[blockOffset + 6];
            data[blockOffset + 6] = tmp;

            tmp = data[blockOffset + 1];
            data[blockOffset + 1] = data[blockOffset + 7];
            data[blockOffset + 7] = tmp;

            tmp = data[blockOffset + 2];
            data[blockOffset + 2] = data[blockOffset + 4];
            data[blockOffset + 4] = tmp;

            tmp = data[blockOffset + 3];
            data[blockOffset + 3] = data[blockOffset + 5];
            data[blockOffset + 5] = tmp;

            FlipBC1Block(data, blockOffset + 8);
        }

        /// <summary> Does a vertical flip of a full BC3 block. </summary>
        /// <remarks>
        ///     A BC3 block layout is:
        ///     [0]    alpha0.
        ///     [1]    alpha1.
        ///     [2-7]  alpha bitmap, 3 bits per pixel.
        ///     [8-15] a BC1 block.
        ///     The alpha bitmap doesn't easily map lines to bytes, so we have to
        ///     interpret it correctly.  Extracted from
        ///     http://www.opengl.org/registry/specs/EXT/texture_compression_s3tc.txt :
        ///     The 6 "bits" bytes of the block are decoded into one 48-bit integer:
        ///     bits = bits_0 + 256 * (bits_1 + 256 * (bits_2 + 256 * (bits_3 +
        ///     256 * (bits_4 + 256 * bits_5))))
        ///     bits is a 48-bit unsigned integer, from which a three-bit control code
        ///     is extracted for a texel at location (x,y) in the block using:
        ///     code(x,y) = bits[3*(4*y+x)+1..3*(4*y+x)+0]
        ///     where bit 47 is the most significant and bit 0 is the least
        ///     significant bit.
        /// </remarks>
        /// <param name="data">The surface data.</param>
        /// <param name="blockOffset">The block offset.</param>
        private static void FlipBC3Block(byte[] data, int blockOffset)
        {
            FlipBC4Block(data, blockOffset);
            FlipBC1Block(data, blockOffset + 8);
        }

        /// <summary> Does a vertical flip of a BC4 block. </summary>
        /// <remarks> BC4 is basically BC3, but stores a grayscale image with a single color channel. </remarks>
        /// <param name="data">The surface data.</param>
        /// <param name="blockOffset">The block offset.</param>
        private static void FlipBC4Block(byte[] data, int blockOffset)
        {
            var line01 = data[blockOffset + 2] + 256 * (data[blockOffset + 3] + 256 * data[blockOffset + 4]);
            var line23 = data[blockOffset + 5] + 256 * (data[blockOffset + 6] + 256 * data[blockOffset + 7]);
            // swap lines 0 and 1 in line_0_1.
            var line10 = ((line01 & 0x000fff) << 12) | ((line01 & 0xfff000) >> 12);
            // swap lines 2 and 3 in line_2_3.
            var line32 = ((line23 & 0x000fff) << 12) | ((line23 & 0xfff000) >> 12);
            data[blockOffset + 2] = (byte) (line32 & 0x000000ff);
            data[blockOffset + 3] = (byte) ((line32 & 0x0000ff00) >> 8);
            data[blockOffset + 4] = (byte) ((line32 & 0x00ff0000) >> 16);
            data[blockOffset + 5] = (byte) (line10 & 0x000000ff);
            data[blockOffset + 6] = (byte) ((line10 & 0x0000ff00) >> 8);
            data[blockOffset + 7] = (byte) ((line10 & 0x00ff0000) >> 16);
        }

        /// <summary> Does a vertical flip of a BC5 block. </summary>
        /// <remarks> BC5 is basically two BC4 blocks, but stores a grayscale image with a single color channel. </remarks>
        /// <param name="data">The surface data.</param>
        /// <param name="blockOffset">The block offset.</param>
        private static void FlipBC5Block(byte[] data, int blockOffset)
        {
            FlipBC4Block(data, blockOffset);
            FlipBC4Block(data, blockOffset + 8);
        }

        /// <summary> Flips and then swaps two compressed blocks. </summary>
        /// <param name="data">The surface data.</param>
        /// <param name="sourceBlockOffset">The source block offset.</param>
        /// <param name="targetBlockOffset">The target block offset.</param>
        /// <param name="format">The BC format.</param>
        private static void FlipAndSwapBCBlocks(byte[] data, int sourceBlockOffset, int targetBlockOffset,
                                                DxgiFormat format)
        {
            FlipBCBlock(data, sourceBlockOffset, format);
            FlipBCBlock(data, targetBlockOffset, format);
            SwapBCBlocks(data, sourceBlockOffset, targetBlockOffset, format);
        }

        /// <summary> Flips a BC block vertically. </summary>
        /// <param name="data">The data.</param>
        /// <param name="sourceBlockOffset">The source block offset.</param>
        /// <param name="format">The format.</param>
        /// <exception cref="System.NotSupportedException">
        ///     Throws for BC6 and BC7 formats. These are complex, so no support I'm
        ///     afraid.
        /// </exception>
        private static void FlipBCBlock(byte[] data, int sourceBlockOffset, DxgiFormat format)
        {
            switch (format) {
                case DxgiFormat.BC1_UNorm_SRGB:
                case DxgiFormat.BC1_UNorm:
                case DxgiFormat.BC1_Typeless:
                    FlipBC1Block(data, sourceBlockOffset);
                    break;

                case DxgiFormat.BC2_UNorm_SRGB:
                case DxgiFormat.BC2_UNorm:
                case DxgiFormat.BC2_Typeless:
                    FlipBC2Block(data, sourceBlockOffset);
                    break;

                case DxgiFormat.BC3_UNorm_SRGB:
                case DxgiFormat.BC3_UNorm:
                case DxgiFormat.BC3_Typeless:
                    FlipBC3Block(data, sourceBlockOffset);
                    break;

                case DxgiFormat.BC4_SNorm:
                case DxgiFormat.BC4_UNorm:
                case DxgiFormat.BC4_Typeless:
                    FlipBC4Block(data, sourceBlockOffset);
                    break;

                case DxgiFormat.BC5_SNorm:
                case DxgiFormat.BC5_UNorm:
                case DxgiFormat.BC5_Typeless:
                    FlipBC5Block(data, sourceBlockOffset);
                    break;

                default:
                    throw new NotSupportedException($"The '{format}' format is unsupported.");
            }
        }

        /// <summary> Picks correct bpp from two surface formats, prioritizing D3D over DXGI. </summary>
        /// <exception cref="ArgumentException">Both formats were unknown.</exception>
        [Pure]
        internal static int GetBitsPerPixel(D3DFormat formatD3D, DxgiFormat formatDxgi)
        {
            int result;
            try {
                result = GetBitsPerPixel(formatD3D);
            }
            catch (ArgumentException) {
                try {
                    result = GetBitsPerPixel(formatDxgi);
                }
                catch (ArgumentException e) {
                    throw new ArgumentException("Both formats were unknown.", e);
                }
            }

            return result;
        }

        /// <summary> Swaps two DXT blocks. </summary>
        /// <param name="data">The surface data.</param>
        /// <param name="sourceBlockOffset">The source block offset.</param>
        /// <param name="targetBlockOffset">The target block offset.</param>
        /// <param name="format">Block-compressed format.</param>
        private static void SwapBCBlocks(byte[] data, int sourceBlockOffset, int targetBlockOffset, DxgiFormat format)
        {
            int bytesPerBlock;
            switch (format) {
                case DxgiFormat.BC1_UNorm_SRGB:
                case DxgiFormat.BC1_UNorm:
                case DxgiFormat.BC1_Typeless:
                case DxgiFormat.BC4_SNorm:
                case DxgiFormat.BC4_UNorm:
                case DxgiFormat.BC4_Typeless:
                    bytesPerBlock = 8;
                    break;

                default:
                    bytesPerBlock = 16;
                    break;
            }

            for (var i = 0; i < bytesPerBlock; i++) {
                var temp = data[sourceBlockOffset + i];
                data[sourceBlockOffset + i] = data[targetBlockOffset + i];
                data[targetBlockOffset + i] = temp;
            }
        }
    }
}
