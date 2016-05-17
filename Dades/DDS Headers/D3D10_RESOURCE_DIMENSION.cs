// ReSharper disable InconsistentNaming
namespace Dades
{
    /// <summary> Identifies the type of resource being used. </summary>
    internal enum D3D10_RESOURCE_DIMENSION
    {
        /// <summary> Resource is of unknown type. </summary>
        D3D10_RESOURCE_DIMENSION_UNKNOWN = 0,

        /// <summary> Resource is a buffer. </summary>
        D3D10_RESOURCE_DIMENSION_BUFFER = 1,

        /// <summary> Resource is a 1D texture. </summary>
        D3D10_RESOURCE_DIMENSION_TEXTURE1D = 2,

        /// <summary> Resource is a 2D texture. </summary>
        D3D10_RESOURCE_DIMENSION_TEXTURE2D = 3,

        /// <summary> Resource is a 3D texture. </summary>
        D3D10_RESOURCE_DIMENSION_TEXTURE3D = 4
    }
}