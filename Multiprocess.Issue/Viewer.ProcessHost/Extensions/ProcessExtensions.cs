// -----------------------------------------------------------------------
// <copyright file="ProcessExtensions.cs" company="Motorola Solutions, Inc.">
//   Copyright (C) 2015 Motorola Solutions, Inc.
// </copyright>
// -----------------------------------------------------------------------

namespace Motorola.IVS.Client.Viewer.ProcessHost.Extensions
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    using Motorola.IVS.Client.Viewer.ProcessHost.Structs;

    /// <summary>
    /// Extensions for the <see cref="Process"/> class.
    /// </summary>
    internal static class ProcessExtensions
    {
        /// <summary>
        /// Returns the <see cref="Process"/>'s parent process, if any.
        /// </summary>
        /// <param name="process">The child process.</param>
        /// <returns>The parent <see cref="Process"/>. NULL if none or error.</returns>
        public static Process GetParent(this Process process)
        {
            try
            {
                return GetParentProcess(process.Handle);
            }
            catch (Win32Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the <see cref="Process"/>'s parent process, if any.
        /// </summary>
        /// <param name="handle">An <see cref="IntPtr"/> handle to the child process.</param>
        /// <returns>The parent <see cref="Process"/>. NULL if none or error.</returns>
        private static Process GetParentProcess(IntPtr handle)
        {
            var pbi = new ProcessBasicInformation();
            int returnLength;
            var status = NativeMethods.NtQueryInformationProcess(handle, 0, ref pbi, Marshal.SizeOf(pbi), out returnLength);

            if (status != 0)
            {
                throw new Win32Exception(status);
            }

            try
            {
                return Process.GetProcessById(pbi.InheritedFromUniqueProcessId.ToInt32());
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
