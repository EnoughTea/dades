using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Dades
{
    /// <summary> Represents a single texture of any kind, which consists of [1, N) surfaces. </summary>
    public class TextureResource : IEnumerable<Surface>
    {
        /// <summary> Initializes a new instance of the <see cref="TextureResource" /> class. </summary>
        /// <param name="surfaces">The surfaces which this texture consists of.</param>
        public TextureResource(List<Surface> surfaces)
        {
            Contract.Requires(surfaces != null);

            Surfaces = surfaces;
        }

        /// <summary> Gets surfaces for this texture.
        /// Keep in mind that is not necessary for a DDS file to provide all mipmap levels of a texture. </summary>
        /// <remarks>
        ///     It specifically exposes <see cref="List{Surface}" /> instead of usual IEnumerable to remind
        ///     that it is possible to tinker with surfaces and their data.
        /// </remarks>
        public List<Surface> Surfaces { get; }

        /// <summary> Returns an enumerator that iterates through a collection. </summary>
        /// <returns>
        ///     An <see cref="IEnumerator{Surface}" /> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<Surface> GetEnumerator()
        {
            return Surfaces.GetEnumerator();
        }

        /// <summary> Returns an enumerator that iterates through a collection. </summary>
        /// <returns>
        ///     An <see cref="IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return $"Texture with {Surfaces.Count} surfaces.";
        }
    }
}
