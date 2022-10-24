using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Foregrounder;

internal static class User32 {

    private const string Dll = "user32.dll";

    [DllImport(Dll)]
    internal static extern IntPtr GetForegroundWindow();

    [DllImport(Dll)]
    internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr processId);

    [DllImport(Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

    [DllImport(Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport(Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetWindowPlacement(IntPtr hWnd, ref WindowPlacement lpwndpl);

    [DllImport(Dll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WindowPlacement lpwndpl);

    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowPlacement {

        /// <summary>
        /// The length of the structure, in bytes. Before calling the GetWindowPlacement or SetWindowPlacement functions, set this member to sizeof(WINDOWPLACEMENT).
        /// <para>
        /// GetWindowPlacement and SetWindowPlacement fail if this member is not set correctly.
        /// </para>
        /// </summary>
        public int Length;

        /// <summary>
        /// Specifies flags that control the position of the minimized window and the method by which the window is restored.
        /// </summary>
        public int Flags;

        /// <summary>
        /// The current show state of the window.
        /// </summary>
        public ShowWindowCommands ShowCmd;

        /// <summary>
        /// The coordinates of the window's upper-left corner when the window is minimized.
        /// </summary>
        public Point MinPosition;

        /// <summary>
        /// The coordinates of the window's upper-left corner when the window is maximized.
        /// </summary>
        public Point MaxPosition;

        /// <summary>
        /// The window's coordinates when the window is in the restored position.
        /// </summary>
        public Rect NormalPosition;

        /// <summary>
        /// Gets the default (empty) value.
        /// </summary>
        public static WindowPlacement Create() {
            WindowPlacement result = new();
            result.Length = Marshal.SizeOf(result);
            return result;
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect {

        public int Left, Top, Right, Bottom;

        public Rect(int left, int top, int right, int bottom) {
            Left   = left;
            Top    = top;
            Right  = right;
            Bottom = bottom;
        }

        public Rect(Rectangle r): this(r.Left, r.Top, r.Right, r.Bottom) { }

        public int X {
            get => Left;
            set {
                Right -= Left - value;
                Left  =  value;
            }
        }

        public int Y {
            get => Top;
            set {
                Bottom -= Top - value;
                Top    =  value;
            }
        }

        public int Height {
            get => Bottom - Top;
            set => Bottom = value + Top;
        }

        public int Width {
            get => Right - Left;
            set => Right = value + Left;
        }

        public System.Drawing.Point Location {
            get => new(Left, Top);
            set {
                X = value.X;
                Y = value.Y;
            }
        }

        public Size Size {
            get => new(Width, Height);
            set {
                Width  = value.Width;
                Height = value.Height;
            }
        }

        public static implicit operator Rectangle(Rect r) {
            return new Rectangle(r.Left, r.Top, r.Width, r.Height);
        }

        public static implicit operator Rect(Rectangle r) {
            return new Rect(r);
        }

        public static bool operator ==(Rect r1, Rect r2) {
            return r1.Equals(r2);
        }

        public static bool operator !=(Rect r1, Rect r2) {
            return !r1.Equals(r2);
        }

        public bool Equals(Rect r) {
            return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
        }

        public override bool Equals(object? obj) {
            return obj switch {
                Rect rect           => Equals(rect),
                Rectangle rectangle => Equals(new Rect(rectangle)),
                _                   => false
            };

        }

        public override int GetHashCode() {
            return ((Rectangle) this).GetHashCode();
        }

        public override string ToString() {
            return string.Format(CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Point {

        public readonly int X;
        public readonly int Y;

        public Point(int x, int y) {
            X = x;
            Y = y;
        }

        public static implicit operator System.Drawing.Point(Point p) {
            return new System.Drawing.Point(p.X, p.Y);
        }

        public static implicit operator Point(System.Drawing.Point p) {
            return new Point(p.X, p.Y);
        }

    }

    internal enum ShowWindowCommands {

        /// <summary>
        /// Hides the window and activates another window.
        /// </summary>
        Hide = 0,

        /// <summary>
        /// Activates and displays a window. If the window is minimized or
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when displaying the window
        /// for the first time.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Activates the window and displays it as a minimized window.
        /// </summary>
        ShowMinimized = 2,

        /// <summary>
        /// Maximizes the specified window.
        /// </summary>
        Maximize = 3, // is this the right value?

        /// <summary>
        /// Activates the window and displays it as a maximized window.
        /// </summary>      
        ShowMaximized = 3,

        /// <summary>
        /// Displays a window in its most recent size and position. This value
        /// is similar to <see cref="Normal"/>, except
        /// the window is not activated.
        /// </summary>
        ShowNoActivate = 4,

        /// <summary>
        /// Activates the window and displays it in its current size and position.
        /// </summary>
        Show = 5,

        /// <summary>
        /// Minimizes the specified window and activates the next top-level
        /// window in the Z order.
        /// </summary>
        Minimize = 6,

        /// <summary>
        /// Displays the window as a minimized window. This value is similar to
        /// <see cref="ShowMinimized"/>, except the
        /// window is not activated.
        /// </summary>
        ShowMinNoActive = 7,

        /// <summary>
        /// Displays the window in its current size and position. This value is
        /// similar to <see cref="Show"/>, except the
        /// window is not activated.
        /// </summary>
        ShowNA = 8,

        /// <summary>
        /// Activates and displays the window. If the window is minimized or
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when restoring a minimized window.
        /// </summary>
        Restore = 9,

        /// <summary>
        /// Sets the show state based on the SW_* value specified in the
        /// STARTUPINFO structure passed to the CreateProcess function by the
        /// program that started the application.
        /// </summary>
        ShowDefault = 10,

        /// <summary>
        ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
        /// that owns the window is not responding. This flag should only be
        /// used when minimizing windows from a different thread.
        /// </summary>
        ForceMinimize = 11

    }

}