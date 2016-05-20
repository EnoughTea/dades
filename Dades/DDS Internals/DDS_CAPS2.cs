// ReSharper disable InconsistentNaming
using System;

namespace Dades
{
    /// <summary> Additional detail about the surfaces stored. </summary>
    /// <remarks>
    ///     Although Direct3D 9 supports partial cube-maps, Direct3D 10, 10.1, and 11 require that you
    ///     define all six cube-map faces (that is, you must set DDS_CUBEMAP_ALLFACES).
    /// </remarks>
    [Flags]
    internal enum DDS_CAPS2 : uint
    {
        /// <summary> Added for correctness. </summary>
        None = 0,

        /// <summary> Required for a cube map. </summary>
        DDSCAPS2_CUBEMAP = 0x200,

        /// <summary> Required when these surfaces are stored in a cube map. </summary>
        DDSCAPS2_CUBEMAP_POSITIVEX = 0x400,

        /// <summary> Required when these surfaces are stored in a cube map. </summary>
        DDSCAPS2_CUBEMAP_NEGATIVEX = 0x800,

        /// <summary> Required when these surfaces are stored in a cube map. </summary>
        DDSCAPS2_CUBEMAP_POSITIVEY = 0x1000,

        /// <summary> Required when these surfaces are stored in a cube map. </summary>
        DDSCAPS2_CUBEMAP_NEGATIVEY = 0x2000,

        /// <summary> Required when these surfaces are stored in a cube map. </summary>
        DDSCAPS2_CUBEMAP_POSITIVEZ = 0x4000,

        /// <summary> Required when these surfaces are stored in a cube map. </summary>
        DDSCAPS2_CUBEMAP_NEGATIVEZ = 0x8000,

        /// <summary> Required for a volume texture. </summary>
        DDSCAPS2_VOLUME = 0x200000
    }
}