using System;
using System.Collections.Generic;
using System.Text;
using Sandbox.ModAPI.Ingame;
using VRage.Game.GUI.TextPanel;
using VRageMath;

namespace IngameScript
{
    /// <summary>
    /// Provides a console-like interface for displaying text on a programmable block's text surface.
    /// </summary>
    public static class Console
    {
        static IMyTextSurface _surface;
        static DataClass _data;
        static int _linesSincePrompt;
        
        /// <summary>
        /// Provides an interface for output operations.
        /// </summary>
        public static Output Out => new Output();

        /// <summary>
        /// Installs the console on the specified program's text surface.
        /// </summary>
        /// <param name="program">The program whose text surface will be used.</param>
        /// <returns>A disposable handle for managing the console lifecycle.</returns>
        public static IDisposable Install(Program program)
        {
            _surface = program.Me.GetSurface(0);
            _surface.ContentType = ContentType.SCRIPT;
            _surface.Script = "";
            _surface.FontSize = 0.5f;
            _surface.ScriptBackgroundColor = Color.Black;
            _surface.ScriptForegroundColor = new Color(64, 64, 64);
            if (_data == null)
            {
                _data = new DataClass();
                _data.Em = _surface.MeasureStringInPixels(new StringBuilder("M"), "Monospace", 0.5f);
                _data.MaxLineCount = (int)(_surface.SurfaceSize.Y / _data.Em.Y) - 2;
            }
            return Handle.Default;
        }

        /// <summary>
        /// Disposes the console and renders text to the surface as sprites.
        /// </summary>
        public static void Dispose()
        {
            var frame = _surface.DrawFrame();
            try
            {
                var lineCount = _data.Lines.Count;
                var offset = (_surface.TextureSize - _surface.SurfaceSize) / 2;
                var y = _data.Em.Y + offset.Y;
                var x = _data.Em.X + offset.X;
                var width = _surface.SurfaceSize.X - _data.Em.X * 2;
                var height = _surface.SurfaceSize.Y - _data.Em.Y * 2;
                frame.Add(MySprite.CreateClipRect(new Rectangle((int)x, (int)y, (int)width, (int)height)));
                for (var i = 0; i < lineCount; i++)
                {
                    var text = _data.Lines[i];
                    if (text == "---")
                    {
                        var lineBox = new RectangleF(x, y + _data.Em.Y / 2, width, 1);
                        var line = MySprite.CreateSprite("SquareSimple", lineBox.Center, lineBox.Size);
                        line.Color = _surface.ScriptForegroundColor;
                        frame.Add(line);
                        y += _data.Em.Y;
                        continue;
                    }
                    var sprite = MySprite.CreateText(text, "Monospace", _surface.ScriptForegroundColor, 0.5f, TextAlignment.LEFT);
                    sprite.Position = new Vector2(x, y);
                    frame.Add(sprite);
                    y += _data.Em.Y;
                }
            }
            finally
            {
                frame.Dispose();
            }
            _surface = null;
        }

        /// <summary>
        /// Provides methods for outputting text and manipulating console output.
        /// </summary>
        public struct Output
        {
            /// <summary>
            /// Removes the last line added to the console.
            /// </summary>
            /// <returns>The Output instance for chaining.</returns>
            public Output PopLine()
            {
                _linesSincePrompt++;
                if (_data.Lines.Count > 0)
                    _data.Lines.RemoveAt(_data.Lines.Count - 1);
                return this;
            }

            /// <summary>
            /// Clears all the lines from the console.
            /// </summary>
            /// <returns>The Output instance for chaining.</returns>
            public Output Clear()
            {
                _linesSincePrompt++;
                _data.Lines.Clear();
                return this;
            }

            /// <summary>
            /// Prints the specified text to the console.
            /// </summary>
            /// <param name="text">The text to print.</param>
            /// <returns>The Output instance for chaining.</returns>
            public Output Print(string text)
            {
                _linesSincePrompt++;
                var newlineIndex = text.IndexOf('\n');
                var firstIndex = 0;
                while (newlineIndex >= 0)
                {
                    var line = text.Substring(firstIndex, newlineIndex - firstIndex);
                    AddLine(line);
                    firstIndex = newlineIndex + 1;
                    newlineIndex = text.IndexOf('\n', firstIndex);
                }
                if (firstIndex < text.Length)
                {
                    var line = firstIndex > 0 ? text.Substring(firstIndex) : text;
                    AddLine(line);
                }
                return new Output();
            }

            /// <summary>
            /// Displays a prompt on the console, optionally with a command.
            /// </summary>
            /// <param name="cmd">The command to display with the prompt.</param>
            /// <returns>The Output instance for chaining.</returns>
            public Output Prompt(string cmd = null)
            {
                if (_linesSincePrompt == 0)
                    PopLine();
                if (string.IsNullOrWhiteSpace(cmd))
                    Print("> _");
                else
                    Print($"> {cmd}");
                _linesSincePrompt = 0;
                return this;
            }
        }

        static void AddLine(string text)
        {
            if (_data.Lines.Count >= _data.MaxLineCount)
                _data.Lines.RemoveAt(0);
            _data.Lines.Add(text);
        }

        class Handle : IDisposable
        {
            public static readonly Handle Default = new Handle();
            public void Dispose() => Console.Dispose();
        }

        class DataClass
        {
            public Vector2 Em;
            public readonly List<string> Lines = new List<string>();
            public int MaxLineCount;
        }
    }
}