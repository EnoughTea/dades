// ReSharper disable InconsistentNaming
using System;

namespace Dades
{
    /// <summary> Identifies other, less common options for resources. </summary>
    /// <remarks>
    ///     <see cref="D3D10_RESOURCE_MISC_SHARED" /> and <see cref="D3D10_RESOURCE_MISC_SHARED_KEYEDMUTEX" />
    ///     are mutually exclusive flags: either one may be set in the resource creation calls but not both simultaneously.
    /// </remarks>
    [Flags]
    internal enum D3D10_RESOURCE_MISC_FLAG : uint
    {
        /// <summary> Added for correctness. </summary>
        None = 0,

        /// <summary>
        ///     Enables an application to call ID3D10Device::GenerateMips on a texture resource.
        ///     The resource must be created with the bind flags that specify that the resource is a render target and a
        ///     shader resource.
        /// </summary>
        D3D10_RESOURCE_MISC_GENERATE_MIPS = 0x1,

        /// <summary>
        ///     Enables the sharing of resource data between two or more Direct3D devices.
        ///     The only resources that can be shared are 2D non-mipmapped textures.
        ///     WARP and REF devices do not support shared resources. Attempting to create a resource with this flag on
        ///     either a WARP or REF device will cause the create method to return an E_OUTOFMEMORY error code.
        /// </summary>
        D3D10_RESOURCE_MISC_SHARED = 0x2,

        /// <summary>
        ///     Enables an application to create a cube texture from a Texture2DArray that contains 6 textures.
        /// </summary>
        D3D10_RESOURCE_MISC_TEXTURECUBE = 0x4,

        D3D10_RESOURCE_MISC_SHARED_KEYEDMUTEX = 0x10,

        /// <summary>
        ///     Enables a surface to be used for GDI interoperability. Setting this flag enables rendering on
        ///     the surface via IDXGISurface1::GetDC.
        /// </summary>
        D3D10_RESOURCE_MISC_GDI_COMPATIBLE = 0x20
    }
}