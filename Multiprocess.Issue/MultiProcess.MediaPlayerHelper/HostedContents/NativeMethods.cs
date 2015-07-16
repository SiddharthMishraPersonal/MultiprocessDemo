using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiProcess.MediaPlayerHelper.HostedContents
{
    using System.Runtime.InteropServices;

    internal static class NativeMethods
    {
        #region Window sizing and positioning constants

        /// <summary>
        /// Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed.
        /// If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
        /// </summary>
        public const uint SWP_FRAMECHANGED = 0x0020;

        /// <summary>
        /// Displays the window.
        /// </summary>
        public const uint SWP_SHOWWINDOW = 0x0040;

        /// <summary>
        /// Hides the window.
        /// </summary>
        public const uint SwpHidewindow = 0x0080;

        /// <summary>
        /// Retains the current size (ignores the Width and Height parameters).
        /// </summary>
        public const uint SWP_NOSIZE = 0x0001;

        /// <summary>
        /// Retains the current position (ignores the Left and Top parameters).
        /// </summary>
        public const uint SWP_NOMOVE = 0x0002;

        /// <summary>
        /// Retains the current Z order (ignores the hWindowInsertAfter parameter).
        /// </summary>
        public const uint SWP_NOZORDER = 0x0004;

        /// <summary>
        /// Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to the client area.
        /// </summary>
        public const uint SWP_NOREDRAW = 0x0008;

        /// <summary>
        /// Does not activate the window. If this flag is not set, the window is activated and moved to the top of either the topmost
        /// or non-topmost group (depending on the setting of the "hWindowInsertAfter" parameter).
        /// </summary>
        public const uint SWP_NOACTIVATE = 0x0010;

        /// <summary>
        /// Discards the entire contents of the client area. If this flag is not specified,
        /// the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.
        /// </summary>
        public const uint SWP_NOCOPYBITS = 0x0100;

        /// <summary>
        /// Does not change the owner window's position in the Z order.
        /// </summary>
        public const uint SWP_NOOWNERZORDER = 0x0200;

        /// <summary>
        /// Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
        /// </summary>
        public const uint SWP_NOSENDCHANGING = 0x0400;

        #endregion

        /// <summary>
        /// Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered according to their appearance on the screen. The topmost window receives the highest rank and is the first window in the Z order.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window.
        /// </param>
        /// <param name="hWndInsertAfter">
        /// A handle to the window to precede the positioned window in the Z order. 
        /// This parameter must be a window handle or one of the values noted for this parameter on <a href="view-source:msdn.microsoft.com/en-us/library/windows/desktop/ms633545(v=vs.85).aspx">MSDN</a>.
        /// </param>
        /// <param name="X">
        /// The new position of the left side of the window, in client coordinates.
        /// </param>
        /// <param name="Y">
        /// The new position of the top of the window, in client coordinates.
        /// </param>
        /// <param name="cx">
        /// The new width of the window, in pixels.
        /// </param>
        /// <param name="cy">
        /// The new height of the window, in pixels.
        /// </param>
        /// <param name="uFlags">
        /// The window sizing and positioning flags. This parameter can be a combination of the values noted for this parameter on <a href="view-source:msdn.microsoft.com/en-us/library/windows/desktop/ms633545(v=vs.85).aspx">MSDN</a>.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is NULL. To get extended error information, call <see cref="GetLastError"/>.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        //// ReSharper disable InconsistentNaming

        #region Window Styling Constants

        /// <summary>
        /// Sets a new window style.
        /// </summary>
        public const int GWL_STYLE = -16;

        /// <summary>
        /// The window is an overlapped window. An overlapped window has a title bar and a border. Same as the <see cref="WS_TILED"/> style.
        /// </summary>
        public const uint WS_OVERLAPPED = 0x00000000;

        /// <summary>
        /// The windows is a pop-up window. This style cannot be used with the <see cref="WS_CHILD"/> style.
        /// </summary>
        public const uint WS_POPUP = 0x80000000;

        /// <summary>
        /// The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the <see cref="WS_POPUP"/> style.
        /// </summary>
        public const uint WS_CHILD = 0x40000000;

        /// <summary>
        /// The window is initially minimized. Same as the <see cref="WS_ICONIC"/> style.
        /// </summary>
        public const uint WS_MINIMIZE = 0x20000000;

        /// <summary>
        /// The window is initially visible.
        /// </summary>
        public const uint WS_VISIBLE = 0x10000000;

        /// <summary>
        /// The window is initially disabled. A disabled window cannot receive input from the user.
        /// </summary>
        public const uint WS_DISABLED = 0x08000000;

        /// <summary>
        /// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message,
        /// the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated.
        /// If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a
        /// child window, to draw within the client area of a neighboring child window.
        /// </summary>
        public const uint WS_CLIPSIBLINGS = 0x04000000;

        /// <summary>
        /// Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.
        /// </summary>
        public const uint WS_CLIPCHILDREN = 0x02000000;

        /// <summary>
        /// The window is initially maximized.
        /// </summary>
        public const uint WS_MAXIMIZE = 0x01000000;

        /// <summary>
        /// The window has a title bar (includes the <see cref="WS_BORDER"/> style).
        /// </summary>
        public const uint WS_CAPTION = 0x00C00000;

        /// <summary>
        /// The window has a thin-line border.
        /// </summary>
        public const uint WS_BORDER = 0x00800000;

        /// <summary>
        /// The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.
        /// </summary>
        public const uint WS_DLGFRAME = 0x00400000;

        /// <summary>
        /// The window has a vertical scroll bar.
        /// </summary>
        public const uint WS_VSCROLL = 0x00200000;

        /// <summary>
        /// The window has a horizontal scroll bar.
        /// </summary>
        public const uint WS_HSCROLL = 0x00100000;

        /// <summary>
        /// The window has a window menu on its title bar. The WS_CAPTION style must also be specified.
        /// </summary>
        public const uint WS_SYSMENU = 0x00080000;

        /// <summary>
        /// The window has a sizing border. Same as the <see cref="WS_SIZEBOX"/> style.
        /// </summary>
        public const uint WS_THICKFRAME = 0x00040000;

        /// <summary>
        /// The window is the first control of a group of controls.
        /// </summary>
        public const uint WS_GROUP = 0x00020000;

        /// <summary>
        /// The window is a control that can receive the keyboard focus when the user presses the TAB key.
        /// </summary>
        public const uint WS_TABSTOP = 0x00010000;

        /// <summary>
        /// The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.
        /// </summary>
        public const uint WS_MINIMIZEBOX = 0x00020000;

        /// <summary>
        /// The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.
        /// </summary>
        public const uint WS_MAXIMIZEBOX = 0x00010000;

        /// <summary>
        /// The window is an overlapped window. An overlapped window has a title bar and a border. Same as the WS_OVERLAPPED style.
        /// </summary>
        public const uint WS_TILED = WS_OVERLAPPED;

        /// <summary>
        /// The window is initially minimized. Same as the WS_MINIMIZE style.
        /// </summary>
        public const uint WS_ICONIC = WS_MINIMIZE;

        /// <summary>
        /// The window has a sizing border. Same as the WS_THICKFRAME style.
        /// </summary>
        public const uint WS_SIZEBOX = WS_THICKFRAME;

        /// <summary>
        /// The window is intended to be used as a floating toolbar.
        /// </summary>
        public const int WS_EX_TOOLWINDOW = 0x00000080;

        #endregion

        /// <summary>
        /// Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
        /// </summary>
        public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

        /// <summary>
        /// Places the window above all non-topmost windows (that is, behind all topmost windows).
        /// This flag has no effect if the window is already a non-topmost window.
        /// </summary>
        public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

        /// <summary>
        /// Places the window at the top of the Z order.
        /// </summary>
        public static readonly IntPtr HWND_TOP = new IntPtr(0);

        /// <summary>
        /// Places the window at the bottom of the Z order. If the hWindow parameter identifies a topmost window
        /// , the window loses its topmost status and is placed at the bottom of all other windows.
        /// </summary>
        public static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        /// <summary>
        /// Changes an attribute of the specified window. The function also sets the 32-bit (long) value at the specified offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window and, indirectly, the class to which the window belongs.
        /// </param>
        /// <param name="nIndex">
        /// The zero-based offset to the value to be set. Valid values are in the range zero through the number of bytes of extra window memory, minus the size of an integer. To set any other value, specify one of the following values.
        /// </param>
        /// <param name="dwNewLong">
        /// The replacement value.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the previous value of the specified 32-bit integer.
        /// If the function fails, the return value is zero. To get extended error information, call <see cref="GetLastError"/>.
        /// </returns>
        //// TODO: This needs to be replaces with a call to SetWindowLongPtr for 64-bit compatibility. See example at http://www.pinvoke.net/default.aspx/user32.SetWindowLongPtr.
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        /// <summary>
        /// Retrieves information about the specified window. The function also retrieves the 32-bit (DWORD) value at the specified offset into the extra window memory.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window and, indirectly, the class to which the window belongs.
        /// </param>
        /// <param name="nIndex">
        /// The zero-based offset to the value to be retrieved. Valid values are in the range zero through the number of bytes of extra window memory, minus four; for example, if you specified 12 or more bytes of extra memory, a value of 8 would be an index to the third 32-bit integer. To retrieve any other value, specify one of the following values.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is the requested value.
        /// If the function fails, the return value is zero. To get extended error information, call <see cref="GetLastError" />.
        /// If SetWindowLong has not been called previously, <see cref="GetWindowLong"/> returns zero for values in the extra window or class memory.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        //// TODO: This needs to be replaces with a call to GetWindowLongPtr for 64-bit compatibility. See example at http://www.pinvoke.net/default.aspx/user32.GetWindowLongPtr.
        internal static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        /// <summary>
        /// Changes the parent window of the specified child window.
        /// </summary>
        /// <param name="hWndChild">
        /// A handle to the child window.
        /// </param>
        /// <param name="hWndNewParent">
        /// A handle to the new parent window. If this parameter is NULL, the desktop window becomes the new parent window. If this parameter is HWND_MESSAGE, the child window becomes a message-only window.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the previous parent window.
        /// If the function fails, the return value is NULL. To get extended error information, call <see cref="GetLastError"/>.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);    
        

        /// <summary>
        /// Retrieves the calling thread's last-error code value. The last-error code is maintained on a per-thread basis. Multiple threads do not overwrite each other's last-error code.
        /// </summary>
        /// <returns>The return value is the calling thread's last-error code.</returns>
        [DllImport("kernel32.dll")]
        internal static extern uint GetLastError();

        /// <summary>
        /// Retrieves the dimensions of the bounding rectangle of the specified window
        /// </summary>
        /// <param name="windowHandle">The window handle whose rectangle is required</param>
        /// <param name="windowRect">A pointer to a RECT structure that receives the screen coordinates of the upper-left and lower-right corners of the window.</param>
        /// <returns>If the function succeeds, the return value is nonzero, else the return value is zero.</returns>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowRect(IntPtr windowHandle, out Rect windowRect);

        /// <summary>
        /// External call to the user32 library to set the current window to foreground
        /// </summary>
        /// <param name="handleForWindow">
        /// The handle For Window.
        /// </param>
        /// <returns>
        /// A boolean value
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr handleForWindow);

        /// <summary>
        /// External call to show the window in different states.
        /// </summary>
        /// <param name="handleForWindow">
        /// The handle For Window.
        /// </param>
        /// <param name="command">
        /// The command to show window
        /// </param>
        /// <returns>
        /// A boolean value
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr handleForWindow, int command);

        /// <summary>
        /// Attaches or detaches the input processing mechanism of one thread to that of another thread.
        /// </summary>
        /// <param name="idAttach">
        /// The identifier of the thread to be attached to another thread. The thread to be attached cannot be a system thread.
        /// </param>
        /// <param name="idAttachTo">
        /// The identifier of the thread to which idAttach will be attached. This thread cannot be a system thread. 
        /// A thread cannot attach to itself. Therefore, idAttachTo cannot equal idAttach.
        /// </param>
        /// <param name="fAttach">
        /// If this parameter is TRUE, the two threads are attached. If the parameter is FALSE, the threads are detached.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. 
        /// If the function fails, the return value is zero. To get extended error information, call <see cref="GetLastError"/>.
        /// </returns>
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        /// <summary>
        /// Retrieves the identifier of the thread that created the specified window and, optionally, the identifier of the process that created the window.
        /// </summary>
        /// <param name="hWnd">
        /// A handle to the window.
        /// </param>
        /// <param name="ProcessId">
        /// A pointer to a variable that receives the process identifier.
        /// If this parameter is not NULL, GetWindowThreadProcessId copies the identifier of the process to the variable; otherwise, it does not.
        /// </param>
        /// <returns>
        /// The return value is the identifier of the thread that created the window.
        /// </returns>
        [DllImport("user32.dll")]
        internal static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

        /// <summary>
        /// Defines the coordinates of the upper-left and lower-right corners of a rectangle.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Rect
        {
            /// <summary>
            /// The x-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public int Left;

            /// <summary>
            /// The y-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public int Top;

            /// <summary>
            /// The x-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public int Right;

            /// <summary>
            /// The y-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public int Bottom;
        }
        // ReSharper restore InconsistentNaming
    }
}
