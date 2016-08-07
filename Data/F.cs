using System;
using System.Drawing;
using System.Windows.Forms;

namespace FitWinN {

    class F {

        public const int BorderWidth = 2, CaptionHeight = 28;
        public const string Text = "Fit Win", Version = "0.3.20140712";
        public static readonly string StartupPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + "\\" + Text + ".lnk";
        public static Data Data;

        public static readonly
            Color BackColor = SystemColors.Control,
            BorderColor = Color.FromArgb(64, 96, 128),
            CaptionColor = Color.FromArgb(172, 201, 230),

            HoverColor = Color.FromArgb(187, 219, 250),
            HoverBorderColor = Color.FromArgb(0, 125, 250),

            TemplateHoverBorderColor = Color.Black,
            HAHColor = Color.FromArgb(191, 255, 128);

        public static int WindowWidth(Control c) {
            return c.TopLevelControl.ClientSize.Width - BorderWidth * 2;
        }

        public static int NoScrollWidth(Control c) {
            return WindowWidth(c) - SystemInformation.HorizontalScrollBarThumbWidth;
        }

        public static Rectangle WorkingArea {
            get {
                return Data.MultiVisible ? Multi.Window.WorkingArea : Screen.PrimaryScreen.WorkingArea;
            }
        }

        public class Multi {

            public static MultiWindow Window = null;

            public static int Height {
                get {
                    return Math.Max(2, Data.MultiHeight);
                }
            }

            public static Size WindowSize(Control c) {
                return new Size(WindowWidth(c), Height);
            }
        }

        public class Editor {

            public const int MinWidth = 32;
            public static FitWinN.Editor Root = null;

            public static int Width {
                get {
                    return Height * WorkingArea.Width / WorkingArea.Height;
                }
            }

            public static int Height {
                get {
                    return Math.Max(2, Data.EditorHeight);
                }
            }

            public static Size Size {
                get {
                    return new Size(Width, Height);
                }
            }

            public static int BorderWidth {
                get {
                    return Math.Max(1, Data.EditorBorderWidth);
                }
            }

            public static Size WindowSize(Control c) {
                return new Size(WindowWidth(c), Height);
            }
        }

        public class Task {

            public const int Width = 48, BorderWidth = 8, Rows = 7;
            public static readonly Size Size = new Size(Width, Width);

            public static int Lines {
                get {
                    return Math.Max(1, Data.TaskLines);
                }
            }

            public static int WindowHeight {
                get {
                    return Width * Lines + BorderWidth * (Lines + 1);
                }
            }

            public static Size WindowSize(Control c) {
                return new Size(WindowWidth(c), WindowHeight);
            }

            public static int Left(int width, int rows, int ix) {
                return (width - Width * rows) / 2 + ix % Rows * Width;
            }

            public static int Top(int ix) {
                return ix / Rows * (Width + BorderWidth) + BorderWidth;
            }

            public static Point Location(int width, int rows, int ix) {
                return new Point(Left(width, rows, ix), Top(ix));
            }
        }

        public class Template {

            public const int BorderWidth = 6, BorderHeight = 12, Rows = 7;
            public static readonly Padding Padding =
                new Padding(BorderWidth, BorderHeight, BorderWidth, BorderHeight);
            public static TemplateWindow Window = null;

            public static int Area {
                get {
                    return Math.Max(2, Data.TemplateArea);
                }
            }

            public static int Width {
                get {
                    return (int)Math.Sqrt(Area * WorkingArea.Width / WorkingArea.Height);
                }
            }

            public static int Height {
                get {
                    return (int)Math.Sqrt(Area * WorkingArea.Height / WorkingArea.Width);
                }
            }

            public static Size Size {
                get {
                    return new Size(Width, Height);
                }
            }

            public static int FrameHeight(int height) {
                return F.BorderWidth + CaptionHeight + height;
            }

            public static int WindowHeight(Control c) {
                return c.TopLevelControl.ClientSize.Height - (Data.EditorVisible ? FrameHeight(Editor.Height) : 0) -
                    (Data.MultiVisible ? FrameHeight(Multi.Height) : 0) - FrameHeight(Task.WindowHeight) - CaptionHeight - F.BorderWidth * 2;
            }

            public static Size WindowSize(Control c) {
                return new Size(WindowWidth(c), WindowHeight(c));
            }

            public static int WindowLeft(Control c) {
                return Math.Max(BorderWidth, (NoScrollWidth(c) - (Width + BorderWidth * 2 + F.BorderWidth * 2) * Rows) / 2);
            }

            public static Padding WindowPadding(Control c) {
                return new Padding(WindowLeft(c), BorderHeight, WindowLeft(c), 0);
            }

            public static Padding TailMargin(Control c) {
                return new Padding(NoScrollWidth(c), BorderHeight * 2, 0, 0);
            }
        }

        public class HAH {

            public static readonly Font Font = new Font("Consolas", 12);
            public static readonly int FontHeight = TextRenderer.MeasureText("A", Font).Height;

            public static int Kind {
                get {
                    return Math.Min(Math.Max(0, Data.HAHKind), 1);
                }
            }

            public static Point Location(Control c, int dx, int dy) {
                Point p = c.TopLevelControl.Location, q = c.PointToScreen(new Point());
                p.Offset(dx - q.X, dy - q.Y + SystemInformation.MenuHeight);
                return p;
            }
        }
    }
}
