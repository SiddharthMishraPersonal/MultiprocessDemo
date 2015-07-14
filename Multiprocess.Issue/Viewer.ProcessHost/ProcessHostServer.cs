// -----------------------------------------------------------------------
// <copyright file="ProcessHostServer.cs" company="Motorola Solutions, Inc.">
//   Copyright (C) 2015 Motorola Solutions, Inc.
// </copyright>
// -----------------------------------------------------------------------

namespace Motorola.IVS.Client.Viewer.ProcessHost
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.ServiceModel;
    using System.Timers;
    using System.Windows;

    using Motorola.IVS.Client.Viewer.ProcessHost.Communication;
    using Motorola.IVS.Client.Viewer.ProcessHost.EventArgs;

    /// <summary>
    /// A process used to host one or more MediaPlayerServers, separate from the spawning parent process.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ProcessHostServer : IProcessHostServiceContract
    {
        /// <summary>
        /// The active Media Players in this process.
        /// </summary>
        private readonly Dictionary<string, MediaPlayerServer> mediaPlayers = new Dictionary<string, MediaPlayerServer>();

        /// <summary>
        /// Timer to shutdown the process if no subsequent call is made to start a media player.
        /// </summary>
        private readonly Timer initTimer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessHostServer"/> class.
        /// </summary>
        public ProcessHostServer()
        {
            // If this process is started, then the associated RIC video pane closed before GetMediaPlayerProxyChannelUri() can be called,
            // this will ensure this process shuts itself down, as RIC will never do so.
            this.initTimer = new Timer(10000);
            this.initTimer.Elapsed += (sender, args) =>
                {
                    if (Application.Current != null)
                    {
                        Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
                    }
                };
            this.initTimer.Start();
        }

        /// <summary>
        /// Spawns a new MediaPlayerServer instance and returns a URI for it.
        /// </summary>
        /// <returns>The URI for communication with the new MediaPlayerServer instance.</returns>
        public string GetMediaPlayerProxyChannelUri()
        {
            this.initTimer.Stop();

            if (Application.Current.Dispatcher.CheckAccess() == false)
            {
                return Application.Current.Dispatcher.Invoke(() => this.GetMediaPlayerProxyChannelUri());
            }

            var playerServer = new MediaPlayerServer();
            playerServer.Exited += this.OnPlayerServerExited;
            
            var uri = playerServer.StartListening();
            this.mediaPlayers.Add(uri, playerServer);

            return uri;
        }

        /// <summary>
        /// Closes the specified Media Player.
        /// </summary>
        /// <param name="mediaPlayerUri">The URI of the player to close.</param>
        /// <param name="retainProcess">Whether to leave this process running even if all players are closed.</param>
        public void CloseMediaPlayer(string mediaPlayerUri, bool retainProcess)
        {
            if (!string.IsNullOrWhiteSpace(mediaPlayerUri))
            {
                var player = this.mediaPlayers[mediaPlayerUri];
                if (player != null)
                {
                    this.mediaPlayers[mediaPlayerUri].Exit();
                    this.mediaPlayers[mediaPlayerUri] = null;
                }

                this.mediaPlayers.Remove(mediaPlayerUri);

                if (!this.mediaPlayers.Any() && !retainProcess)
                {
                    Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
                }
            }
        }

        /// <summary>
        /// Handles a MediaPlayerServer shutting down.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">
        /// Whether to retain this ProcessHostServer instance even if this event causes the mediaPlayerServerCount to reach zero.
        /// Generally only true during a Back10 operation from live to recorded.
        /// </param>
        private void OnPlayerServerExited(object sender, ExitedEventArgs e)
        {
            ((MediaPlayerServer)sender).Exited -= this.OnPlayerServerExited;
            this.mediaPlayers.Remove(((MediaPlayerServer)sender).ListeningAddress);

            if (!this.mediaPlayers.Any() && !e.RetainProcessHost)
            {
                Application.Current.Dispatcher.Invoke(() => Application.Current.Shutdown());
            }
        }
    }
}