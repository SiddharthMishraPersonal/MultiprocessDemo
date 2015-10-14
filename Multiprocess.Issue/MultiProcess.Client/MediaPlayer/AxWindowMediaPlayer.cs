// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AxWindowMediaPlayer.cs" company="Motorola Solutions">
//  For Demo Purpose 
// </copyright>
// <summary>
//   The play event handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiProcess.Client.MediaPlayer
{
    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Forms.Integration;

    using WmpFormsLib;

    using WMPLib;

    /// <summary>
    /// The ax window media player.
    /// </summary>
    public class AxWindowMediaPlayer : IMediaPlayer
    {
        /// <summary>
        /// Gets or sets the Window media player control.
        /// </summary>
        private readonly WmpControl windowMediaPlayerControl;

        /// <summary>
        ///   The player win form host.
        /// </summary>
        private WindowsFormsHost playerWinFormHost;

        /// <summary>
        /// The video URL.
        /// </summary>
        private string videoUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="AxWindowMediaPlayer"/> class.
        /// </summary>
        public AxWindowMediaPlayer()
        {
            try
            {
                this.windowMediaPlayerControl = new WmpControl();
                this.windowMediaPlayerControl.OnErrorOccurred += this.WmpControlOnOnErrorOccurred;
                this.windowMediaPlayerControl.OnPlayerStateChanged += this.WmpControlOnOnPlayerStateChanged;
            }
            catch (Exception exception)
            {
                this.PlayerObjectStreamingStatusChanged(ConnectionStatus.Interrupted, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// The streaming statistics updated.
        /// </summary>
        public event EventHandler<StreamingStaticsEventArgs> StreamingStatisticsUpdated;

        /// <summary>
        /// The streaming status changed.
        /// </summary>
        public event StreamingStatusEventHanlder StreamingStatusChanged;

        /// <summary>
        /// The selected.
        /// </summary>
        public event SelectEventHandler Selected;

        /// <summary>
        /// The drag start.
        /// </summary>
        public event DragStartEventHandler DragStart;

        /// <summary>
        /// The drop.
        /// </summary>
        public event DragEventHandler Drop;

        /// <summary>
        /// The player error.
        /// </summary>
        public event PlayerErrorEventHandler PlayerError;

        /// <summary>
        /// The play state changed.
        /// </summary>
        public event PlayStateChangedEventHandler PlayStateChanged;

        /// <summary>
        /// The play position changed.
        /// </summary>
        public event PlayPositionChangedEventHandler PlayPositionChanged;


        /// <summary>
        /// Gets the width.
        /// </summary>
        public double Width
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public double Height
        {
            get
            {
                return 0;
            }
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.windowMediaPlayerControl.Dispose();
        }

        /// <summary>
        /// The setup player object.
        /// </summary>
        /// <returns>
        /// The <see cref="UIElement"/>.
        /// </returns>
        public UIElement SetupPlayerObject()
        {
            this.playerWinFormHost = new WindowsFormsHost { Child = this.windowMediaPlayerControl };
            return this.playerWinFormHost;
        }

        /// <summary>
        /// The play.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Play()
        {
            var uri = new Uri(string.IsNullOrEmpty(this.videoUri) ? @"http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4" : this.videoUri);
            Trace.WriteLine(uri);
            this.windowMediaPlayerControl.Play(uri);
            return true;
        }

        /// <summary>
        /// The play.
        /// </summary>
        /// <param name="startTime">
        /// The start time.
        /// </param>
        /// <param name="endTime">
        /// The end time.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Play(DateTime startTime, DateTime endTime)
        {
            return true;
        }

        /// <summary>
        /// The pause.
        /// </summary>
        public void Pause()
        {
            this.windowMediaPlayerControl.Pause();
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.windowMediaPlayerControl.Stop();
        }

        /// <summary>
        /// The take snapshot.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        public void TakeSnapshot(string fileName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The exit.
        /// </summary>
        /// <param name="retainProcessHost">
        /// The retain process host.
        /// </param>
        public void Exit(bool retainProcessHost = false)
        {
            this.windowMediaPlayerControl.Exit();
            this.windowMediaPlayerControl.Dispose();
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="uri">
        /// The URL.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Initialize(string uri)
        {
            this.videoUri = uri;
            return true;
        }

        /// <summary>
        /// The _player object_ streaming status changed.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="errorCode">
        /// The errorCode.
        /// </param>
        private void PlayerObjectStreamingStatusChanged(ConnectionStatus status, string errorCode)
        {
            if (this.StreamingStatusChanged != null)
            {
                this.StreamingStatusChanged(status, errorCode);
            }
        }


        /// <summary>
        /// The Window Media Player control on on player state changed.
        /// </summary>
        /// <param name="newState">
        /// The new state.
        /// </param>
        private void WmpControlOnOnPlayerStateChanged(WMPPlayState newState)
        {
            switch (newState)
            {
                case WMPPlayState.wmppsTransitioning:
                    this.PlayerObjectStreamingStatusChanged(ConnectionStatus.Idle, string.Empty);
                    break;
                case WMPPlayState.wmppsPlaying:
                    this.PlayerObjectStreamingStatusChanged(ConnectionStatus.Streaming, string.Empty);
                    break;
                case WMPPlayState.wmppsBuffering:
                    this.PlayerObjectStreamingStatusChanged(ConnectionStatus.Streaming, string.Empty);
                    break;
                case WMPPlayState.wmppsWaiting:
                    this.PlayerObjectStreamingStatusChanged(ConnectionStatus.Streaming, string.Empty);
                    break;
                default:
                    this.PlayerObjectStreamingStatusChanged(ConnectionStatus.Streaming, string.Empty);
                    break;
            }
        }

        /// <summary>
        /// The Window Media Player control on on error occurred.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        private void WmpControlOnOnErrorOccurred(Exception exception)
        {
            this.PlayerObjectStreamingStatusChanged(ConnectionStatus.LongInterruption, exception.Message);
        }
    }
}
