using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multiprocess.Issue.HostedContents
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
    }
}
