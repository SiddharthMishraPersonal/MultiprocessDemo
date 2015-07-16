// -----------------------------------------------------------------------
// <copyright file="MediaPlayerServer.cs" company="Motorola Solutions, Inc.">
//   Copyright (C) 2015 Motorola Solutions, Inc.
// </copyright>
// -----------------------------------------------------------------------

namespace Motorola.IVS.Client.Viewer.ProcessHost
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.ServiceModel;
    using System.Windows;
    using System.Windows.Interop;

    using global::Viewer.ProcessHost;
    using Motorola.IVS.Client.Viewer.ProcessHost.EventArgs;

    using MultiProcess.Client;
    using MultiProcess.Client.MediaPlayer;

    using Viewer.ProcessHost.Communication;

    using Vlc.DotNet.Wpf;

    /// <summary>
    /// Class for interfacing between actual MediaPlayer and WCF client.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class MediaPlayerServer : IMediaPlayerServer
    {
        /// <summary>
        /// Name of the event log source to write to.
        /// </summary>
        private static readonly string EventLogSource = "Test Application";

        /// <summary>
        /// Backing store for <see cref="HostForm"/>.
        /// </summary>
        private ProcessHostForm processContent;

        private IMediaPlayer mediaPlayer;

        /// <summary>
        /// The hosted Media Player object.
        /// </summary>
        private IMediaPlayer MediaPlayer
        {
            get
            {
                return mediaPlayer;
            }
            set
            {
                mediaPlayer = value;
                Trace.WriteLine("IMediaPlayer setter value :" + value);
            }
        }

        /// <summary>
        /// The callback channel for raising events back on the client.
        /// </summary>
        private IMediaPlayerServiceEventContract callbackChannel;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlayerServer"/> class.
        /// </summary>
        public MediaPlayerServer()
        {
            Application.Current.Dispatcher.Invoke(this.InitializeHostForm);
        }

        /// <summary>
        /// EventHandler delegate for Exited event.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">Whether to retain the process host even if this is the last MediaPlayer instance.</param>
        public delegate void ExitedEventHandler(object sender, ExitedEventArgs e);

        /// <summary>
        /// Fired when the plugin/stream is being closed down.
        /// </summary>
        public event ExitedEventHandler Exited;

        /// <summary>
        /// Gets the WCF listening address/uri for this instance.
        /// </summary>
        public string ListeningAddress { get; private set; }

        /// <summary>
        /// Gets the HostForm containing the hosted media player.
        /// </summary>
        public Window HostForm
        {
            get
            {
                return this.processContent;
            }
        }

        /// <summary>
        /// Creates the <see cref="ServiceHost"/> and sets it to Open state.
        /// </summary>
        /// <returns>The URI of the service.</returns>
        public string StartListening()
        {
            var uri = @"net.pipe://localhost/viewer.processhost/{0}/{1}";
            var listeningAddress = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(uri))
                {
                    listeningAddress = string.Format(uri, Process.GetCurrentProcess().Id, Guid.NewGuid());
                    var serviceHost = this.CreateServiceHost(new Uri(listeningAddress));

                    serviceHost.Open();
                }

                this.ListeningAddress = listeningAddress;
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
            }

            return listeningAddress;
        }



        /// <summary>
        /// Creates a snapshot of the video stream with the specified path/filename.
        /// </summary>
        /// <param name="filename">The full path/name of the snapshot file.</param>
        public void TakeSnapshot(string filename)
        {
            this.SafePlayerCall(() => this.MediaPlayer.TakeSnapshot(filename));
        }

        /// <summary>
        /// Begins streaming Live on the configured stream.
        /// </summary>
        /// <returns>A value indicating whether Play was successful.</returns>
        public bool Play()
        {
            bool played = false;

            try
            {
                played = this.SafePlayerCall(() => this.MediaPlayer.Play(), false);
            }
            catch (Exception exception)
            {
                Trace.WriteLine(exception);
            }

            return played;
        }

        /// <summary>
        /// Play recorded stream.
        /// </summary>
        /// <param name="startTime">The <see cref="DateTime"/> for the starting-point of the stream.</param>
        /// <param name="endTime">The <see cref="DateTime"/> for the ending-point of the stream.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        public bool Play(DateTime startTime, DateTime endTime)
        {
            return this.SafePlayerCall(() => this.MediaPlayer.Play(startTime, endTime), false);
        }

        /// <summary>
        /// Pause recorded stream.
        /// </summary>
        public void Pause()
        {
            this.SafePlayerCall(this.MediaPlayer.Pause);
        }

        /// <summary>
        /// Stop the video stream.
        /// </summary>
        public void Stop()
        {
            Trace.WriteLine("Stopped.");
            this.SafePlayerCall(this.MediaPlayer.Stop);
        }

        /// <summary>
        /// Calls SetupPlayerObject on the IMediaPlayer, but returns a handle rather than the actual UIElement.
        /// </summary>
        /// <param name="uri">
        /// The uri of the video stream.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/> handle to the player window.
        /// </returns>
        public IntPtr SetupPlayerObject(string uri)
        {
            return this.SafePlayerCall(() => this.SetupPlayerObjectImpl(uri), IntPtr.Zero);
        }

        /// <summary>
        /// Moves the camera to the indicated preset.
        /// </summary>
        /// <param name="channelId">The channel of the camera.</param>
        /// <param name="presetId">The preset to move to.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public bool GoToPreset(string channelId, int presetId)
        {
            //var player = this.mediaPlayer as IPtzEnabled;
            //if (player != null)
            //{
            //    return this.SafePlayerCall(() => player.GoToPreset(channelId, presetId), false);
            //}

            return false;
        }

        /// <summary>
        /// Starts a pan/tilt operation.
        /// </summary>
        /// <param name="channelId">The channel of the camera.</param>
        /// <param name="pan">The pan value.</param>
        /// <param name="tilt">The tilt value.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public bool StartPanTilt(string channelId, int pan, int tilt)
        {
            //var player = this.mediaPlayer as IPtzEnabled;
            //if (player != null)
            //{
            //    return this.SafePlayerCall(() => player.StartPanTilt(channelId, pan, tilt), false);
            //}

            return false;
        }

        /// <summary>
        /// Starts a zoom operation.
        /// </summary>
        /// <param name="channelId">The channel of the camera.</param>
        /// <param name="zoomDirection">The direction to zoom in. True for ZoomIn and False for ZoomOut.</param>
        /// <param name="zoomStrength">The speed to zoom.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public bool StartZoom(string channelId, bool zoomDirection, int zoomStrength)
        {
            //var player = this.mediaPlayer as IPtzEnabled;
            //if (player != null)
            //{
            //    return this.SafePlayerCall(() => player.StartZoom(channelId, zoomDirection, zoomStrength), false);
            //}

            return false;
        }

        /// <summary>
        /// Stops a pan/tilt operation.
        /// </summary>
        /// <param name="channelId">The channel of the camera.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public bool StopPanTilt(string channelId)
        {
            //var player = this.mediaPlayer as IPtzEnabled;
            //if (player != null)
            //{
            //    return this.SafePlayerCall(() => player.StopPanTilt(channelId), false);
            //}

            return false;
        }

        /// <summary>
        /// Stops a zoom operation.
        /// </summary>
        /// <param name="channelId">The channel of the camera.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public bool StopZoom(string channelId)
        {
            //var player = this.mediaPlayer as IPtzEnabled;
            //if (player != null)
            //{
            //    return this.SafePlayerCall(() => player.StopZoom(channelId), false);
            //}

            return false;
        }

        /// <summary>
        /// Sets the digital zoom window.
        /// </summary>
        /// <param name="left">The left coordinate of digitalZoom rectangle</param>
        /// <param name="top">The top coordinate of digitalZoom rectangle</param>
        /// <param name="width">The width of digitalZoom rectangle</param>
        /// <param name="height">The height of digitalZoom rectangle</param>
        public void SetDigitalZoomWindow(int left, int top, int width, int height)
        {
            //var player = this.mediaPlayer as IDigitalPtzEnabled;
            //if (player != null)
            //{
            //    this.SafePlayerCall(() => player.SetDigitalZoomWindow(left, top, width, height));
            //}
        }

        /// <summary>
        /// Gets the width.
        /// </summary>
        public double Width
        {
            get
            {
                return this.SafePlayerCall(() => this.MediaPlayer.Width, 0);
            }
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        public double Height
        {
            get
            {
                return this.SafePlayerCall(() => this.MediaPlayer.Height, 0);
            }
        }

        /// <summary>
        /// Sets-up and persists the callback channel.
        /// </summary>
        public void SetupCallbackChannel()
        {
            this.callbackChannel = OperationContext.Current.GetCallbackChannel<IMediaPlayerServiceEventContract>();
        }

        /// <summary>
        /// Shuts-down the media player for exit.
        /// </summary>
        internal void Exit()
        {
            Trace.WriteLine("Exit Called.");
            this.SafePlayerCall(() => this.MediaPlayer.Stop(), updateStatusOnError: false);
            this.MediaPlayer = null;
        }

        /// <summary>
        /// Writes an entry to the event log.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="type">What severity to make the message.</param>
        private static void Log(string message, EventLogEntryType type)
        {
            if (EventLog.SourceExists(EventLogSource))
            {
                EventLog.WriteEntry(EventLogSource, message, type);
            }
        }

        /// <summary>
        /// Initializes the <see cref="IMediaPlayer"/> object.
        /// </summary>
        /// <param name="uri">
        /// The video stream URI.
        /// </param>
        /// <returns>
        /// A value indicating whether the initialization was successful.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an <see cref="Exception"/> if <see cref="SetupPlayerObject"/> was not called first.
        /// </exception>
        private bool Initialize(string uri)
        {
            if (this.MediaPlayer == null)
            {
                throw new Exception("SetupPlayerObject must be called before initialize.");
            }

            //this.mediaPlayer.StreamingStatusChanged += this.OnStreamingStatusChanged;
            //((IMouseAware)this.mediaPlayer).MouseEnter += this.OnMouseEnter;
            //((IMouseAware)this.mediaPlayer).MouseLeave += this.OnMouseLeave;

            return this.SafePlayerCall(() => this.MediaPlayer.Initialize(uri), false, true);
        }

        /// <summary>
        /// Handle an error state in the player.
        /// </summary>
        /// <param name="signalExit">Whether to signal an "exit" condition to the ProcessHost.</param>
        /// <param name="updateStreamingStatus">Whether to update the StreamingStatus to Disconnected.</param>
        private void HandlePlayerError(bool signalExit = false, bool updateStreamingStatus = true)
        {
            Trace.WriteLine("Handle Player Error");
            if (updateStreamingStatus)
            {
                this.OnStreamingStatusChanged(ConnectionStatus.Disconnected, string.Empty);
            }

            this.MediaPlayer = null;

            if (signalExit)
            {
                this.OnExited(false);
            }
        }

        /// <summary>
        /// Fires the Exited event.
        /// </summary>
        /// <param name="retainProcessHost">
        /// Whether to signal to the ProcessHost to not exit even if this was the last open MediaPlayer.
        /// </param>
        private void OnExited(bool retainProcessHost)
        {
            var handler = this.Exited;

            if (handler != null)
            {
                handler(this, new ExitedEventArgs(retainProcessHost));
            }
        }

        /// <summary>
        /// Creates the ServiceHost and listening endpoint.
        /// </summary>
        /// <param name="listeningUri">The URI at which to listen.</param>
        /// <returns>The <see cref="ServiceHost"/>.</returns>
        private ServiceHost CreateServiceHost(Uri listeningUri)
        {
            var serviceHost = new ServiceHost(this, listeningUri);
            serviceHost.AddServiceEndpoint(typeof(IMediaPlayerServiceContract), new NetNamedPipeBinding(), string.Empty);

            return serviceHost;
        }



        void control_Unloaded(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Calls SetupPlayerObject on the IMediaPlayer, but returns a handle rather than the actual UIElement.
        /// </summary>
        /// <param name="uri">
        /// The uri of the video stream.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/> handle to the player window.
        /// </returns>
        private IntPtr SetupPlayerObjectImpl(string uri)
        {
            //Debugger.Launch();
            Trace.WriteLine("Called SetupPlayerObjectImpl(string uri)");
            Trace.WriteLine(uri);

            this.MediaPlayer = new MediaPlayer();
            this.MediaPlayer.StreamingStatusChanged += MediaPlayer_StreamingStatusChanged;
            Trace.WriteLine(this.MediaPlayer);
            var hostedControl = this.MediaPlayer.SetupPlayerObject();

            if (!this.Initialize(uri))
            {
                return IntPtr.Zero;
            }

            WindowInteropHelper windowHelper = null;

            if (this.HostForm != null)
            {

                this.HostForm.Content = hostedControl;
                (this.HostForm as ProcessHostForm).Show();
                //var processHostForm = this.HostForm as ProcessHostForm;
                //if (processHostForm != null)
                //{
                //    processHostForm.ShowWindow();
                //}
                windowHelper = new WindowInteropHelper(this.HostForm);
            }

            Trace.WriteLine("Media is playing.");
            Trace.WriteLine(windowHelper);
            Trace.WriteLine(windowHelper.Handle);
            return windowHelper.Handle;
        }

        void MediaPlayer_StreamingStatusChanged(ConnectionStatus status, string errorCode)
        {
            this.OnStreamingStatusChanged(status, errorCode);
        }



        /// <summary>
        /// Initialize the <see cref="ProcessHostForm"/> that will contain the video.
        /// </summary>
        private void InitializeHostForm()
        {
            this.processContent = new ProcessHostForm();
        }

        /// <summary>
        /// Called when streaming status has changed. Notifies client via callback.
        /// </summary>
        /// <param name="status">The new status.</param>
        /// <param name="errorCode">Associated error code (if any).</param>
        private void OnStreamingStatusChanged(ConnectionStatus status, string errorCode)
        {
            if (this.callbackChannel != null)
            {
                this.callbackChannel.OnStreamingStatusChanged(status, errorCode);
            }
        }

        /// <summary>
        /// Notifies the client of a MouseLeave event.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="eventArgs">Event arguments. (Not used)</param>
        private void OnMouseLeave(object sender, System.EventArgs eventArgs)
        {
            if (this.callbackChannel != null)
            {
                this.callbackChannel.OnMouseLeave();
            }
        }

        /// <summary>
        /// Notifies the client of a MouseEnter event.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="eventArgs">Event arguments. (Not used)</param>
        private void OnMouseEnter(object sender, System.EventArgs eventArgs)
        {
            if (this.callbackChannel != null)
            {
                this.callbackChannel.OnMouseEnter();
            }
        }

        /// <summary>
        /// Executes the supplied action with exception handling.
        /// </summary>
        /// <param name="action">The <see cref="Action"/> to execute.</param>
        /// <param name="signalExitOnError">Whether to cause an exit if the action errors.</param>
        /// <param name="updateStatusOnError">Whether to update the StreamingStatus to Disconnected on error.</param>
        /// <param name="useDispatcher">Whether to use the application dispatcher.</param>
        private void SafePlayerCall(Action action, bool signalExitOnError = false, bool updateStatusOnError = true, bool useDispatcher = true)
        {
            this.SafePlayerCall<object>(
                () =>
                {
                    action();
                    return null;
                },
                    null,
                    signalExitOnError,
                    updateStatusOnError,
                    useDispatcher);
        }

        /// <summary>
        /// Executes the supplied action with exception handling.
        /// </summary>
        /// <typeparam name="TResult">The return type of the supplied action.</typeparam>
        /// <param name="action">The method to execute.</param>
        /// <param name="defaultReturn">What default value to return if the action errors.</param>
        /// <param name="signalExitOnError">Whether to cause an exit if the action errors.</param>
        /// <param name="updateStatusOnError">Whether to update the StreamingStatus to Disconnected on error.</param>
        /// <param name="useDispatcher">Whether to use the application dispatcher.</param>
        /// <returns>The return value of the supplied action.</returns>
        private TResult SafePlayerCall<TResult>(
            Func<TResult> action, TResult defaultReturn, bool signalExitOnError = false, bool updateStatusOnError = true, bool useDispatcher = true)
        {
            try
            {
                return useDispatcher ? Application.Current.Dispatcher.Invoke(action) : action();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                Trace.WriteLine(ex.StackTrace);
                Trace.WriteLine("Unexpected error during SafePlayerCall: " + action.Method.Name + "\r\n\r\nError: " + ex);
            }

            this.HandlePlayerError(signalExitOnError, updateStatusOnError);
            return defaultReturn;
        }

        public int CurrentPosition
        {
            get { throw new NotImplementedException(); }
        }

        public DateTimeOffset CurrentLiveTimestamp
        {
            get { throw new NotImplementedException(); }
        }

        public int MaxBufferSize
        {
            get { throw new NotImplementedException(); }
        }

        public int CurrentBufferSize
        {
            get { throw new NotImplementedException(); }
        }

        public bool DigitalZoomCapable
        {
            get { throw new NotImplementedException(); }
        }


        public bool FastForward(PlayerSpeed speed)
        {
            throw new NotImplementedException();
        }

        public bool FastRewind(PlayerSpeed speed)
        {
            throw new NotImplementedException();
        }

        public void Seek(int secondsBack)
        {
            throw new NotImplementedException();
        }
    }
}