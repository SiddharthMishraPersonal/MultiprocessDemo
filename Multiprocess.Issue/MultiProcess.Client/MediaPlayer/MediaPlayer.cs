using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiProcess.Client.MediaPlayer
{
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Forms.Integration;

    using MultiProcess.Client.EventArg;

    using Vlc.DotNet.Wpf;

    public class MediaPlayer : IMediaPlayer
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

        public MediaPlayer()
        {
            try
            {
                var control = new VlcControl();
                control.MediaPlayer.VlcLibDirectoryNeeded += MediaPlayer_VlcLibDirectoryNeeded;
                control.MediaPlayer.Playing += MediaPlayer_Playing;
                control.MediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
                control.MediaPlayer.Opening += MediaPlayer_Opening;
                control.Unloaded += this.ucVLCMediaPlayerControl_Unloaded;
                this.VlcControl = control;
            }
            catch (Exception)
            {
                this.VlcControl.MediaPlayer.OnEncounteredError();
                throw;
            }
        }

        void MediaPlayer_Opening(object sender, Vlc.DotNet.Core.VlcMediaPlayerOpeningEventArgs e)
        {
            StreamingStatusChanged(ConnectionStatus.Idle, "");
        }

        void MediaPlayer_EncounteredError(object sender, Vlc.DotNet.Core.VlcMediaPlayerEncounteredErrorEventArgs e)
        {
            PlayerError(sender, new PlayerErrorEventArgs(e.ToString(), "Error Occurred in VLC Media Player"));
        }

        void MediaPlayer_Playing(object sender, Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs e)
        {
            StreamingStatusChanged(ConnectionStatus.Streaming, "");
        }

        void MediaPlayer_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;

            if (currentDirectory == null)
                return;

            e.VlcLibDirectory = AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture
                                == ProcessorArchitecture.X86 ? new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\lib\WPF\x86")) : new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\lib\WPF\x64"));
        }

        void ucVLCMediaPlayerControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.VlcControl.MediaPlayer.IsPlaying)
            {
                this.VlcControl.MediaPlayer.Stop();
            }
        }


        public UIElement SetupPlayerObject()
        {
            _playerWinFormHost = new WindowsFormsHost
            {
                Child = this.VlcControl.MediaPlayer
            };
            return _playerWinFormHost;
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
                StreamingStatusChanged(status, errorCode);
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
