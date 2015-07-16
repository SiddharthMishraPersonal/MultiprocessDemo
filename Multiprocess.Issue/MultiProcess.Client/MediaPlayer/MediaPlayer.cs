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
    using System.Windows.Forms.Integration;

    using Vlc.DotNet.Wpf;

    public class MediaPlayer : IMediaPlayer
    {
        /// <summary>
        ///   The _player win form host.
        /// </summary>
        private WindowsFormsHost _playerWinFormHost;


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
                this.VlcControl = control;
            }
            catch (Exception)
            {

                throw;
            }
        }

        void MediaPlayer_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;

            if (currentDirectory == null)
                return;

            e.VlcLibDirectory = AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture
                                == ProcessorArchitecture.X86 ? new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\..\lib\WPF\x86")) : new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\..\lib\WPF\x64"));

            //var directoryInfo = new DirectoryInfo(@"C:\Users\WKTF64\Documents\GitHub\MultiprocessDemoIssue\Multiprocess.Issue\lib\WinForms\x86");
            //Trace.WriteLine(directoryInfo.Exists);
            //Trace.WriteLine(e.VlcLibDirectory);   
        }

        void ucVLCMediaPlayerControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.VlcControl.MediaPlayer.IsPlaying)
            {
                this.VlcControl.MediaPlayer.Stop();
            }
        }


        public System.Windows.UIElement SetupPlayerObject()
        {
            _playerWinFormHost = new WindowsFormsHost
            {
                Child = this.VlcControl.MediaPlayer
            };
            this.VlcControl.MediaPlayer.Play();

            return this._playerWinFormHost;
        }

        public bool Play()
        {
            this.VlcControl.MediaPlayer.Play();
            return true;
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }
    }
}
