// ReSharper disable InconsistentNaming

using System.Runtime.InteropServices;

namespace Dades
{
    /// <summary> Surface pixel format. </summary>
    /// <remarks>
    ///     To store DXGI formats such as floating-point data, use a <see cref="dwFlags" /> of
    ///     <see cref="DDS_PIXELFORMAT_FLAGS.DDPF_FOURCC" /> and set <see cref="dwFourCC" /> to
    ///     'D','X','1','0'. Use the <see cref="DDS_HEADER_DXT10" /> extension header to store the DXGI format in the
    ///     <see cref="DDS_HEADER_DXT10.dxgiFormat" /> member.
    ///     Note that there are non-standard variants of DDS files where <see cref="dwFlags" /> has
    ///     <see cref="DDS_PIXELFORMAT_FLAGS.DDPF_FOURCC" /> and the <see cref="dwFourCC" /> value
    ///     is set directly to a D3DFORMAT or <see cref="DxgiFormat" /> enumeration value. It is not possible to
    ///     disambiguate the D3DFORMAT versus <see cref="DxgiFormat" /> values using this non-standard scheme, so the
    ///     DX10 extension header is recommended instead.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Size = 32, Pack = 1)]
    internal struct DDS_PIXELFORMAT
    {
        /// <summary> Structure size; set to 32 (bytes). </summary>
        public uint dwSize;

        /// <summary> Values which indicate what type of data is in the surface. </summary>
        public DDS_PIXELFORMAT_FLAGS dwFlags;

        /// <summary>
        ///     Four-character codes for specifying compressed or custom formats.
        ///     Possible values include: DXT1, DXT2, DXT3, DXT4, or DXT5. A FOURCC of DX10 indicates the prescense of
        ///     the <see cref="DDS_HEADER_DXT10" /> extended header, and the
        ///     <see cref="DDS_HEADER_DXT10.dxgiFormat" /> member of that structure indicates the
        ///     true format. When using a four-character code, <see cref="dwFlags" /> must include
        ///     <see cref="DDS_PIXELFORMAT_FLAGS.DDPF_FOURCC" />.
        /// </summary>
        public uint dwFourCC;

        /// <summary>
        ///     Number of bits in an RGB (possibly including alpha) format. Valid when <see cref="dwFlags" />
        ///     includes <see cref="DDS_PIXELFORMAT_FLAGS.DDPF_RGB" />, or <see cref="DDS_PIXELFORMAT_FLAGS.DDPF_LUMINANCE" />,
        ///     or <see cref="DDS_PIXELFORMAT_FLAGS.DDPF_YUV" />.
        /// </summary>
        public uint dwRGBBitCount;

        /// <summary>
        ///     Red (or lumiannce or Y) mask for reading color data. For instance, given the A8R8G8B8 format,
        ///     the red mask would be 0x00ff0000.
        /// </summary>
        public uint dwRBitMask;

        /// <summary>
        ///     Green (or U) mask for reading color data. For instance, given the A8R8G8B8 format,
        ///     the green mask would be 0x0000ff00.
        /// </summary>
        public uint dwGBitMask;

        /// <summary>
        ///     Blue (or V) mask for reading color data. For instance, given the A8R8G8B8 format,
        ///     the blue mask would be 0x000000ff.
        /// </summary>
        public uint dwBBitMask;

        /// <summary>
        ///     Alpha mask for reading alpha data. dwFlags must include
        ///     <see cref="DDS_PIXELFORMAT_FLAGS.DDPF_ALPHAPIXELS" /> or <see cref="DDS_PIXELFORMAT_FLAGS.DDPF_ALPHA" />.
        ///     For instance, given the A8R8G8B8 format, the alpha mask would be 0xff000000.
        /// </summary>
        public uint dwABitMask;

        /// <summary> Gets the <see cref="D3DFormat" /> represented by this structure. </summary>
        /// <returns> The <see cref="D3DFormat" /> represented by this structure. </returns>
        public D3DFormat GetD3DFormat()
        {
            if ((dwFlags & DDS_PIXELFORMAT_FLAGS.DDPF_RGBA) == DDS_PIXELFORMAT_FLAGS.DDPF_RGBA) {
                switch (dwRGBBitCount) {
                    case 32:
                        return GetRgba32();
                    case 16:
                        return GetRgba16();
                }
            }
            else if ((dwFlags & DDS_PIXELFORMAT_FLAGS.DDPF_RGB) == DDS_PIXELFORMAT_FLAGS.DDPF_RGB) {
                switch (dwRGBBitCount) {
                    case 32:
                        return GetRgb32();
                    case 24:
                        return GetRgb24();
                    case 16:
                        return GetRgb16();
                }
            }
            else if ((dwFlags & DDS_PIXELFORMAT_FLAGS.DDPF_ALPHA) == DDS_PIXELFORMAT_FLAGS.DDPF_ALPHA) {
                if (dwRGBBitCount == 8) {
                    if (dwABitMask == 0xff) {
                        return D3DFormat.A8;
                    }
                }
            }
            else if ((dwFlags & DDS_PIXELFORMAT_FLAGS.DDPF_LUMINANCE) == DDS_PIXELFORMAT_FLAGS.DDPF_LUMINANCE) {
                switch (dwRGBBitCount) {
                    case 16:
                        return GetLumi16();
                    case 8:
                        return GetLumi8();
                }
            }
            else if ((dwFlags & DDS_PIXELFORMAT_FLAGS.DDPF_FOURCC) == DDS_PIXELFORMAT_FLAGS.DDPF_FOURCC) {
                return (D3DFormat) dwFourCC;
            }

            return D3DFormat.Unknown;
        }

        /// <summary> Returns a <see cref="string" /> that represents this instance. </summary>
        /// <returns> A <see cref="string" /> that represents this instance. </returns>
        public override string ToString()
        {
            return $"DDS_PIXEL_FORMAT: Size = {dwSize}; Flags={dwFlags}; FOURCC={dwFourCC}; " +
                   $"RGBBitCount={dwRGBBitCount}; RBitMask={dwRBitMask}; GBitMask={dwGBitMask}; BBitMask={dwABitMask}";
        }

        private D3DFormat GetRgba32()
        {
            if (dwRBitMask == 0xff && dwGBitMask == 0xff00 && dwBBitMask == 0xff0000 &&
                dwABitMask == 0xff000000) {
                return D3DFormat.A8B8G8R8;
            }

            if (dwRBitMask == 0xffff && dwGBitMask == 0xffff0000) {
                return D3DFormat.G16R16;
            }

            if (dwRBitMask == 0x3ff && dwGBitMask == 0xffc00 && dwBBitMask == 0x3ff00000) {
                return D3DFormat.A2B10G10R10;
            }

            if (dwRBitMask == 0xff0000 && dwGBitMask == 0xff00 && dwBBitMask == 0xff &&
                dwABitMask == 0xff000000) {
                return D3DFormat.A8R8G8B8;
            }

            if (dwRBitMask == 0x3ff00000 && dwGBitMask == 0xffc00 && dwBBitMask == 0x3ff &&
                dwABitMask == 0xc0000000) {
                return D3DFormat.A2R10G10B10;
            }

            return D3DFormat.Unknown;
        }

        private D3DFormat GetRgba16()
        {
            if (dwRBitMask == 0x7c00 && dwGBitMask == 0x3e0 && dwBBitMask == 0x1f &&
                dwABitMask == 0x8000) {
                return D3DFormat.A1R5G5B5;
            }

            if (dwRBitMask == 0xf00 && dwGBitMask == 0xf0 && dwBBitMask == 0xf &&
                dwABitMask == 0xf000) {
                return D3DFormat.A4R4G4B4;
            }

            if (dwRBitMask == 0xe0 && dwGBitMask == 0x1c && dwBBitMask == 0x3 &&
                dwABitMask == 0xff00) {
                return D3DFormat.A8R3G3B2;
            }

            return D3DFormat.Unknown;
        }

        private D3DFormat GetRgb32()
        {
            if (dwRBitMask == 0xffff && dwGBitMask == 0xffff0000) {
                return D3DFormat.G16R16;
            }

            if (dwRBitMask == 0xff0000 && dwGBitMask == 0xff00 && dwBBitMask == 0xff) {
                return D3DFormat.X8R8G8B8;
            }

            if (dwRBitMask == 0xff && dwGBitMask == 0xff00 && dwBBitMask == 0xff0000) {
                return D3DFormat.X8B8G8R8;
            }

            return D3DFormat.Unknown;
        }

        private D3DFormat GetRgb24()
        {
            if (dwRBitMask == 0xff0000 && dwGBitMask == 0xff00 && dwBBitMask == 0xff) {
                return D3DFormat.R8G8B8;
            }

            return D3DFormat.Unknown;
        }

        private D3DFormat GetRgb16()
        {
            if (dwRBitMask == 0xf800 && dwGBitMask == 0x7e0 && dwBBitMask == 0x1f) {
                return D3DFormat.R5G6B5;
            }

            if (dwRBitMask == 0x7c00 && dwGBitMask == 0x3e0 && dwBBitMask == 0x1f) {
                return D3DFormat.X1R5G5B5;
            }

            if (dwRBitMask == 0xf00 && dwGBitMask == 0xf0 && dwBBitMask == 0xf) {
                return D3DFormat.X4R4G4B4;
            }

            return D3DFormat.Unknown;
        }

        private D3DFormat GetLumi16()
        {
            if (dwRBitMask == 0xff && dwABitMask == 0xff00) {
                return D3DFormat.A8L8;
            }

            if (dwRBitMask == 0xffff) {
                return D3DFormat.L16;
            }

            return D3DFormat.Unknown;
        }

        private D3DFormat GetLumi8()
        {
            if (dwRBitMask == 0xf && dwABitMask == 0xf0) {
                return D3DFormat.A4L4;
            }

            if (dwRBitMask == 0xff) {
                return D3DFormat.L8;
            }

            return D3DFormat.Unknown;
        }
    }
}
