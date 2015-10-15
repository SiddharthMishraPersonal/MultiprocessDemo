// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediaPlayer.cs" company="Motorola Solutions inc">
//   Demo purpose only.
// </copyright>
// <summary>
//   Defines the VlcMediaPlayer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace MultiProcess.Client.MediaPlayer
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Forms.Integration;

    using MultiProcess.Client.EventArg;

    using Vlc.DotNet.Wpf;

    public class VlcMediaPlayer : IMediaPlayer
    {
        /// <summary>
        ///   The _player win form host.
        /// </summary>
        private WindowsFormsHost _playerWinFormHost;

        private string videoUri;


        public VlcControl VlcControl
        {
            get;
            set;
        }

        public VlcMediaPlayer()
        {
            try
            {
                if (!Debugger.IsAttached)
                {
                    Debugger.Launch();
                }

                var control = new VlcControl();
                control.MediaPlayer.VlcLibDirectoryNeeded += this.MediaPlayerVlcLibDirectoryNeeded;
                control.MediaPlayer.Playing += this.MediaPlayerPlaying;
                control.MediaPlayer.EncounteredError += this.MediaPlayerEncounteredError;
                control.MediaPlayer.Opening += this.MediaPlayerOpening;
                control.Unloaded += this.UcVlcMediaPlayerControlUnloaded;
                this.VlcControl = control;
            }
            catch (Exception)
            {
                this.VlcControl.MediaPlayer.OnEncounteredError();
                throw;
            }
        }

        /// <summary>
        /// The media player opening.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MediaPlayerOpening(object sender, Vlc.DotNet.Core.VlcMediaPlayerOpeningEventArgs e)
        {
            this.StreamingStatusChanged(ConnectionStatus.Idle, string.Empty);
        }

        /// <summary>
        /// The media player encountered error.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MediaPlayerEncounteredError(object sender, Vlc.DotNet.Core.VlcMediaPlayerEncounteredErrorEventArgs e)
        {
            this.PlayerError(sender, new PlayerErrorEventArgs(e.ToString(), "Error Occurred in VLC Media Player"));
        }

        /// <summary>
        /// The media player playing.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MediaPlayerPlaying(object sender, Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs e)
        {
            this.StreamingStatusChanged(ConnectionStatus.Streaming, string.Empty);
        }

        /// <summary>
        /// The media player vlc lib directory needed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MediaPlayerVlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;

            if (currentDirectory == null)
            {
                return;
            }

            e.VlcLibDirectory = AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture
                                == ProcessorArchitecture.X86 ? new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\lib\WPF\x86")) : new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\lib\WPF\x64"));
        }

        /// <summary>
        /// The VLC media player control unloaded.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void UcVlcMediaPlayerControlUnloaded(object sender, RoutedEventArgs e)
        {
            if (this.VlcControl.MediaPlayer.IsPlaying)
            {
                this.VlcControl.MediaPlayer.Stop();
            }
        }

        /// <summary>
        /// The setup player object.
        /// </summary>
        /// <param name="mediaType">
        /// The media type.
        /// </param>
        /// <returns>
        /// The <see cref="UIElement"/>.
        /// </returns>
        public UIElement SetupPlayerObject(MediaType mediaType)
        {
            this._playerWinFormHost = new WindowsFormsHost
             {
                 Child = this.VlcControl.MediaPlayer
             };
            return this._playerWinFormHost;
        }

        public bool Play()
        {
            try
            {
                var uri = new Uri(string.IsNullOrEmpty(this.videoUri) ? @"http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4" : this.videoUri);
                Trace.WriteLine(uri);
                this.VlcControl.MediaPlayer.Play(uri);
            }
            catch (Exception exception)
            {

                Trace.WriteLine(exception);
            }
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
            if (StreamingStatusChanged != null)
            {
                this.StreamingStatusChanged(status, errorCode);
            }
        }

        public bool Play(DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            if (this.VlcControl.MediaPlayer.IsPlaying)
            {
                this.VlcControl.MediaPlayer.Pause();
            }
        }

        public void Stop()
        {
            if (this.VlcControl.MediaPlayer.IsPlaying)
            {
                this.VlcControl.MediaPlayer.Stop();
            }
        }

        public void TakeSnapshot(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Exit(bool retainProcessHost = false)
        {
            throw new NotImplementedException();
        }

        public bool Initialize(string uri)
        {
            this.videoUri = uri;
            return true;
        }

        public event EventHandler<StreamingStaticsEventArgs> StreamingStatisticsUpdated;

        public event StreamingStatusEventHanlder StreamingStatusChanged;

        public event SelectEventHandler Selected;

        public event DragStartEventHandler DragStart;

        public event System.Windows.DragEventHandler Drop;

        public event PlayerErrorEventHandler PlayerError;

        public event PlayStateChangedEventHandler PlayStateChanged;

        public event PlayPositionChangedEventHandler PlayPositionChanged;

        public double Width
        {
            get { throw new NotImplementedException(); }
        }

        public double Height
        {
            get { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
            this.VlcControl.Dispose();
        }
    }
}
