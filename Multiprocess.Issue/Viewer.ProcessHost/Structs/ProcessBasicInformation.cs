// -----------------------------------------------------------------------
// <copyright file="ProcessBasicInformation.cs" company="Motorola Solutions, Inc.">
//   Copyright (C) 2015 Motorola Solutions, Inc.
// </copyright>
// -----------------------------------------------------------------------

namespace Motorola.IVS.Client.Viewer.ProcessHost.Structs
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains basic process information for use by <see cref="NativeMethods"/>.
    /// </summary>
    /// <remarks>
    /// See <a href="https://msdn.microsoft.com/en-us/library/windows/desktop/ms684280(v=vs.85).aspx#PROCESS_BASIC_INFORMATION">MSDN entry here</a>.
    /// </remarks>
    [ExcludeFromCodeCoverage]
    [StructLayout(LayoutKind.Sequential)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1606:ElementDocumentationMustHaveSummaryText", Justification = "Native struct, see remarks.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Native struct, see remarks.")]
    internal struct ProcessBasicInformation
    {
        public IntPtr ExitStatus;
        public IntPtr PebBaseAddress;
        public IntPtr AffinityMask;
        public IntPtr BasePriority;
        public UIntPtr UniqueProcessId;
        public IntPtr InheritedFromUniqueProcessId;
    }
}