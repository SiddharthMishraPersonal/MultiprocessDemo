// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationViewModel.cs" company="Motorola Solutions Inc">
//   Demo purpose only.
// </copyright>
// <summary>
//   The application view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Multiprocess.Issue.DemoApp.ViewModels
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using MultiProcess.Client;
    using Multiprocess.Issue.DemoApp.UserControls;
    using MultiProcess.MediaPlayerHelper;
    using Telerik.Windows.Controls;

    /// <summary>
    /// The application view model.
    /// </summary>
    public class ApplicationViewModel : ViewModelBase
    {
        #region Private member variables

        /// <summary>
        /// The is VLC player.
        /// </summary>
        private bool isVlcPlayer;

        /// <summary>
        /// The video count.
        /// </summary>
        private int count = 0;

        /// <summary>
        /// The external process id.
        /// </summary>
        private int externalProcessId = 0;

        /// <summary>
        /// The media player proxy factory.
        /// </summary>
        private IMediaPlayerProxyFactory mediaPlayerProxyFactory;

        /// <summary>
        /// The busy indicator.
        /// </summary>
        private RadBusyIndicator busyIndicator;

        /// <summary>
        /// The is busy.
        /// </summary>
        private bool isBusy;

        /// <summary>
        /// The view.
        /// </summary>
        private Window view;

        /// <summary>
        /// The start video command.
        /// </summary>
        private ICommand startVideoCommand;

        /// <summary>
        /// The stop video command.
        /// </summary>
        private ICommand stopVideoCommand;

        /// <summary>
        /// The setup command.
        /// </summary>
        private ICommand setupCommand;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationViewModel"/> class.
        /// </summary>
        /// <param name="view">
        /// The view.
        /// </param>
        public ApplicationViewModel(Window view)
        {
            if (view == null)
            {
                return;
            }

            this.View = view;
            this.View.DataContext = this;

            this.StartVideoCommand = new DelegateCommand(this.StartVlcVideos);
            this.StopVideoCommand = new DelegateCommand(this.StopVlcVideos);
            this.SetupCommand = new DelegateCommand(this.SetupCommandHandler);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        public Window View
        {
            get
            {
                return this.view;
            }

            set
            {
                this.view = value;
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
        /// Gets or sets the player.
        /// </summary>
        public IMediaPlayer Player { get; set; }

        /// <summary>
        /// Gets or sets the media.
        /// </summary>
        public UIElement Media { get; set; }

        /// <summary>
        /// Gets or sets the setup command.
        /// </summary>
        public ICommand SetupCommand
        {
            get
            {
                return this.setupCommand;
            }

            set
            {
                this.setupCommand = value;
                this.OnPropertyChanged("SetupCommand");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is busy.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                this.isBusy = value;
                this.OnPropertyChanged("IsBusy");

                if (this.busyIndicator != null)
                {
                    this.busyIndicator.IsBusy = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is VLC player.
        /// </summary>
        public bool IsVlcPlayer
        {
            get
            {
                return this.isVlcPlayer;
            }

            set
            {
                this.isVlcPlayer = value;
                this.OnPropertyChanged(() => this.isVlcPlayer);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            this.mediaPlayerProxyFactory.Dispose();
            base.Dispose(disposing);
        }

        /// <summary>
        /// The dispatcher service used to call to the UI thread and analyze performance
        /// </summary>
        /// <param name="host">
        /// The host.
        /// </param>
        private void PlayVideoExpernally(ContentControl host)
        {
            this.mediaPlayerProxyFactory = new MediaPlayerProxyFactory();
            var workerThread = new Thread(
                () =>
                {
                    var isHostUnInitialized = true;

                    // need to access these properties from UI thread
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            isHostUnInitialized = host.Content == null;
                        });

                    if (isHostUnInitialized)
                    {
                        var videoUrl = IsVlcPlayer ? string.Format(@"C:\VideoHD\4KVideo0{0}.mp4", this.count++) : string.Format(@"C:\VideoHD\WMPMP4\4KVideo0{0}Converted.mp4", this.count++);

                        // videoUrl = @"http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_1080p_h264.mov";
                        Trace.WriteLine(videoUrl);
                        var mediaUri =
                            new Uri(videoUrl);
                        Application.Current.Dispatcher.Invoke(
                      () =>
                      {
                          host.ToolTip = videoUrl;
                          host.ParentOfType<Border>().ToolTip = videoUrl;
                          var textBlock = host.ParentOfType<Border>().FindName("VideoUrlTextBlock") as TextBlock;
                          if (textBlock != null)
                          {
                              textBlock.Text = videoUrl;
                          }
                      });

                        this.Player = this.mediaPlayerProxyFactory.GetPlayerInstance(mediaUri, this.externalProcessId);

                        this.externalProcessId = 0;

                        if (this.Player != null)
                        {
                            this.Player.Initialize(mediaUri.AbsoluteUri);
                            var mediaType = this.IsVlcPlayer ? MediaType.VlcPlayer : MediaType.WindowMediaPlayer;
                            this.Media = this.Player.SetupPlayerObject(mediaType);
                        }

                        if (this.Media != null)
                        {
                            Application.Current.Dispatcher.Invoke(
                                () =>
                                {
                                    if (host != null)
                                    {
                                        host.Content = this.Media;
                                    }
                                });
                        }

                        if (this.Player != null)
                        {
                            this.Player.StreamingStatusChanged += this.PlayerStreamingStatusChanged;
                            this.Player.PlayerError += this.PlayerPlayerError;
                            this.Player.Play();
                        }
                    }
                });

            workerThread.Start();
        }

        /// <summary>
        /// The player player error.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void PlayerPlayerError(object sender, MultiProcess.Client.EventArg.PlayerErrorEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(
                () => { MessageBox.Show(this.View, e.ErrorCode + "\n" + e.ErrorMessage, "Error"); });
        }

        /// <summary>
        /// The player streaming status changed.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="errorCode">
        /// The error code.
        /// </param>
        private void PlayerStreamingStatusChanged(ConnectionStatus status, string errorCode)
        {
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(
                    () =>
                    {
                        this.IsBusy = !status.Equals(ConnectionStatus.Streaming);
                    });
            }
        }

        #region Command Methods

        /// <summary>
        /// Method starts all VLC videos.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        private void StartVlcVideos(object o)
        {
            this.Player.Play();
        }

        /// <summary>
        /// The setup command handler.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        private void SetupCommandHandler(object o)
        {
            if (this.count >= 15)
            {
                return;
            }

            var userContentControl = new ucContentControlMedia();
            var contentControl = userContentControl.FindName("ContentControl") as ContentControl;
            this.busyIndicator = userContentControl.FindName("BusyIndicator") as RadBusyIndicator;
            this.IsBusy = true;

            var mainGrid = this.View.FindName("MainGrid") as Grid;
            if (mainGrid != null)
            {
                mainGrid.Children.Add(userContentControl);

                Grid.SetRow(userContentControl, this.count / 4);
                Grid.SetColumn(userContentControl, this.count % 4);
            }

            this.PlayVideoExpernally(contentControl);
        }

        /// <summary>
        /// The stop VLC videos.
        /// </summary>
        /// <param name="o">
        /// The o.
        /// </param>
        private void StopVlcVideos(object o)
        {
            this.Player.Stop();
        }

        #endregion

        #endregion
    }
}
