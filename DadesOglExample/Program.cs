using System;
using System.IO;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace DadesOglExample
{
    /// <summary>
    ///     Simple texture test, which allows user to view textures with cursor keys.
    ///     Press left and right cursos keys to change textures, press up and down cursor keys to change texture types.
    ///     Textures will be searched for in some predefined directories.
    /// </summary>
    internal class Program
    {
        // Initial window dimensions:
        private const int InitialWidth = 800;
        private const int InitialHeight = 600;
        // Test textures will be loaded from these directories:
        private const string DataDir = "Data";
        private static readonly string _FlatTestDir = Path.Combine(DataDir, "TestFlat");
        private static readonly string _CubemapTestDir = Path.Combine(DataDir, "TestCubemap");
        private static readonly string _VolumeTestDir = Path.Combine(DataDir, "TestVolume");

        private static GameWindow game;
        private static TextureKind _currentTextureKind = TextureKind.Flat; // Current textures are of this type.
        private static string[] _textureFiles; // Current textures will be loaded from these files.

        private static int _currentTextureIndex;
                           // This is index of the currently loaded texture in the 'textureFiles' array.

        private static SimpleTexture _currentTexture; // This is a currently loaded texture.

        /// <summary> Main entry point. </summary>
        internal static void Main(string[] args)
        {
            game = new GameWindow(InitialWidth, InitialHeight, GraphicsMode.Default, "DDS loading test") {
                VSync = VSyncMode.On
            };
            game.Load += Init;
            game.RenderFrame += RenderTexturedQuads;
            game.Keyboard.KeyDown += KeyboardOnKeyDown;
            game.Resize += GameOnResize;
            game.Run();
        }

        /// <summary> Loads a texture with the specified file name. </summary>
        /// <param name="kind"> What kind of a texture is it? </param>
        /// <param name="filename"> File to load. </param>
        private static void ChangeTexture(TextureKind kind, string filename)
        {
            _currentTexture?.Dispose();

            game.Title = "Loaded '" + Path.GetFileName(filename) + "'";
            _currentTexture = new SimpleTexture(kind, filename);
            _currentTexture.Enable();
        }

        // Initialize with simple orthographic projection, usual blending and first flat texture.
        private static void Init(object sender, EventArgs eventArgs)
        {
            GraphicsContext.Assert();
            GL.ClearColor(0.3f, 0.4f, 0.5f, 1f); // Surprisingly, it's a nice slate color.
            GL.Ortho(0, game.Width, 0, game.Height, -1, 1);
            GL.Viewport(0, 0, game.Width, game.Height);
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            _textureFiles = GetTextures(_currentTextureKind);
            if (_textureFiles.Length > 0) {
                ChangeTexture(_currentTextureKind, _textureFiles[0]);
            }

            game.Title =
                "Press keyboard arrows Up and Down to change texture type, press Left and Right to change textures.";
        }

        // Renders a single quad with the currently active texture. If texture loading failed, quad will be white.
        private static void RenderTexturedQuads(object sender, FrameEventArgs frameEventArgs)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit |
                     ClearBufferMask.StencilBufferBit);

            // Daily reminder: don't use immediate mode for anything more complex than this example :)
            GL.Begin(BeginMode.Quads);
            if (_currentTextureKind == TextureKind.Cubemap) // Draw all faces of a cubemap from PositiveX to NegativeZ:
            {
                const float size = InitialHeight / 3;
                for (var i = 0; i < 6; i++) {
                    var row = i / 3;
                    var col = i % 3;
                    DrawQuad(size + col * size, size + (1 - row) * size, size, cubemapTexcoords[i]);
                }
            }
            else // Draw a flat texture or the first slice from a 3D texture in the center of the screen:
            {
                DrawQuad(InitialWidth / 2f, InitialHeight / 2f, 400, quadTexcoords);
            }

            GL.End();
            game.SwapBuffers();
        }

        // Issues 4 vertices and texcoords for a typical quad.
        private static void DrawQuad(float posX, float posY, float size, Vector3[] texcoords)
        {
            GL.TexCoord3(texcoords[0]);
            GL.Vertex3(posX - size / 2, posY - size / 2, 0);
            GL.TexCoord3(texcoords[1]);
            GL.Vertex3(posX + size / 2, posY - size / 2, 0);
            GL.TexCoord3(texcoords[2]);
            GL.Vertex3(posX + size / 2, posY + size / 2, 0);
            GL.TexCoord3(texcoords[3]);
            GL.Vertex3(posX - size / 2, posY + size / 2, 0);
        }

        #region Standard texcoords

        private static readonly Vector3[] quadTexcoords = new[] {
            Vector3.Zero, Vector3.UnitX, new Vector3(1, 1, 0), Vector3.UnitY
        };

        private static readonly Vector3[] cubemapPosXTexcoords = new[] {
            new Vector3(1, 1, -1), new Vector3(1, 1, 1), new Vector3(1, -1, 1), new Vector3(1, -1, -1)
        };

        private static readonly Vector3[] cubemapNegXTexcoords = new[] {
            new Vector3(-1, 1, -1), new Vector3(-1, 1, 1), new Vector3(-1, -1, 1), new Vector3(-1, -1, -1)
        };

        private static readonly Vector3[] cubemapPosYTexcoords = new[] {
            new Vector3(1, 1, 1), new Vector3(1, 1, -1), new Vector3(-1, 1, -1), new Vector3(-1, 1, 1)
        };

        private static readonly Vector3[] cubemapNegYTexcoords = new[] {
            new Vector3(1, -1, -1), new Vector3(1, -1, 1), new Vector3(-1, -1, 1), new Vector3(-1, -1, -1)
        };

        private static readonly Vector3[] cubemapPosZTexcoords = new[] {
            new Vector3(1, 1, 1), new Vector3(-1, 1, 1), new Vector3(-1, -1, 1), new Vector3(1, -1, 1)
        };

        private static readonly Vector3[] cubemapNegZTexcoords = new[] {
            new Vector3(-1, 1, -1), new Vector3(1, 1, -1), new Vector3(1, -1, -1), new Vector3(-1, -1, -1)
        };

        private static readonly Vector3[][] cubemapTexcoords = new[] {
            cubemapPosXTexcoords, cubemapNegXTexcoords, cubemapPosYTexcoords, cubemapNegYTexcoords,
            cubemapPosZTexcoords, cubemapNegZTexcoords
        };

        #endregion Standard texcoords

        #region Unimportant auxiliary stuff

        private static readonly TextureKind[] _TextureKindValues = Enum.GetValues(typeof (TextureKind)) as TextureKind[];

        private static void KeyboardOnKeyDown(object sender, KeyboardKeyEventArgs keyboardKeyEventArgs)
        {
            if (keyboardKeyEventArgs.Key == Key.Left && _textureFiles.Length > 0) // Show previous texture:
            {
                _currentTextureIndex = WrapIndex(--_currentTextureIndex, _textureFiles.Length);
                ChangeTexture(_currentTextureKind, _textureFiles[_currentTextureIndex]);
            }
            else if (keyboardKeyEventArgs.Key == Key.Right && _textureFiles.Length > 0) // Show next texture:
            {
                _currentTextureIndex = WrapIndex(++_currentTextureIndex, _textureFiles.Length);
                ChangeTexture(_currentTextureKind, _textureFiles[_currentTextureIndex]);
            }
            else if (game.Keyboard[Key.Up]) // Show textures of next texture type:
            {
                // Let's not abuse LINQ like this:
                // currTextureType = textureKindValues.SkipWhile(e => e != currTextureType).Skip(1).First();

                // Advance texture type and get its texture files:
                var index = Array.IndexOf(_TextureKindValues, _currentTextureKind);
                _currentTextureKind = _TextureKindValues[WrapIndex(++index, _TextureKindValues.Length)];
                _textureFiles = GetTextures(_currentTextureKind);
                // Load first texture:
                if (_textureFiles.Length > 0) {
                    ChangeTexture(_currentTextureKind, _textureFiles[_currentTextureIndex]);
                }
            }
            else if (game.Keyboard[Key.Down]) // Show textures of previous texture type:
            {
                // Backpedal texture type and get its texture files:
                var index = Array.IndexOf(_TextureKindValues, _currentTextureKind);
                _currentTextureKind = _TextureKindValues[WrapIndex(--index, _TextureKindValues.Length)];
                _textureFiles = GetTextures(_currentTextureKind);
                // Load first texture:
                if (_textureFiles.Length > 0) {
                    ChangeTexture(_currentTextureKind, _textureFiles[_currentTextureIndex]);
                }
            }
        }

        /// <summary> Gets filenames of the textures with the specified type. </summary>
        /// <param name="textureType"> Texture type. </param>
        /// <returns> Array of texture filenames.</returns>
        private static string[] GetTextures(TextureKind textureType)
        {
            _currentTextureIndex = 0;
            switch (textureType) {
                case TextureKind.Flat:
                    return Directory.EnumerateFiles(_FlatTestDir, "*.dds").ToArray();
                case TextureKind.Cubemap:
                    return Directory.EnumerateFiles(_CubemapTestDir, "*.dds").ToArray();
                case TextureKind.Volume:
                    return Directory.EnumerateFiles(_VolumeTestDir, "*.dds").ToArray();
                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        ///     Makes sure that specified index stays in bounds for the given length by wrapping it around.
        /// </summary>
        /// <param name="index"> Index to wrap. </param>
        /// <param name="length"> Array length. </param>
        /// <returns> Wrapped index. </returns>
        private static int WrapIndex(int index, int length)
        {
            if (index > length - 1) {
                index = 0;
            }
            if (index < 0) {
                index = length - 1;
            }

            return index;
        }

        private static void GameOnResize(object sender, EventArgs eventArgs)
        {
            GL.Viewport(0, 0, game.Width, game.Height);
        }

        #endregion
    }
}
