// -----------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Motorola Solutions, Inc.">
//   Copyright (C) 2015 Motorola Solutions, Inc.
// </copyright>
// -----------------------------------------------------------------------

namespace Motorola.IVS.Client.Viewer.ProcessHost
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    using Motorola.IVS.Client.Viewer.ProcessHost.Structs;

    /// <summary>
    /// Contains native windows methods.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal static class NativeMethods
    {
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
        /// If the function fails, the return value is NULL.
        /// </returns>
        [DllImport("user32")]
        internal static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        /// <summary>
        /// External call to show the window in different states.
        /// </summary>
        /// <param name="hWnd">
        /// The handle For Window.
        /// </param>
        /// <param name="nCmdShow">
        /// The command to show window
        /// </param>
        /// <returns>
        /// If the window was previously visible, the return value is nonzero.
        /// If the window was previously hidden, the return value is zero.
        /// </returns>
        [DllImport("user32")]
        internal static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("ntdll.dll")]
        internal static extern int NtQueryInformationProcess(IntPtr processHandle, int processInformationClass, ref ProcessBasicInformation processInformation, int processInformationLength, out int returnLength);
    }
}
