// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExitedEventArgs.cs" company="Motorola Solutions, Inc.">
//   Copyright (C) 2015 Motorola Solutions, Inc.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Motorola.IVS.Client.Viewer.ProcessHost.EventArgs
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Event args for Exited event.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ExitedEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExitedEventArgs"/> class.
        /// </summary>
        /// <param name="retainProcessHost">
        /// Whether to retain the ProcessHost even if this is the last media player instance.
        /// </param>
        public ExitedEventArgs(bool retainProcessHost)
        {
            this.RetainProcessHost = retainProcessHost;
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether to retain the ProcessHost even if this is the last media player instance.
        /// </summary>
        public bool RetainProcessHost { get; set; }
    }
}
