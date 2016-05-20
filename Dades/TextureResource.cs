using System.Diagnostics.Contracts;

namespace Dades
{
    /// <summary> Represents a single texture of any kind, which consists of [1, N) surfaces. </summary>
    public class TextureResource
    {
        /// <summary> Initializes a new instance of the <see cref="TextureResource" /> class. </summary>
        /// <param name="surfaces">The surfaces which this texture consists of.</param>
        public TextureResource(Surface[] surfaces)
        {
            Contract.Requires(surfaces != null);

            Surfaces = surfaces;
        }

        /// <summary> Gets surfaces for this texture.
        /// Keep in mind that is not necessary for a DDS file to provide all mipmap levels of a texture. </summary>
        public Surface[] Surfaces { get; }

        /// <summary>Returns a <see cref="string" /> that represents this instance.</summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return $"Texture with {Surfaces.Length} surfaces.";
        }
    }
}
