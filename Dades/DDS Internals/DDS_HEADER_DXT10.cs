// ReSharper disable InconsistentNaming

using System.Runtime.InteropServices;

namespace Dades
{
    /// <summary> DDS header extension to handle resource arrays. </summary>
    /// <remarks>
    ///     Use this structure together with a <see cref="DDS_HEADER" /> to store a resource array in a DDS file.
    ///     For more information, see texture arrays.
    ///     This header is present if the <see cref="DDS_PIXELFORMAT.dwFourCC" /> member of the
    ///     <see cref="DDS_PIXELFORMAT" /> structure is set to 'DX10'.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential, Size = 20, Pack = 1)]
    internal struct DDS_HEADER_DXT10
    {
        /// <summary> The surface pixel format. </summary>
        public DxgiFormat dxgiFormat;

        /// <summary>
        ///     Identifies the type of resource.
        ///     The following values for this member are a subset of the values in the
        ///     <see cref="D3D10_RESOURCE_DIMENSION" /> enumeration:
        ///     <see cref="D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE1D" /> :
        ///     Resource is a 1D texture. The <see cref="DDS_HEADER.dwWidth" /> member of <see cref="DDS_HEADER" />
        ///     specifies the size of the texture. Typically, you set the <see cref="DDS_HEADER.dwHeight" /> member of
        ///     <see cref="DDS_HEADER" /> to 1; you also must set the <see cref="DDS_FLAGS.DDSD_HEIGHT" /> flag in
        ///     the <see cref="DDS_HEADER.dwFlags" /> member of <see cref="DDS_HEADER" />.
        ///     <see cref="D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE2D" /> :
        ///     Resource is a 2D texture with an area specified by the <see cref="DDS_HEADER.dwWidth" /> and
        ///     <see cref="DDS_HEADER.dwHeight" /> members of <see cref="DDS_HEADER" />.
        ///     You can also use this type to identify a cube-map texture. For more information about how to identify a
        ///     cube-map texture, see <see cref="miscFlag" /> and <see cref="arraySize" /> members.
        ///     <see cref="D3D10_RESOURCE_DIMENSION.D3D10_RESOURCE_DIMENSION_TEXTURE3D" /> :
        ///     Resource is a 3D texture with a volume specified by the <see cref="DDS_HEADER.dwWidth" />,
        ///     <see cref="DDS_HEADER.dwHeight" />, and <see cref="DDS_HEADER.dwDepth" /> members of
        ///     <see cref="DDS_HEADER" />. You also must set the <see cref="DDS_FLAGS.DDSD_DEPTH" /> flag
        ///     in the <see cref="DDS_HEADER.dwFlags" /> member of <see cref="DDS_HEADER" />.
        /// </summary>
        public D3D10_RESOURCE_DIMENSION resourceDimension;

        /// <summary>
        ///     Identifies other, less common options for resources. The following value for this member is a
        ///     subset of the values in the <see cref="D3D10_RESOURCE_MISC_FLAG" /> enumeration:
        ///     DDS_RESOURCE_MISC_TEXTURECUBE Indicates a 2D texture is a cube-map texture.
        /// </summary>
        public D3D10_RESOURCE_MISC_FLAG miscFlag;

        /// <summary>
        ///     The number of elements in the array.
        ///     For a 2D texture that is also a cube-map texture, this number represents the number of cubes.
        ///     This number is the same as the number in the NumCubes member of D3D10_TEXCUBE_ARRAY_SRV1 or
        ///     D3D11_TEXCUBE_ARRAY_SRV). In this case, the DDS file contains arraySize*6 2D textures.
        ///     For more information about this case, see the <see cref="miscFlag" /> description.
        ///     For a 3D texture, you must set this number to 1.
        /// </summary>
        public uint arraySize;

        /// <summary> Reserved for future use. </summary>
        public uint reserved;


        /// <summary> Returns a <see cref="string" /> that represents this instance. </summary>
        /// <returns> A <see cref="string" /> that represents this instance. </returns>
        public override string ToString()
        {
            return $"DDS_HEADER_DXT10: Format = {dxgiFormat}; ResourceDimension = {resourceDimension}; " +
                   $"MiscFlag = {miscFlag}; ArraySize = {arraySize}; ";
        }
    }
}
