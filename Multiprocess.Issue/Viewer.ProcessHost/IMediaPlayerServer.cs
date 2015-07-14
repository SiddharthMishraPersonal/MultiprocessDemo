// -----------------------------------------------------------------------
// <copyright file="IMediaPlayerServer.cs" company="Motorola Solutions, Inc.">
//   Copyright (C) 2015 Motorola Solutions, Inc.
// </copyright>
// -----------------------------------------------------------------------

namespace Motorola.IVS.Client.Viewer.ProcessHost
{
    using System.ServiceModel;
    using System.Windows;

    using Motorola.IVS.Client.Viewer.ProcessHost.Communication;

    /// <summary>
    /// Interface for <see cref="MediaPlayerServer"/> class.
    /// </summary>
    public interface IMediaPlayerServer : IMediaPlayerServiceContract
    {
        /// <summary>
        /// Fired when the plugin/stream is being closed down.
        /// </summary>
        event MediaPlayerServer.ExitedEventHandler Exited;

        /// <summary>
        /// Gets the HostForm containing the hosted media player.
        /// </summary>
        Window HostForm { get; }

        /// <summary>
        /// Creates the <see cref="ServiceHost"/> and sets it to Open state.
        /// </summary>
        /// <returns>The URI of the service.</returns>
        string StartListening();
    }
}