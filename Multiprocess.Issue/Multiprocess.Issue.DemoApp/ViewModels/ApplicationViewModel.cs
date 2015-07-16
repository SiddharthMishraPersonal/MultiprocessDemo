using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multiprocess.Issue.DemoAppViewModels
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using MultiProcess.Client;

    using Multiprocess.Issue.DemoApp.UserControls;

    using MultiProcess.MediaPlayerHelper;

    using Telerik.Windows.Controls;

    using Vlc.DotNet.Wpf;

    public class ApplicationViewModel : ViewModelBase
    {
        #region Private member variables

        /// <summary>
        /// The vlc media player view models.
        /// </summary>
        private ObservableCollection<VLCMediaPlayerViewModel> vlcMediaPlayerViewModels = new ObservableCollection<VLCMediaPlayerViewModel>();

        private List<string> uriList = new List<string>(16);

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

        private ICommand setupCommand;

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

            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4");
            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_640x360.m4v");
            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_1080p_h264.mov");
            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_1080p_stereo.avi");
            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_1080p_stereo.ogg");
            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_1080p_surround.avi");
            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_h264.mov");
            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_stereo.avi");
            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_stereo.ogg");
            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_surround-fix.avi");
            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_480p_surround.avi");
            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_720p_h264.mov");
            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_720p_stereo.avi");
            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_720p_surround.avi");
            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_720p_stereo.ogg");
            this.uriList.Add("http://download.blender.org/peach/bigbuckbunny_movies/big_buck_bunny_1080p_h264.mov");


            this.View = view;
            this.View.DataContext = this;
            var mainGrid = this.View.FindName("MainGrid") as Grid;

            //if (mainGrid != null)
            //{
            //    for (int i = 0; i < 16; i++)
            //    {
            //        var uriObject = new Uri(this.uriList[i]);
            //        var vlcControl = new VLCMediaPlayerViewModel(new ucVLCMediaPlayerControl(), uriObject);
            //        this.vlcMediaPlayerViewModels.Add(vlcControl);
            //    }
            //    var row = 0;
            //    var column = 0;
            //    foreach (var vlcMediaPlayerViewModel in vlcMediaPlayerViewModels)
            //    {
            //        mainGrid.Children.Add(vlcMediaPlayerViewModel.View);
            //        Grid.SetRow(vlcMediaPlayerViewModel.View, row);
            //        Grid.SetColumn(vlcMediaPlayerViewModel.View, column++);
            //        if (column == 4)
            //        {
            //            row++;
            //            column = 0;
            //        }
            //    }
            //}

            this.StartVideoCommand = new DelegateCommand(this.StartVlcVideos);
            this.StopVideoCommand = new DelegateCommand(this.StopVlcVideos);
            this.SetupCommand = new DelegateCommand(this.SetupCommandHanlder);
        }

        #endregion

        #region Private Methods

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

        private int count = 0;

        private RadBusyIndicator busyIndicator;

        private void SetupCommandHanlder(object o)
        {
            if (count >= 15)
                return;

            var ucContentControl = new ucContentControlMedia();
            var contentControl = ucContentControl.FindName("ContentControl") as ContentControl;
            this.busyIndicator = ucContentControl.FindName("BusyIndicator") as RadBusyIndicator;
            if (this.busyIndicator != null)
            {
                this.busyIndicator.IsBusy = true;
            }

            var mainGrid = this.View.FindName("MainGrid") as Grid;
            mainGrid.Children.Add(ucContentControl);
            Grid.SetRow(ucContentControl, count / 4);
            Grid.SetColumn(ucContentControl, count % 4);
            count++;
            PlayVideoExpernally(contentControl);
        }

        /// <summary>
        /// The stop vlc videos.
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

        public IMediaPlayer Player { get; set; }

        private int externalProcessId = 0;

        private IMediaPlayerProxyFactory mediaPlayerProxyFactory;

        public UIElement Media { get; set; }

        public ICommand SetupCommand
        {
            get
            {
                return this.setupCommand;
            }
            set
            {
                this.setupCommand = value;
                OnPropertyChanged("SetupCommand");
            }
        }

        /// <summary>
        /// The dispatcher service used to call to the UI thread and analyze performance
        /// </summary>
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
                        var mediaUri =
                            new Uri(
                                @"http://download.blender.org/peach/bigbuckbunny_movies/BigBuckBunny_320x180.mp4");
                        this.Player = this.mediaPlayerProxyFactory.GetPlayerInstance(mediaUri, externalProcessId);
                        this.externalProcessId = 0;

                        if (this.Player != null)
                        {
                            this.Player.Initialize(mediaUri.AbsoluteUri);

                            this.Media = this.Player.SetupPlayerObject();
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
                            this.Player.Play();

                        }
                    }
                });

            workerThread.Start();
        }

        private void PlayerStreamingStatusChanged(ConnectionStatus status, string errorCode)
        {
            if (status.Equals(ConnectionStatus.Streaming))
            {
                Application.Current.Dispatcher.Invoke(
                    () =>
                        {
                            busyIndicator.IsBusy = false;
                        });
            }
        }
    }
}
