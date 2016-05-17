namespace Dades
{
    /// <summary> Shows what kind of surface we're dealing with. </summary>
    /// <remarks> For occasional convenience enum values correspond to the OpenGL texture target values. </remarks>
    public enum SurfaceType
    {
        /// <summary> Default value. </summary>
        Unknown,

        /// <summary> Positive X cube map face. </summary>
        CubemapPositiveX = 0x8515,

        /// <summary> Negative X cube map face. </summary>
        CubemapNegativeX = 0x8516,

        /// <summary> Positive Y cube map face. </summary>
        CubemapPositiveY = 0x8517,

        /// <summary> Negative Y cube map face. </summary>
        CubemapNegativeY = 0x8518,

        /// <summary> Positive Z cube map face. </summary>
        CubemapPositiveZ = 0x8519,

        /// <summary> Negative Z cube map face. </summary>
        CubemapNegativeZ = 0x851A,

        /// <summary> Represents a one-dimensional texture (height == 1). </summary>
        Texture1D = 0x0DE0,

        /// <summary> Represents a usual two-dimensional texture. </summary>
        Texture2D = 0x0DE1,

        /// <summary> Represents slices of a volume texture for a one mip-map level. </summary>
        Texture3D = 0x806F
    }
}
