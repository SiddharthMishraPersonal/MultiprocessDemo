using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multiprocess.Issue.DemoAppViewModels
{
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Telerik.Windows.Controls;

    using Vlc.DotNet.Wpf;

    public class VLCMediaPlayerViewModel : ViewModelBase
    {
        #region Private Variables

        /// <summary>
        /// The _view.
        /// </summary>
        private UserControl _view = null;

        /// <summary>
        /// The vlc control.
        /// </summary>
        private VlcControl vlcControl = null;

        /// <summary>
        /// The video uri.
        /// </summary>
        private Uri videoUri;

        /// <summary>
        /// The start video command.
        /// </summary>
        private ICommand startVideoCommand;

        /// <summary>
        /// The stop video command.
        /// </summary>
        private ICommand stopVideoCommand;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        public UserControl View
        {
            get
            {
                return this._view;
            }
            set
            {
                this._view = value;
                this.OnPropertyChanged("View");
            }
        }

        /// <summary>
        /// Gets or sets the start video command.
        /// </summary>
        public ICommand StartVideoCommand
        {
            get
            {
                return this.startVideoCommand;
            }
            set
            {
                this.startVideoCommand = value;
                this.OnPropertyChanged("StartVideoCommand");
            }
        }

        /// <summary>
        /// Gets or sets the stop video command.
        /// </summary>
        public ICommand StopVideoCommand
        {
            get
            {
                return this.stopVideoCommand;
            }
            set
            {
                this.stopVideoCommand = value;
                this.OnPropertyChanged("StopVideoCommand");
            }
        }

        /// <summary>
        /// Gets and Sets the video Uri.
        /// </summary>
        public Uri VideoUri
        {
            get
            {
                return this.videoUri;
            }
            set
            {
                this.videoUri = value;
                this.OnPropertyChanged("VideoUri");
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="VLCMediaPlayerViewModel"/> class.
        /// </summary>
        /// <param name="viewUserControl">
        /// The view user control.
        /// </param>
        /// <param name="videoUri">
        /// The video Uri.
        /// </param>
        public VLCMediaPlayerViewModel(UserControl viewUserControl, Uri videoUri)
        {
            this.View = viewUserControl;
            this.VideoUri = videoUri;
            this.vlcControl = this.View.FindName("VlcControl") as VlcControl;

            this.View.DataContext = this;

            if (this.vlcControl != null)
            {
                this.vlcControl.MediaPlayer.VlcLibDirectoryNeeded += MediaPlayer_VlcLibDirectoryNeeded;
            }

            this.StartVideoCommand = new DelegateCommand(this.StartVlcVideo);
            this.StopVideoCommand = new DelegateCommand(this.StopVlcVideo);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the video.
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        public void StartVideo(Uri uri)
        {
            try
            {
                this.vlcControl.MediaPlayer.Play(uri);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    this.View.Parent as Window,
                    exception.Message,
                    "Media Player Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Stops the video.
        /// </summary>
        public void StopVideo()
        {
            if (this.vlcControl.MediaPlayer.IsPlaying)
            {
                this.vlcControl.MediaPlayer.Stop();
            }
        }

        #endregion

        #region Private Methods
        /// <summary>
        /// Method starts all VLC videos.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        private void StartVlcVideo(object o)
        {
            if (o != null)
            {
                var url = o.ToString();
                this.StartVideo(new Uri(url));
            }
            else
            {
                this.StartVideo(this.VideoUri);
            }
        }


        /// <summary>
        /// The stop VLC videos.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        private void StopVlcVideo(object o)
        {
            this.StopVideo();
        }
        
        /// <summary>
        /// The media player_ vlc lib directory needed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void MediaPlayer_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;

            if (currentDirectory == null)
                return;

            e.VlcLibDirectory = AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture
                                == ProcessorArchitecture.X86 ? new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\..\lib\x86")) : new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\..\lib\x64"));
        }

        #endregion

       
    }
}
