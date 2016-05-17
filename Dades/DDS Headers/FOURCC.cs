// ReSharper disable InconsistentNaming

namespace Dades
{
    /// <summary> Four character codes constants used in DDS files. </summary>
    internal static class FOURCC
    {
        public const uint DDS_MAGICWORD = 'D' | 'D' << 8 | 'S' << 16 | ' ' << 24;
        public const uint DDS_DX10 = 'D' | 'X' << 8 | '1' << 16 | '0' << 24;
        public const uint D3DFMT_UYVY = 'U' | 'Y' << 8 | 'V' << 16 | 'Y' << 24;
        public const uint D3DFMT_R8G8_B8G8 = 'R' | 'G' << 8 | 'B' << 16 | 'G' << 24;
        public const uint D3DFMT_YUY2 = 'Y' | 'U' << 8 | 'Y' << 16 | '2' << 24;
        public const uint D3DFMT_G8R8_G8B8 = 'G' | 'R' << 8 | 'G' << 16 | 'B' << 24;
        public const uint D3DFMT_DXT1 = 'D' | 'X' << 8 | 'T' << 16 | '1' << 24;
        public const uint D3DFMT_DXT2 = 'D' | 'X' << 8 | 'T' << 16 | '2' << 24;
        public const uint D3DFMT_DXT3 = 'D' | 'X' << 8 | 'T' << 16 | '3' << 24;
        public const uint D3DFMT_DXT4 = 'D' | 'X' << 8 | 'T' << 16 | '4' << 24;
        public const uint D3DFMT_DXT5 = 'D' | 'X' << 8 | 'T' << 16 | '5' << 24;
        public const uint D3DFMT_MULTI2_ARGB8 = 'M' | 'E' << 8 | 'T' << 16 | '1' << 24;
        public const uint FMT_BC4U = 'B' | 'C' << 8 | '4' << 16 | 'U' << 24;
        public const uint FMT_BC4S = 'B' | 'C' << 8 | '4' << 16 | 'S' << 24;
        public const uint FMT_BC5U = 'B' | 'C' << 8 | '5' << 16 | 'U' << 24;
        public const uint FMT_BC5S = 'B' | 'C' << 8 | '5' << 16 | 'S' << 24;
        public const uint FMT_ATI1 = 'A' | 'T' << 8 | 'I' << 16 | '1' << 24;
        public const uint FMT_ATI2 = 'A' | 'T' << 8 | 'I' << 16 | '2' << 24;
        public const uint FMT_RGBG = 'R' | 'G' << 8 | 'B' << 16 | 'G' << 24;
        public const uint FMT_GRGB = 'G' | 'R' << 8 | 'G' << 16 | 'B' << 24;
    }
}
