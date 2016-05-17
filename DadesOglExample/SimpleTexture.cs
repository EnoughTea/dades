using System;
using System.Collections.Generic;
using Dades;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace DadesOglExample
{
    /// <summary> Represents a texture type. </summary>
    public enum TextureKind
    {
        Flat = TextureTarget.Texture2D,
        Cubemap = TextureTarget.TextureCubeMap,
        Volume = TextureTarget.Texture3D
    }

    /// <summary>
    ///     Very primitive class which demonstrates how texture loading could be done with <see cref="DdsFileData" />.
    /// </summary>
    public class SimpleTexture : IDisposable
    {
        private static readonly HashSet<string> _SupportedExtensions = new HashSet<string>(
            GL.GetString(StringName.Extensions).Split(new[] {' '}));

        private static readonly bool _SupportsNpot = _SupportedExtensions.Contains("GL_ARB_texture_non_power_of_two");
        private TextureTarget _target;

        /// <summary> Initializes a new instance of the <see cref="SimpleTexture" /> class. </summary>
        /// <param name="kind">The texture kind.</param>
        /// <param name="filename">The file to load.</param>
        /// <param name="verticalFlip">if set to <c>true</c>, will perform vertical flip on the pixel data.</param>
        public SimpleTexture(TextureKind kind, string filename, bool verticalFlip = true)
        {
            Id = GL.GenTexture();
            Kind = kind;
            _target = (TextureTarget) Kind;
            Load(filename, verticalFlip);
        }

        /// <summary> Gets the OpenGL texture ID. </summary>
        public int Id { get; }

        /// <summary> Gets the texture kind. </summary>
        public TextureKind Kind { get; }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting
        ///     unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary> Loads a .dds texture into the default texture unit. </summary>
        /// <param name="filename">File to load.</param>
        /// <param name="verticalFlip">if set to <c>true</c>, will perform vertical flip on the pixel data.</param>
        private void Load(string filename, bool verticalFlip)
        {
            var dds = new DdsFileData(filename, verticalFlip);
            // This time lets deal only with DXGI formats, so get either the original one or D3D-casted one.
            var format = new PixelFormatInfo(dds.FormatDxgi);

            // Check if we can deal with texture dimensions or not:
            if ((!IsPoT(dds.Width) || !IsPoT(dds.Height)) && !_SupportsNpot) {
                throw new ArgumentException("Texture has NPOT dimensions, and there is no support for them.");
            }

            // Adjust our texture target if this flat texture turned out to be 1D:
            if (_target == TextureTarget.Texture2D && dds.Height == 1) {
                _target = TextureTarget.Texture1D;
            }

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(_target, Id);
            if (Kind == TextureKind.Flat || Kind == TextureKind.Cubemap) // Load flat textures and cube-maps:
            {
                // Simply read all surfaces, they already have correct layout:
                foreach (var surface in dds.Textures[0]) {
                    // If this is a cube map, find which cube map face we have to load:
                    var casted = (Kind == TextureKind.Cubemap) ? (TextureTarget) surface.Type : _target;
                    if (dds.IsBlockCompressed) // Indicates that this is one of BC formats.
                    {
                        GL.CompressedTexImage2D(casted, surface.Level, format.InternalFormat, surface.Width,
                                                surface.Height, 0, surface.Data.Length, surface.Data);
                    }
                    else {
                        GL.TexImage2D(casted, surface.Level, format.InternalFormat, surface.Width, surface.Height, 0,
                                      format.Format, format.Type, surface.Data);
                    }
                }
            }
            else if (Kind == TextureKind.Volume) // Load volume textures:
            {
                // Each 'slices' is a single mip level of slices in a volume texture.
                foreach (var slices in dds.Textures[0]) {
                    if (dds.IsBlockCompressed) // Indicates that this is one of BC formats.
                    {
                        GL.CompressedTexImage3D(_target, slices.Level, format.InternalFormat, dds.Width,
                                                dds.Height, dds.Depth, 0, slices.Data.Length, slices.Data);
                    }
                    else {
                        GL.TexImage3D(_target, slices.Level, format.InternalFormat, dds.Width, dds.Height,
                                      dds.Depth, 0, format.Format, format.Type, slices.Data);
                    }
                }
            }

            // Set filtering:
            GL.TexParameter(_target, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.Nearest);
            GL.TexParameter(_target, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);

            // Set wrapping modes:
            var wrapMode = (Kind == TextureKind.Cubemap) // Cube maps need clamping:
                ? (int) TextureWrapMode.ClampToEdge
                : (int) TextureWrapMode.Repeat;
            GL.TexParameter(_target, TextureParameterName.TextureWrapS, wrapMode);
            if (_target != TextureTarget.Texture1D) {
                GL.TexParameter(_target, TextureParameterName.TextureWrapT, wrapMode);
            }

            if (Kind != TextureKind.Flat) {
                GL.TexParameter(_target, TextureParameterName.TextureWrapR, wrapMode);
            }

            // Set mip-map level:
            GL.TexParameter(_target, TextureParameterName.TextureBaseLevel, 0);
            GL.TexParameter(_target, TextureParameterName.TextureMaxLevel,
                            dds.MipMapCount > 0 ? dds.MipMapCount - 1 : 0);
        }

        /// <summary> Sets states needed for the current texture. </summary>
        public void Enable()
        {
            switch (Kind) {
                case TextureKind.Flat:
                    GL.Disable(EnableCap.TextureCubeMap);
                    GL.Disable(EnableCap.Texture3DExt);
                    GL.Enable(EnableCap.Texture2D);
                    break;
                case TextureKind.Cubemap:
                    GL.Disable(EnableCap.Texture2D);
                    GL.Disable(EnableCap.Texture3DExt);
                    GL.Enable(EnableCap.TextureCubeMap);
                    break;
                case TextureKind.Volume:
                    GL.Disable(EnableCap.Texture2D);
                    GL.Disable(EnableCap.TextureCubeMap);
                    GL.Enable(EnableCap.Texture3DExt);
                    break;
            }
        }

        /// <summary> Finalizes an instance of the <see cref="SimpleTexture" /> class. </summary>
        ~SimpleTexture()
        {
            Dispose(false);
        }

        /// <summary> Releases unmanaged and - optionally - managed resources. </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources;
        ///     <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (Id != 0 && GraphicsContext.CurrentContext != null) {
                GL.BindTexture(_target, 0);
                GL.DeleteTexture(Id);
            }
        }

        /// <summary> Performs a power-of-two check of the given value. </summary>
        /// <param name="value">Value to check.</param>
        /// <returns> Returns true if the specified value is a power of two; false otherwise. </returns>
        private static bool IsPoT(int value)
        {
            return (value > 0) && ((value & (value - 1)) == 0);
        }
    }
}
