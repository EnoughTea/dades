using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace Dades
{
    /// <summary> DDS file data reader. </summary>
    /// <remarks>
    ///     Even though this reader can deal with almost anything (some packed formats will break it),
    ///     usually it's safe to have your DDS in one of the following formats:
    ///     * DXGI_FORMAT_R8G8B8A8_UNORM and its SRGB
    ///     * DXGI_FORMAT_R8G8B8A8_SNORM
    ///     * DXGI_FORMAT_B8G8R8A8_UNORM
    ///     * DXGI_FORMAT_R16G16_SNORM
    ///     * DXGI_FORMAT_R8G8_SNORM
    ///     * DXGI_FORMAT_R8_UNORM
    ///     * DXGI_FORMAT_BC1_UNORM and its SRGB
    ///     * DXGI_FORMAT_BC2_UNORM and its SRGB
    ///     * DXGI_FORMAT_BC3_UNORM and its SRGB
    /// </remarks>
    public class DdsFileData
    {
        private DDS_HEADER _header;
        private DDS_HEADER_DXT10? _headerDxt10;
        private int _resourceCount;
        private bool _vFlip;

        /// <summary> Initializes a new instance of the <see cref="DdsFileData" /> class. </summary>
        /// <param name="ddsStream">The DDS file to read from.</param>
        /// <param name="doVerticalFlip">
        ///     if set to <c>true</c>, pixel data will be vertically flipped. Useful for OpenGL.
        /// </param>
        /// <exception cref="DdsException">Throws when DDS file stream is invalid.</exception>
        public DdsFileData(Stream ddsStream, bool doVerticalFlip = true)
        {
            Contract.Requires(ddsStream != null);
            Contract.Requires(ddsStream.Length > 0);

            LoadFromStream(ddsStream, doVerticalFlip);
        }

        /// <summary> Initializes a new instance of the <see cref="DdsFileData" /> class. </summary>
        /// <param name="ddsFile">The DDS file to read from.</param>
        /// <param name="doVerticalFlip">
        ///     if set to <c>true</c>, pixel data will be vertically flipped. Useful for OpenGL.
        /// </param>
        /// <exception cref="DdsException">Throws when DDS file is invalid.</exception>
        public DdsFileData(string ddsFile, bool doVerticalFlip = true)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(ddsFile));
            Contract.Requires(File.Exists(ddsFile));

            using (var stream = File.Open(ddsFile, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                LoadFromStream(stream, doVerticalFlip);
            }
        }

        /// <summary> Gets the width of the top level surface (in pixels). </summary>
        public int Width => (int) _header.dwWidth;

        /// <summary> Gets the height of the top level surface (in pixels). </summary>
        public int Height => (int) _header.dwHeight;

        /// <summary> Gets the DirectX &lt;= 9 representation of the resource format. </summary>
        public D3DFormat FormatD3D { get; private set; }

        /// <summary>
        ///     Gets the DirectX &gt; 10 representation of the resource format. If no DXGI format is present,
        ///     tries to map a D3D format to a corresponding DXGI format.
        /// </summary>
        public DxgiFormat FormatDxgi { get; private set; }

        /// <summary>
        ///     Gets all texture resources contained in the file.
        ///     File is guaranteed to contain at least one texture; if it is a texture array, there will be more than one.
        /// </summary>
        public List<TextureResource> Textures { get; } = new List<TextureResource>(1);

        /// <summary> Gets amount of bits per pixel for a surface data. </summary>
        public int BitsPerPixel { get; private set; }

        /// <summary>
        ///     Gets the pitch of the top level surface: number of bytes per scan line.
        ///     For DXT formats it's the amount of bytes per horizontal block line.
        /// </summary>
        public int Pitch { get; private set; }

        /// <summary> Gets the total number of bytes in the top level surface. </summary>
        public int LinearSize { get; private set; }

        /// <summary> Gets either depth of a volume texture in pixels or amount of faces in a cubemap. </summary>
        public int Depth { get; private set; }

        /// <summary>
        ///     Gets number of mip-map levels for a mip-mapped texture, original image also counted as a mip-map
        ///     lefel; Returns 0 if no mip-maps are present.
        /// </summary>
        public int MipMapCount { get; private set; }

        /// <summary> Gets the total size of all surfaces in all texture resources. </summary>
        public long TotalResourceSize { get; private set; }

        /// <summary> Determines whether DDS resource is a cubemap. </summary>
        /// <value> <c>true</c> if the DDS resource is a cubemap; otherwise, <c>false</c>. </value>
        public bool IsCubemap => _header.IsCubemap;

        /// <summary> Determines whether the DDS resource is a volume texture. </summary>
        /// <value> <c>true</c> if the DDS is a volume texture; otherwise, <c>false</c>. </value>
        public bool IsVolumeTexture => _header.IsVolumeTexture;

        /// <summary> Determines whether the DDS contains block-compressed surface data. </summary>
        /// <value>
        ///     <c>true</c> if the DDS contains block-compressed surface data;
        ///     otherwise, <c>false</c>.
        /// </value>
        public bool IsBlockCompressed => _header.IsCompressed;

        /// <summary> Gets a value indicating whether this file data came from a DX10 DDS file. DX10 DDS files have
        ///  different format (<see cref="DxgiFormat"/>) and can contain texture arrays. </summary>
        public bool IsDx10 => _headerDxt10 != null;

        /// <summary> Determines whether the DDS contains alpha data. </summary>
        /// <value> <c>true</c> if the DDS contains alpha data; otherwise, <c>false</c>. </value>
        public bool HasAlpha => _header.HasAlpha;

        /// <summary> Returns a <see cref="string" /> that represents this instance. </summary>
        /// <returns> A <see cref="string" /> that represents this instance. </returns>
        public override string ToString()
        {
            return (_headerDxt10 != null)
                ? _header + Environment.NewLine + _headerDxt10.Value
                : _header.ToString();
        }

        /// <summary> Loads DDS from stream. </summary>
        /// <param name="ddsStream">The DDS stream.</param>
        /// <param name="verticalFlip">If set to <c>true</c>, will perform vertical flip on the pixel data.</param>
        private void LoadFromStream(Stream ddsStream, bool verticalFlip)
        {
            _vFlip = verticalFlip;

            // Firstly load a required DDS_HEADER and check all required stuff.
            ReadHeader(ddsStream);

            // There could be an DXT10 header:
            if (_header.ShouldHaveDxt10Header()) {
                _headerDxt10 = ddsStream.ReadStruct<DDS_HEADER_DXT10>();
                // DXT10 file always contains at least 1 texture.
                _resourceCount = (_headerDxt10.Value.arraySize > 0) ? (int) _headerDxt10.Value.arraySize : 1;
            }
            else {
                _resourceCount = 1; // Non-dxt10 files always contain only one texture.
            }

            FillFormatInfo();

            // Calculate properties:
            Pitch = DdsTools.ComputePitch(Width, FormatD3D, FormatDxgi, (int) _header.dwPitchOrLinearSize);
            LinearSize = DdsTools.ComputeLinearSize(Width, Height, FormatD3D, FormatDxgi,
                                                    (int) _header.dwPitchOrLinearSize);
            Depth = _header.ComputeDepth();
            if (_header.HasMipmaps) {
                MipMapCount = (int) _header.dwMipMapCount;
            }

            // Load all surfaces:
            using (var reader = new BinaryReader(ddsStream)) {
                if (IsVolumeTexture) {
                    ReadVolumeTexture(reader);
                }
                else if (IsCubemap) {
                    ReadCubemap(reader);
                }
                else {
                    ReadFlatTexture(reader);
                }
            }
        }

        /// <summary> Reads header from the specified stream containing DDS file and validates it. </summary>
        /// <param name="ddsStream">The DDS stream.</param>
        /// <exception cref="DdsException">
        ///     Trying to read a non-DDS file -or- DDS header is invalid -or-
        ///     DDS header does not contain texture dimensions -or-
        ///     Dimensions of the compressed formats must be divisible by 4.
        /// </exception>
        private void ReadHeader(Stream ddsStream)
        {
            // That's a reading of an UInt32.
            var magicNumber = (uint) (ddsStream.ReadByte() + (ddsStream.ReadByte() << 8) +
                                      (ddsStream.ReadByte() << 16) + (ddsStream.ReadByte() << 24));

            // Magic number should contain the four character code value 'DDS '.
            if (magicNumber != FOURCC.DDS_MAGICWORD) {
                throw new DdsException("Trying to read a non-DDS file.");
            }

            _header = ddsStream.ReadStruct<DDS_HEADER>();
            if (!_header.IsValid()) {
                throw new DdsException("DDS header is invalid.");
            }

            if (!_header.AreDimensionsSet()) {
                throw new DdsException("DDS header does not contain texture dimensions.");
            }

            if (IsBlockCompressed && (Width % 4 != 0 || Height % 4 != 0)) {
                throw new DdsException("Dimensions of the compressed formats must be divisible by 4");
            }
        }

        /// <summary> Sets the format-related properties. </summary>
        /// <exception cref="DdsException">DDS texture is in an unknown format.</exception>
        private void FillFormatInfo()
        {
            FormatD3D = _header.ddspf.GetD3DFormat();
            FormatDxgi = (_headerDxt10 != null) ? _headerDxt10.Value.dxgiFormat : DxgiFormat.Unknown;

            if (FormatDxgi == DxgiFormat.Unknown) {
                // If both formats are unknown, it's time to quit.
                if (FormatD3D == D3DFormat.Unknown) {
                    throw new DdsException("DDS texture is in an unknown format.");
                }

                FormatDxgi = DdsTools.D3DToDxgiFormat(FormatD3D);
                BitsPerPixel = DdsTools.GetBitsPerPixel(FormatD3D);
            }
            else {
                BitsPerPixel = DdsTools.GetBitsPerPixel(FormatDxgi);
            }
        }

        /// <summary> Reads the generic flat texture. </summary>
        /// <param name="reader">The stream reader.</param>
        private void ReadFlatTexture(BinaryReader reader)
        {
            var textureType = (Height > 1) ? SurfaceType.Texture2D : SurfaceType.Texture1D;
            var surfaceCount = (MipMapCount == 0) ? 1 : MipMapCount;
            for (var i = 0; i < _resourceCount; i++) {
                // Flat textures contain only mip-map levels, so read them:
                var surfaces = ReadMipMapSurfaces(reader, surfaceCount, textureType, Width, Height);
                Textures.Add(new TextureResource(surfaces.ToArray()));
            }
        }

        /// <summary> Reads the volume texture. </summary>
        /// <param name="reader">The stream reader.</param>
        private void ReadVolumeTexture(BinaryReader reader)
        {
            var surfaceCount = (MipMapCount == 0) ? 1 : MipMapCount;
            for (var i = 0; i < _resourceCount; i++) {
                // Volume textures contain slices first, mip levels for them later.
                // So for every mip-map level load all its slices:
                var surfaces = ReadMipMapSurfaces(reader, surfaceCount, SurfaceType.Texture3D, Width, Height, Depth);
                Textures.Add(new TextureResource(surfaces.ToArray()));
            }
        }

        /// <summary> Reads the cube map. </summary>
        /// <param name="reader">The stream reader.</param>
        private void ReadCubemap(BinaryReader reader)
        {
            var surfaceCount = (MipMapCount == 0) ? 1 : MipMapCount;
            var faces = _header.GetExistingCubemapFaces();
            if (faces == null) {
                return;
            }

            for (var i = 0; i < _resourceCount; i++) {
                // Cube maps contain mip-map levels after every cube face.
                // So for every face read all mip-map levels:
                var surfaces = new List<Surface>(Depth * MipMapCount);
                for (var face = 0; face < Depth; face++) {
                    // I'm sure we do not have to swap positive and negative Y faces for OpenGL...
                    var faceType = faces[face];
                    surfaces.AddRange(ReadMipMapSurfaces(reader, surfaceCount, faceType, Width, Height));
                }

                Textures.Add(new TextureResource(surfaces.ToArray()));
            }
        }

        /// <summary> Reads all mip surfaces for the specified depth. </summary>
        /// <param name="reader">The stream reader.</param>
        /// <param name="surfaceCount">Total count of surfaces to read.</param>
        /// <param name="surfaceType">Type of the surfacse.</param>
        /// <param name="width">The surface width in pixels.</param>
        /// <param name="height">The surface width in pixels.</param>
        /// <param name="depth"> Depth of single surface. Always equals 1 except for volume textures.</param>
        private IEnumerable<Surface> ReadMipMapSurfaces(BinaryReader reader, int surfaceCount, SurfaceType surfaceType,
                                                        int width, int height, int depth = 1)
        {
            // Load every mip-map level:
            for (var level = 0; level < surfaceCount && (width > 0 || height > 0); ++level) {
                var surfaceSize = DdsTools.ComputeLinearSize(width, height, FormatD3D, FormatDxgi,
                                                             (int) _header.dwPitchOrLinearSize) * depth;
                TotalResourceSize += surfaceSize;

                var data = reader.ReadBytes(surfaceSize);
                yield return Surface.FromBytes(data, surfaceType, level, width, height, FormatD3D, FormatDxgi,
                    _vFlip);

                width /= 2;
                height /= 2;
            }
        }
    }
}
