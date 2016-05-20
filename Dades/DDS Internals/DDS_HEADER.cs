// ReSharper disable InconsistentNaming

using System;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace Dades
{
    /// <summary> Describes a DDS file header. </summary>
    /// <remarks>
    ///     Note  When you write .dds files, you should set the  <see cref="DDS_FLAGS.DDSD_CAPS" /> and
    ///     <see cref="DDS_FLAGS.DDSD_PIXELFORMAT" /> flags, and for mipmapped textures you should also set the
    ///     <see cref="DDS_FLAGS.DDSD_MIPMAPCOUNT" /> flag.
    ///     However, when you read a .dds file, you should not rely on the <see cref="DDS_FLAGS.DDSD_CAPS" />,
    ///     <see cref="DDS_FLAGS.DDSD_PIXELFORMAT" />, and <see cref="DDS_FLAGS.DDSD_MIPMAPCOUNT" />
    ///     flags being set because some writers of such a file might not set these flags.
    ///     Include flags in <see cref="dwFlags" /> for the members of the structure that contain valid data. Use this
    ///     structure in combination with a <see cref="DDS_HEADER_DXT10" /> to store a resource array in a DDS file.
    ///     For more information, see texture arrays.
    ///     <see cref="DDS_HEADER" /> is identical to the DirectDraw DDSURFACEDESC2 structure without DirectDraw
    ///     dependencies.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 124, Pack = 1)]
    internal struct DDS_HEADER
    {
        /// <summary> Size of structure. This member must be set to 124. </summary>
        [FieldOffset(0)] public uint dwSize;

        /// <summary> Flags to indicate which members contain valid data. </summary>
        /// <remarks>
        ///     When you write .dds files, you should set the <see cref="DDS_FLAGS.DDSD_CAPS" /> and
        ///     <see cref="DDS_FLAGS.DDSD_PIXELFORMAT" /> flags,
        ///     and for mipmapped textures you should also set the <see cref="DDS_FLAGS.DDSD_MIPMAPCOUNT" /> flag.
        ///     However, when you read a .dds file, you should not rely on the <see cref="DDS_FLAGS.DDSD_CAPS" />,
        ///     <see cref="DDS_FLAGS.DDSD_PIXELFORMAT" />, and <see cref="DDS_FLAGS.DDSD_MIPMAPCOUNT" />
        ///     flags being set because some writers of such a file might not set these flags.
        /// </remarks>
        [FieldOffset(4)] public DDS_FLAGS dwFlags;

        /// <summary> Surface height (in pixels). </summary>
        [FieldOffset(8)] public uint dwHeight;

        /// <summary> Surface width (in pixels). </summary>
        [FieldOffset(12)] public uint dwWidth;

        /// <summary>
        ///     The pitch or number of bytes per scan line in an uncompressed texture;
        ///     the total number of bytes in the top level texture for a compressed texture.
        /// </summary>
        [FieldOffset(16)] public uint dwPitchOrLinearSize;

        /// <summary> Depth of a volume texture (in pixels), otherwise unused. </summary>
        [FieldOffset(20)] public uint dwDepth;

        /// <summary> Number of mipmap levels, otherwise unused. </summary>
        [FieldOffset(24)] public uint dwMipMapCount;

        /// <summary> Unused. </summary>
        [FieldOffset(28), MarshalAs(UnmanagedType.ByValArray, SizeConst = 11)] public uint[] dwReserved1;

        /// <summary> The pixel format. </summary>
        [FieldOffset(72)] // 28 + sizeof(uint) * 11
        public DDS_PIXELFORMAT ddspf;

        /// <summary>Specifies the complexity of the surfaces stored. </summary>
        /// <remarks>
        ///     When you write .dds files, you should set the <see cref="DDS_CAPS.DDSCAPS_TEXTURE" /> flag,
        ///     and for multiple surfaces you should also set the <see cref="DDS_CAPS.DDSCAPS_COMPLEX" /> flag.
        ///     However, when you read a .dds file, you should not rely on the <see cref="DDS_CAPS.DDSCAPS_TEXTURE" /> and
        ///     <see cref="DDS_CAPS.DDSCAPS_COMPLEX" /> flags being set because some file writers might not set these flags.
        /// </remarks>
        [FieldOffset(104)] //72 + 32
        public DDS_CAPS dwCaps;

        /// <summary> Defines additional capabilities of the surface.</summary>
        [FieldOffset(108)] public DDS_CAPS2 dwCaps2;

        /// <summary> Unused. </summary>
        [FieldOffset(112)] public uint dwCaps3;

        /// <summary> Unused. </summary>
        [FieldOffset(116)] public uint dwCaps4;

        /// <summary> Unused. </summary>
        [FieldOffset(120)] public uint dwReserved2;

        /// <summary> Determines whether this resource is a cubemap. </summary>
        /// <value> <c>true</c> if this resource is a cubemap; otherwise, <c>false</c>. </value>
        public bool IsCubemap => (dwCaps2 & DDS_CAPS2.DDSCAPS2_CUBEMAP) != 0;

        /// <summary> Determines whether this resource is a volume texture. </summary>
        /// <value> <c>true</c> if this resource is a volume texture; otherwise, <c>false</c>. </value>
        public bool IsVolumeTexture => ((dwCaps2 & DDS_CAPS2.DDSCAPS2_VOLUME) != 0);

        /// <summary> Determines whether this resource contains compressed surface data. </summary>
        /// <value><c>true</c> if this resource contains compressed surface data; otherwise, <c>false</c>.</value>
        public bool IsCompressed => (dwFlags & DDS_FLAGS.DDSD_LINEARSIZE) != 0;

        /// <summary> Determines whether this resource contains alpha data. </summary>
        /// <value> <c>true</c> if this resource contains alpha data; otherwise, <c>false</c>. </value>
        public bool HasAlpha => ((ddspf.dwFlags & DDS_PIXELFORMAT_FLAGS.DDPF_ALPHAPIXELS) != 0);

        /// <summary> Determines whether this resource contains mipmap data. </summary>
        /// <value> <c>true</c> if this resource contains mipmap data; otherwise, <c>false</c>. </value>
        public bool HasMipmaps
            => ((dwCaps & DDS_CAPS.DDSCAPS_MIPMAP) != 0 && (dwFlags & DDS_FLAGS.DDSD_MIPMAPCOUNT) != 0);

        /// <summary> Determines whether this header is valid. </summary>
        /// <param name="strict">if set to <c>true</c>, will check required flags and caps. </param>
        /// <returns> <c>true</c> if this header is valid; otherwise, <c>false</c>. </returns>
        [Pure]
        public bool IsValid(bool strict = false)
        {
            var correctSize = (dwSize == 124) && (ddspf.dwSize == 32);
            var hasRequiredFlags = (dwFlags & DDS_FLAGS.DDSD_CAPS) != 0 &&
                                   (dwFlags & DDS_FLAGS.DDSD_PIXELFORMAT) != 0 &&
                                   (dwCaps & DDS_CAPS.DDSCAPS_TEXTURE) != 0;
            var hasInvalidCompression = ((dwFlags & DDS_FLAGS.DDSD_PITCH) != 0 &&
                                         (dwFlags & DDS_FLAGS.DDSD_LINEARSIZE) != 0);
            return strict
                ? correctSize && hasRequiredFlags && !hasInvalidCompression
                : correctSize && !hasInvalidCompression;
        }

        /// <summary> Checks if dds resource should have the <see cref="DDS_HEADER_DXT10" /> header. </summary>
        /// <returns>
        ///     <c>true</c> if dds resource should have the <see cref="DDS_HEADER_DXT10" /> header;
        ///     otherwise <c>false</c>.
        /// </returns>
        [Pure]
        public bool ShouldHaveDxt10Header()
        {
            return (ddspf.dwFlags == DDS_PIXELFORMAT_FLAGS.DDPF_FOURCC) && (ddspf.dwFourCC == FOURCC.DDS_DX10);
        }

        /// <summary>
        ///     Determines whether width and height flags are set, so <see cref="dwWidth" /> and
        ///     <see cref="dwHeight" /> contain valid values.
        /// </summary>
        /// <returns> <c>true</c> if dimensions flags are set; otherwise, <c>false</c>. </returns>
        [Pure]
        public bool AreDimensionsSet()
        {
            return (dwFlags & DDS_FLAGS.DDSD_WIDTH) != 0 && (dwFlags & DDS_FLAGS.DDSD_HEIGHT) != 0;
        }

        /// <summary>
        ///     Returns either depth of a volume texture in pixels, amount of faces in a cubemap or
        ///     a 1 for a flat resource.
        /// </summary>
        /// <returns> Actual depth of a resource. </returns>
        [Pure]
        public int ComputeDepth()
        {
            var result = 1;
            if (IsVolumeTexture) {
                result = (int) dwDepth;
            }
            else if (IsCubemap) {
                result = 0;
                // Partial cubemaps are not supported by Direct3D >= 11, but lets support them for the legacy sake.
                // So cubemaps can store up to 6 faces:
                if ((dwCaps2 & DDS_CAPS2.DDSCAPS2_CUBEMAP_POSITIVEX) != 0) {
                    result++;
                }

                if ((dwCaps2 & DDS_CAPS2.DDSCAPS2_CUBEMAP_NEGATIVEX) != 0) {
                    result++;
                }

                if ((dwCaps2 & DDS_CAPS2.DDSCAPS2_CUBEMAP_POSITIVEY) != 0) {
                    result++;
                }

                if ((dwCaps2 & DDS_CAPS2.DDSCAPS2_CUBEMAP_NEGATIVEY) != 0) {
                    result++;
                }

                if ((dwCaps2 & DDS_CAPS2.DDSCAPS2_CUBEMAP_POSITIVEZ) != 0) {
                    result++;
                }

                if ((dwCaps2 & DDS_CAPS2.DDSCAPS2_CUBEMAP_NEGATIVEZ) != 0) {
                    result++;
                }
            }

            return result;
        }

        /// <summary> Gets the existing cube map faces, if this header represents a cube map. </summary>
        /// <returns> Types of cube map faces stored in this cube map or null if this is not a cubemap.</returns>
        public SurfaceType[] GetExistingCubemapFaces()
        {
            var depth = ComputeDepth();
            var result = new SurfaceType[depth];
            var index = 0;

            if (depth > 0) {
                if ((dwCaps2 & DDS_CAPS2.DDSCAPS2_CUBEMAP_POSITIVEX) != 0) {
                    result[index++] = SurfaceType.CubemapPositiveX;
                }

                if ((dwCaps2 & DDS_CAPS2.DDSCAPS2_CUBEMAP_NEGATIVEX) != 0) {
                    result[index++] = SurfaceType.CubemapNegativeX;
                }

                if ((dwCaps2 & DDS_CAPS2.DDSCAPS2_CUBEMAP_POSITIVEY) != 0) {
                    result[index++] = SurfaceType.CubemapPositiveY;
                }

                if ((dwCaps2 & DDS_CAPS2.DDSCAPS2_CUBEMAP_NEGATIVEY) != 0) {
                    result[index++] = SurfaceType.CubemapNegativeY;
                }

                if ((dwCaps2 & DDS_CAPS2.DDSCAPS2_CUBEMAP_POSITIVEZ) != 0) {
                    result[index++] = SurfaceType.CubemapPositiveZ;
                }

                if ((dwCaps2 & DDS_CAPS2.DDSCAPS2_CUBEMAP_NEGATIVEZ) != 0) {
                    result[index++] = SurfaceType.CubemapNegativeZ;
                }
            }

            return (index > 0) ? result : null;
        }

        /// <summary> Returns a <see cref="string" /> that represents this instance. </summary>
        /// <returns> A <see cref="string" /> that represents this instance. </returns>
        public override string ToString()
        {
            return $"DDS_HEADER: Size = {dwSize}; Flags = {dwFlags}; Width = {dwWidth}; Height = {dwHeight}; " +
                   $"PitchOrLinearSize = {dwPitchOrLinearSize}; Depth = {dwDepth}; MipMapCount = {dwMipMapCount}; " +
                   $"Caps = {dwCaps}, {dwCaps2};" + Environment.NewLine + ddspf;
        }
    }
}
