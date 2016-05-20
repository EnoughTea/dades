using System;
using Dades;
using OpenTK.Graphics.OpenGL;

namespace DadesOglExample
{
    /// <summary> Holder for OpenGL pixel formats used in texture creation.</summary>
    internal struct PixelFormatInfo
    {
        /// <summary> Initializes a new instance of the <see cref="PixelFormatInfo" /> struct. </summary>
        /// <param name="format">DXGI surface format.</param>
        /// <exception cref="NotSupportedException">Unsupported pixel format.</exception>
        public PixelFormatInfo(DxgiFormat format)
            : this()
        {
            FillFromDxgiFormat(format);
        }

        /// <summary>
        ///     Gets the internal pixel format. This is the real format of the image as OpenGL stores it.
        /// </summary>
        public PixelInternalFormat InternalFormat { get; set; }

        /// <summary>
        ///     Gets the pixel format. Describes part of the format of the pixel data you are providing to OpenGL.
        /// </summary>
        public PixelFormat Format { get; set; }

        /// <summary> Gets the pixel type. </summary>
        public PixelType Type { get; set; }

        /// <summary> Gets amount of bits per pixel. </summary>
        public int BitsPerPixel { get; set; }

        /// <summary> Build OpenGL pixel format data from DXGI format.</summary>
        /// <param name="format">DXGI format.</param>
        /// <exception cref="System.NotSupportedException">Unsupported pixel format.</exception>
        private void FillFromDxgiFormat(DxgiFormat format)
        {
            BitsPerPixel = DdsTools.GetBitsPerPixel(format);

            switch (format) {
                case DxgiFormat.BC1_UNorm:
                    InternalFormat = PixelInternalFormat.CompressedRgbS3tcDxt1Ext;
                    break;
                case DxgiFormat.BC1_UNorm_SRGB:
                    InternalFormat = PixelInternalFormat.CompressedSrgbS3tcDxt1Ext;
                    break;
                case DxgiFormat.BC2_UNorm:
                    InternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt3Ext;
                    break;
                case DxgiFormat.BC2_UNorm_SRGB:
                    InternalFormat = PixelInternalFormat.CompressedSrgbAlphaS3tcDxt3Ext;
                    break;
                case DxgiFormat.BC3_UNorm:
                    InternalFormat = PixelInternalFormat.CompressedRgbaS3tcDxt5Ext;
                    break;
                case DxgiFormat.BC3_UNorm_SRGB:
                    InternalFormat = PixelInternalFormat.CompressedSrgbAlphaS3tcDxt5Ext;
                    break;
                case DxgiFormat.BC4_UNorm:
                    InternalFormat = PixelInternalFormat.CompressedRedRgtc1;
                    break;
                case DxgiFormat.BC4_SNorm:
                    InternalFormat = PixelInternalFormat.CompressedSignedRedRgtc1;
                    break;
                case DxgiFormat.BC5_UNorm:
                    InternalFormat = PixelInternalFormat.CompressedRgRgtc2;
                    break;
                case DxgiFormat.BC5_SNorm:
                    InternalFormat = PixelInternalFormat.CompressedSignedRgRgtc2;
                    break;

                case DxgiFormat.A8_UNorm:
                    InternalFormat = PixelInternalFormat.Alpha8;
                    Format = PixelFormat.Alpha;
                    Type = PixelType.UnsignedByte;
                    break;

                case DxgiFormat.B5G5R5A1_UNorm:
                    InternalFormat = PixelInternalFormat.Rgb5A1;
                    Format = PixelFormat.Bgra;
                    Type = PixelType.UnsignedShort1555Reversed;
                    break;

                case DxgiFormat.B5G6R5_UNorm:
                    InternalFormat = PixelInternalFormat.Rgb;
                    Format = PixelFormat.Rgb;
                    Type = PixelType.UnsignedShort565;
                    break;

                case DxgiFormat.B4G4R4A4_UNorm:
                    InternalFormat = PixelInternalFormat.Rgba4;
                    Format = PixelFormat.Bgra;
                    Type = PixelType.UnsignedShort4444Reversed;
                    break;

                case DxgiFormat.B8G8R8A8_UNorm:
                    InternalFormat = PixelInternalFormat.Rgba8;
                    Format = PixelFormat.Bgra;
                    Type = PixelType.UnsignedInt8888Reversed;
                    break;

                case DxgiFormat.B8G8R8A8_UNorm_SRGB:
                    InternalFormat = PixelInternalFormat.Rgba8;
                    Format = PixelFormat.Bgra;
                    Type = PixelType.UnsignedInt8888Reversed;
                    break;

                case DxgiFormat.B8G8R8X8_UNorm:
                    InternalFormat = PixelInternalFormat.Rgba8;
                    Format = PixelFormat.Bgra;
                    Type = PixelType.UnsignedInt8888Reversed;
                    break;

                case DxgiFormat.B8G8R8X8_UNorm_SRGB:
                    InternalFormat = PixelInternalFormat.Rgba8;
                    Format = PixelFormat.Bgra;
                    Type = PixelType.UnsignedInt8888Reversed;
                    break;

                case DxgiFormat.R10G10B10A2_UNorm:
                case DxgiFormat.R10G10B10A2_UInt:
                case DxgiFormat.R10G10B10_XR_BIAS_A2_UNorm:
                    InternalFormat = PixelInternalFormat.Rgb10A2;
                    Format = PixelFormat.Rgba;
                    Type = PixelType.UnsignedInt2101010Reversed;
                    break;

                case DxgiFormat.R11G11B10_Float:
                    InternalFormat = PixelInternalFormat.R11fG11fB10f;
                    Format = PixelFormat.Rgb;
                    Type = PixelType.UnsignedInt10F11F11FRev;
                    break;

                case DxgiFormat.R16_UInt:
                case DxgiFormat.R16_UNorm:
                    InternalFormat = PixelInternalFormat.R16;
                    Format = PixelFormat.Red;
                    Type = PixelType.UnsignedShort;
                    break;

                case DxgiFormat.R16_Float:
                    InternalFormat = PixelInternalFormat.R16f;
                    Format = PixelFormat.Red;
                    Type = PixelType.HalfFloat;
                    break;

                case DxgiFormat.R16_SNorm:
                case DxgiFormat.R16_SInt:
                    InternalFormat = PixelInternalFormat.R16;
                    Format = PixelFormat.Red;
                    Type = PixelType.Short;
                    break;

                case DxgiFormat.R16G16_Float:
                    InternalFormat = PixelInternalFormat.Rg16f;
                    Format = PixelFormat.Rg;
                    Type = PixelType.HalfFloat;
                    break;

                case DxgiFormat.R16G16_SNorm:
                case DxgiFormat.R16G16_SInt:
                    InternalFormat = PixelInternalFormat.Rg16;
                    Format = PixelFormat.Rg;
                    Type = PixelType.Short;
                    break;

                case DxgiFormat.R16G16_UNorm:
                case DxgiFormat.R16G16_UInt:
                    InternalFormat = PixelInternalFormat.Rg16;
                    Format = PixelFormat.Rg;
                    Type = PixelType.UnsignedShort;
                    break;

                case DxgiFormat.R16G16B16A16_Float:
                    InternalFormat = PixelInternalFormat.Rgba16f;
                    Format = PixelFormat.Rgba;
                    Type = PixelType.HalfFloat;
                    break;

                case DxgiFormat.R16G16B16A16_UNorm:
                case DxgiFormat.R16G16B16A16_UInt:
                    InternalFormat = PixelInternalFormat.Rgba16;
                    Format = PixelFormat.Rgba;
                    Type = PixelType.UnsignedShort;
                    break;

                case DxgiFormat.R16G16B16A16_SNorm:
                case DxgiFormat.R16G16B16A16_SInt:
                    InternalFormat = PixelInternalFormat.Rgba16;
                    Format = PixelFormat.Rgba;
                    Type = PixelType.Short;
                    break;


                case DxgiFormat.R32_Float:
                    InternalFormat = PixelInternalFormat.R32f;
                    Format = PixelFormat.Red;
                    Type = PixelType.Float;
                    break;

                case DxgiFormat.R32_UInt:
                    InternalFormat = PixelInternalFormat.R32f;
                    Format = PixelFormat.Red;
                    Type = PixelType.UnsignedInt;
                    break;

                case DxgiFormat.R32_SInt:
                    InternalFormat = PixelInternalFormat.R32f;
                    Format = PixelFormat.Red;
                    Type = PixelType.Int;
                    break;

                case DxgiFormat.R32G32_Float:
                    InternalFormat = PixelInternalFormat.Rg32f;
                    Format = PixelFormat.Rg;
                    Type = PixelType.Float;
                    break;

                case DxgiFormat.R32G32_SInt:
                    InternalFormat = PixelInternalFormat.Rg32f;
                    Format = PixelFormat.Rg;
                    Type = PixelType.Int;
                    break;

                case DxgiFormat.R32G32_UInt:
                    InternalFormat = PixelInternalFormat.Rg32f;
                    Format = PixelFormat.Rg;
                    Type = PixelType.UnsignedInt;
                    break;

                case DxgiFormat.R32G32B32_Float:
                    InternalFormat = PixelInternalFormat.Rgb32f;
                    Format = PixelFormat.Rgb;
                    Type = PixelType.Float;
                    break;

                case DxgiFormat.R32G32B32_SInt:
                    InternalFormat = PixelInternalFormat.Rgb32f;
                    Format = PixelFormat.Rgb;
                    Type = PixelType.Int;
                    break;

                case DxgiFormat.R32G32B32_UInt:
                    InternalFormat = PixelInternalFormat.Rgb32f;
                    Format = PixelFormat.Rgb;
                    Type = PixelType.UnsignedInt;
                    break;

                case DxgiFormat.R32G32B32A32_Float:
                    InternalFormat = PixelInternalFormat.Rgba32f;
                    Format = PixelFormat.Rgba;
                    Type = PixelType.Float;
                    break;
                case DxgiFormat.R32G32B32A32_SInt:
                    InternalFormat = PixelInternalFormat.Rgba32f;
                    Format = PixelFormat.Rgba;
                    Type = PixelType.Int;
                    break;

                case DxgiFormat.R32G32B32A32_UInt:
                    InternalFormat = PixelInternalFormat.Rgba32f;
                    Format = PixelFormat.Rgba;
                    Type = PixelType.UnsignedInt;
                    break;

                case DxgiFormat.R8_SNorm:
                case DxgiFormat.R8_SInt:
                    InternalFormat = PixelInternalFormat.R8;
                    Format = PixelFormat.Red;
                    Type = PixelType.Byte;
                    break;

                case DxgiFormat.R8_UNorm:
                case DxgiFormat.R8_UInt:
                    InternalFormat = PixelInternalFormat.R8;
                    Format = PixelFormat.Red;
                    Type = PixelType.UnsignedByte;
                    break;

                case DxgiFormat.G8R8_G8B8_UNorm:
                    InternalFormat = PixelInternalFormat.Rgb8;
                    Format = PixelFormat.Bgra;
                    Type = PixelType.UnsignedShort4444Reversed;
                    break;

                case DxgiFormat.R8G8_B8G8_UNorm:
                    InternalFormat = PixelInternalFormat.Rgb8;
                    Format = PixelFormat.Rgba;
                    Type = PixelType.UnsignedShort4444;
                    break;

                case DxgiFormat.R8G8_SNorm:
                case DxgiFormat.R8G8_SInt:
                    InternalFormat = PixelInternalFormat.Rg8;
                    Format = PixelFormat.Rg;
                    Type = PixelType.Byte;
                    break;

                case DxgiFormat.R8G8_UInt:
                case DxgiFormat.R8G8_UNorm:
                    InternalFormat = PixelInternalFormat.Rg8;
                    Format = PixelFormat.Rg;
                    Type = PixelType.UnsignedByte;
                    break;

                case DxgiFormat.R8G8B8A8_SNorm:
                case DxgiFormat.R8G8B8A8_SInt:
                    InternalFormat = PixelInternalFormat.Rgba8;
                    Format = PixelFormat.Rgba;
                    Type = PixelType.Byte;
                    break;

                case DxgiFormat.R8G8B8A8_UNorm:
                case DxgiFormat.R8G8B8A8_UInt:
                case DxgiFormat.R8G8B8A8_UNorm_SRGB:
                    InternalFormat = PixelInternalFormat.Rgba8;
                    Format = PixelFormat.Rgba;
                    Type = PixelType.UnsignedByte;
                    break;

                // Needs to be decoded like this: decoded.rgb = encoded.rgb * pow(2, encoded.a)
                case DxgiFormat.R9G9B9E5_SHAREDEXP:
                    InternalFormat = PixelInternalFormat.Rgb9E5;
                    Format = PixelFormat.Rgb;
                    Type = PixelType.UnsignedInt5999Rev;
                    break;

                default:
                    throw new NotSupportedException($"DXGI format '{format}' is not supported.");
            }
        }
    }
}
