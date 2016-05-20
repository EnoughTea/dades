using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Dades
{
    /// <summary> Holds surface info and actual pixel data. </summary>
    public class Surface
    {
        /// <summary> Initializes a new instance of the <see cref="Surface" /> class. </summary>
        /// <param name="type">The surface type.</param>
        /// <param name="level">The mip level represented by this surface.</param>
        /// <param name="width">The width of this surface in pixels.</param>
        /// <param name="height">The height of this surface in pixels.</param>
        /// <param name="data">The pixel data.</param>
        public Surface(SurfaceType type, int level, int width, int height, byte[] data)
        {
            Contract.Requires(data != null);

            Type = type;
            Level = level;
            Width = width;
            Height = height;
            Data = data;
        }

        /// <summary> Gets the surface type. </summary>
        public SurfaceType Type { get; private set; }

        /// <summary> Gets the mip level represented by this surface. </summary>
        public int Level { get; private set; }

        /// <summary> Gets the pixel data. It's not a copy but an original array.</summary>
        public byte[] Data { get; private set; }

        /// <summary> Gets the width of this surface in pixels. </summary>
        public int Width { get; private set; }

        /// <summary> Gets the height of this surface in pixels. </summary>
        public int Height { get; private set; }

        /// <summary> Creates a single surface the given byte data. </summary>
        /// <param name="rawData">Raw and unflipped surface bytes.</param>
        /// <param name="surfaceType">Type of the surface.</param>
        /// <param name="level">The mip level of the surface.</param>
        /// <param name="width">The surface width in pixels.</param>
        /// <param name="height">The surface width in pixels.</param>
        /// <param name="formatD3D">
        ///     D3D resource data format. Can be used instead of <paramref name="formatDxgi" />.
        /// </param>
        /// <param name="formatDxgi">
        ///     DXGI resource data format. Can be used instead of <paramref name="formatD3D" />.
        /// </param>
        /// <param name="vFlip">true if surface should be flipped vertically, useful for OpenGL.</param>
        /// <returns>Created surface.</returns>
        /// <exception cref="NotSupportedException">Packed format is currently untested with vertical flip.</exception>
        public static Surface FromBytes(byte[] rawData, SurfaceType surfaceType, int level, int width, int height,
                                        D3DFormat formatD3D, DxgiFormat formatDxgi, bool vFlip)
        {
            Debug.Assert(rawData.Length > 0); // This should never happen if pitch and linear size are correct.

            int bitsPerPixel = DdsTools.GetBitsPerPixel(formatD3D, formatDxgi);

            // Flip block-compressed formats:
            if (DdsTools.IsBlockCompressedFormat(formatD3D) || DdsTools.IsBlockCompressedFormat(formatDxgi)) {
                if (vFlip && width > 2 && height > 2) {
                    // Every D3D BC format can be converted to its DXGI BC representation:
                    Debug.Assert(formatDxgi != DxgiFormat.Unknown);
                    DdsTools.FlipBCSurface(rawData, width, height, bitsPerPixel * 2, formatDxgi);
                }
            }
            else if (vFlip) {
                // Flip other formats.
                // TODO: Test this naive flipping with obscure packed formats.
                int bytes = bitsPerPixel / 8;
                for (var sourceColumn = 0; sourceColumn < height / 2; sourceColumn++) {
                    int targetColumn = height - sourceColumn - 1;
                    for (var row = 0; row < width; row++) {
                        int source = (sourceColumn * width + row) * bytes;
                        int target = (targetColumn * width + row) * bytes;

                        for (var i = 0; i < bytes; i++) {
                            byte temp = rawData[source + i];
                            rawData[source + i] = rawData[target + i];
                            rawData[target + i] = temp;
                        }
                    }
                }
            }

            return new Surface(surfaceType, level, width, height, rawData);
        }
    }
}
