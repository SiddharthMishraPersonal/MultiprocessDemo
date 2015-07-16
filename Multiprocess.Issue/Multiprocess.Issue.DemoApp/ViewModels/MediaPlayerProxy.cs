using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multiprocess.Issue.ViewModels
{
    using System.Diagnostics;
    using System.Windows;

    using Motorola.APS.MMC.Common.Controls;
    using Motorola.APS.MMC.Common.Events;
    using Motorola.APS.MMC.Common.Logging;
    using Motorola.APS.MMC.Common.Logging.Prism;
    using Motorola.APS.MMC.Common.Utilities;
    using Motorola.IVS.Client;
    using Motorola.IVS.Client.Viewer.ProcessHost.Communication;

    public class MediaPlayerProxy
    {
        /// <summary>
        /// How many milliseconds to wait for the external process to finish starting.
        /// </summary>
        private const int ProcessTimeout = 20000;

        /// <summary>
        /// The external ProcessHost.
        /// </summary>
        private readonly Process externalProcess;


        /// <summary>
        /// Provides methods to access this module's specific config file.
        /// </summary>
        private readonly IModuleConfiguration moduleConfiguration;

        /// <summary>
        /// Lock to prevent threading issues.
        /// </summary>
        private readonly object setupLock = new object();

        /// <summary>
        /// The object for communication with the remote media player.
        /// </summary>
        private IMediaPlayerServiceContract mediaPlayerServiceProxy;

        /// <summary>
        /// The object for communication with the process host service.
        /// </summary>
        private IProcessHostServiceContract processHost;

        /// <summary>
        /// Whether this proxy has been initialized.
        /// </summary>
        private bool isInitialized;

        /// <summary>
        /// Whether this instance of MediaPlayerProxy is currently exiting.
        /// </summary>
        private bool exiting;

        /// <summary>
        /// The URI of the desired video stream.
        /// </summary>
        private string channelUri;

        /// <summary>
        /// The URI to the remote media player endpoint.
        /// </summary>
        private string playerProxyUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlayerProxy"/> class.
        /// </summary>
        /// <param name="processHost">
        /// The external process that will host the media player.
        /// </param>
        /// <param name="applicationEventLogger">
        /// The application event logger for writing entries to the application event log and/or log file.
        /// </param>
        /// <param name="moduleConfiguration">
        /// Provides methods to access this module's specific config file.
        /// </param>
        /// <param name="eventAggregator">
        /// Event aggregator to provide access to and a means to publish PRISM events.
        /// </param>
        public MediaPlayerProxy()
        {
            
        }

        /// <summary>
        /// Not currently used.
        /// </summary>
        public event EventHandler<StreamingStaticsEventArgs> StreamingStatisticsUpdated;

        /// <summary>
        /// Fires when the streaming status of the video stream has changed.
        /// </summary>
        public event StreamingStatusEventHanlder StreamingStatusChanged;

        /// <summary>
        /// Not currently used.
        /// </summary>
        public event SelectEventHandler Selected;

        /// <summary>
        /// Not currently used.
        /// </summary>
        public event DragStartEventHandler DragStart;

        /// <summary>
        /// Not currently used.
        /// </summary>
        public event DragEventHandler Drop;

        /// <summary>
        /// Not currently used.
        /// </summary>
        public event PlayerErrorEventHandler PlayerError;

        /// <summary>
        /// Not currently used.
        /// </summary>
        public event PlayStateChangedEventHandler PlayStateChanged;

        /// <summary>
        /// Not currently used.
        /// </summary>
        public event PlayPositionChangedEventHandler PlayPositionChanged;

        /// <summary>
        /// Fired when the mouse enters the video area.
        /// </summary>
        public event EventHandler MouseEnter;

        /// <summary>
        /// Fired when the mouse leaves the video area.
        /// </summary>
        public event EventHandler MouseLeave;

        /// <summary>
        /// Gets the process ID of the associated external process.
        /// </summary>
        public int ExternalProcessId
        {
            get
            {
                return this.externalProcess.Id;
            }
        }

        /// <summary>
        /// Gets the play direction of the current stream. NOT IMPLEMENTED.
        /// </summary>
        public PlayDirection PlayDirection
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the current position in seconds.
        /// </summary>
        public int CurrentPosition
        {
            get
            {
                return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.CurrentPosition, 0);
            }
        }

        /// <summary>
        /// Gets the current timestamp from the VMS (if live)
        /// </summary>
        public DateTimeOffset CurrentLiveTimestamp
        {
            get
            {
                return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.CurrentLiveTimestamp, default(DateTimeOffset));
            }
        }

        /// <summary>
        /// Gets the max buffer size.
        /// </summary>
        public int MaxBufferSize
        {
            get
            {
                return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.MaxBufferSize, 0);
            }
        }

        /// <summary>
        /// Gets the current buffer size.
        /// </summary>
        public int CurrentBufferSize
        {
            get
            {
                return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.CurrentBufferSize, 0);
            }
        }

        /// <summary>
        /// Gets a value indicating whether player is digital zoom capable.
        /// </summary>
        public bool DigitalZoomCapable
        {
            get
            {
                return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.DigitalZoomCapable, false);
            }
        }

        /// <summary>
        /// Sets the digital zoom window.
        /// </summary>
        public void SetDigitalZoomWindow(int left, int top, int width, int height)
        {
            this.SafeChannelCall(() => this.mediaPlayerServiceProxy.SetDigitalZoomWindow(left, top, width, height));
        }

        /// <summary>
        /// Gets or sets the volume.
        /// </summary>
        public Volume Volume
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Initializes the stream URI.
        /// </summary>
        /// <param name="uri">The URI to the desired stream.</param>
        /// <returns>Whether initialization succeeded.</returns>
        public bool Initialize(string uri)
        {
            if (string.IsNullOrWhiteSpace(uri))
            {
                throw new ArgumentNullException("uri");
            }

            this.channelUri = uri;
            this.isInitialized = true;
            return this.isInitialized;
        }

        /// <summary>
        /// Performs setup and initialization on the video plugin.
        /// </summary>
        /// <returns>The <see cref="UIElement"/> containing the video stream.</returns>
        public UIElement SetupPlayerObject()
        {
            lock (this.setupLock)
            {
                if (this.isInitialized == false)
                {
                    throw new Exception("Initialize() must be called before SetupPlayerObject().");
                }

                if (!this.EstablishProcessHostChannel())
                {
                    return null;
                }

                // Wait for process to finish initialization.
                try
                {
                    if (!this.externalProcess.WaitForInputIdle(ProcessTimeout))
                    {
                        this.externalProcess.Kill();
                        return null;
                    }
                }
                catch (InvalidOperationException)
                {
                    /* This condition can happen if we're waiting for InputIdle when the process is closed due to the user closing the video panes.
                     * This is an expected result of using WaitForInputIdle. An alternative method for determining when the process is ready for
                     * communication may be more desirable. */
                    return null;
                }

                this.playerProxyUri = this.SafeChannelCall(this.processHost.GetMediaPlayerProxyChannelUri, string.Empty);
                if (string.IsNullOrEmpty(this.playerProxyUri))
                {
                    return null;
                }

                if (!this.EstablishMediaPlayerServiceChannel())
                {
                    return null;
                }

                var hostHandle = this.SafeChannelCall(() => this.mediaPlayerServiceProxy.SetupPlayerObject(this.channelUri), IntPtr.Zero);

                if (hostHandle == IntPtr.Zero)
                {
                   
                    return null;
                }

                HostedContent hostedContent = null;

                if (Application.Current != null)
                {
                    hostedContent = Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            try
                            {
                                return new HostedContent(null) { ExternalHandle = hostHandle };
                            }
                            catch (OutOfMemoryException ex)
                            {
                               

                                return null;
                            }
                        });

                    if (hostedContent != null)
                    {
                        hostedContent.NativeCallFailed += this.HostedContentNativeCallFailed;
                    }
                }

                return hostedContent;
            }
        }

        /// <summary>
        /// Plays the previously configured stream.
        /// </summary>
        /// <returns>Whether the operation succeeded.</returns>
        public bool Play()
        {
            return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.Play(), false);
        }

        /// <summary>
        /// Plays the previously configured stream at the specified time.
        /// </summary>
        /// <param name="startTime">The date/time at which to start playback.</param>
        /// <param name="endTime">The date/time at which to end playback.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public bool Play(DateTime startTime, DateTime endTime)
        {
            return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.Play(startTime, endTime), false);
        }

        /// <summary>
        /// Fast-forwards the video stream at the specified speed.
        /// </summary>
        /// <param name="speed">
        /// The speed at which to fast-forward.
        /// </param>
        /// <returns>
        /// Whether the operation succeeded.
        /// </returns>
        public bool FastForward(PlayerSpeed speed)
        {
            return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.FastForward(speed), false);
        }

        /// <summary>
        /// Rewinds the video stream at the specified speed.
        /// </summary>
        /// <param name="speed">
        /// The speed at which to rewind.
        /// </param>
        /// <returns>
        /// Whether the operation succeeded.
        /// </returns>
        public bool FastRewind(PlayerSpeed speed)
        {
            return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.FastRewind(speed), false);
        }

        /// <summary>
        /// Pauses the currently playing video stream.
        /// </summary>
        public void Pause()
        {
            this.SafeChannelCall(this.mediaPlayerServiceProxy.Pause);
        }

        /// <summary>
        /// Stops the currently playing stream.
        /// </summary>
        public void Stop()
        {
            this.SafeChannelCall(this.mediaPlayerServiceProxy.Stop);
        }

        /// <summary>
        /// Seeks to the specified position.
        /// </summary>
        /// <param name="secondsBack">
        /// How many seconds back from the end point to seek.
        /// </param>
        public void Seek(int secondsBack)
        {
            this.SafeChannelCall(() => this.mediaPlayerServiceProxy.Seek(secondsBack));
        }

        /// <summary>
        /// Takes a snapshot of the current frame and saves it to the specified location.
        /// </summary>
        /// <param name="fileName">
        /// The desired path/filename for the snapshot file.
        /// </param>
        public void TakeSnapshot(string fileName)
        {
            this.SafeChannelCall(() => this.mediaPlayerServiceProxy.TakeSnapshot(fileName));
        }

        /// <summary>
        /// Sets the video to full-screen mode. NOT IMPLEMENTED.
        /// </summary>
        public void SetFullscreen()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Stops the video stream and cleans up used resources.
        /// </summary>
        public void Exit()
        {
            this.Exit(false);
        }

        /// <summary>
        /// Stops the video stream and cleans up used resources.
        /// </summary>
        /// <param name="retainProcessHost">
        /// Whether to retain ProcessHost even if this is the last MediaPlayer in use. (RIC only)
        /// </param>
        public void Exit(bool retainProcessHost)
        {
            this.exiting = true;
            lock (this.setupLock)
            {
                this.SafeChannelCall(() => this.processHost.CloseMediaPlayer(this.playerProxyUri, retainProcessHost));

                // ReSharper disable SuspiciousTypeConversion.Global (These interfaces are WCF channels)
                var mediaChannel = this.mediaPlayerServiceProxy as System.ServiceModel.Channels.IChannel;
                if (mediaChannel != null)
                {
                    mediaChannel.Close();
                }

                var hostChannel = this.processHost as System.ServiceModel.Channels.IChannel;
                if (hostChannel != null)
                {
                    hostChannel.Close();
                }

                // ReSharper restore SuspiciousTypeConversion.Global
                this.mediaPlayerServiceProxy = null;
                this.processHost = null;
            }
        }

        /// <summary>
        /// Checks whether SDK is installed or not. NOT IMPLEMENTED.
        /// </summary>
        public bool IsSDKInstalled()
        {
        }

        /// <summary>
        /// Moves the camera to the indicated preset.
        /// </summary>
        /// <param name="channelId">The channel of the camera.</param>
        /// <param name="presetId">The preset to move to.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public bool GoToPreset(string channelId, int presetId)
        {
            return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.GoToPreset(channelId, presetId), false);
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
            return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.StartPanTilt(channelId, pan, tilt), false);
        }

        /// <summary>
        /// Starts a zoom operation.
        /// </summary>
        /// <param name="channelId">The channel of the camera.</param>
        /// <param name="zoomDirection">The direction to zoom in.</param>
        /// <param name="zoomStrength">The speed to zoom.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public bool StartZoom(string channelId, bool zoomDirection, int zoomStrength = 30)
        {
            return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.StartZoom(channelId, zoomDirection, zoomStrength), false);
        }

        /// <summary>
        /// Stops a pan/tilt operation.
        /// </summary>
        /// <param name="channelId">The channel of the camera.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public bool StopPanTilt(string channelId)
        {
            return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.StopPanTilt(channelId), false);
        }

        /// <summary>
        /// Stops a zoom operation.
        /// </summary>
        /// <param name="channelId">The channel of the camera.</param>
        /// <returns>Whether the operation succeeded.</returns>
        public bool StopZoom(string channelId)
        {
            return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.StopZoom(channelId), false);
        }

        /// <summary>
        /// Fires the <see cref="StreamingStatusChanged"/> event.
        /// </summary>
        /// <param name="status">What status to indicate in the event.</param>
        /// <param name="errorCode">What error code to indicate in the event.</param>
        public void OnStreamingStatusChanged(ConnectionStatus status, string errorCode)
        {
            var handler = this.StreamingStatusChanged;
            if (handler != null)
            {
                handler(status, errorCode);
            }
        }

        /// <summary>
        /// Fires the <see cref="MouseEnter"/> event.
        /// </summary>
        public void OnMouseEnter()
        {
            var handler = this.MouseEnter;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// Fires the <see cref="MouseLeave"/> event.
        /// </summary>
        public void OnMouseLeave()
        {
            var handler = this.MouseLeave;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources. NOT IMPLEMENTED.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Executes the supplied action with exception handling. Signals a Disconnect state on error.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        private void SafeChannelCall(Action action)
        {
            this.SafeChannelCall<object>(
                () =>
                {
                    action();
                    return null;
                },
                null);
        }

        /// <summary>
        /// Executes the supplied action with exception handling. Signals a Disconnect state on error.
        /// </summary>
        /// <typeparam name="TResult">The type of result the passed-in action will return.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <param name="defaultReturn">The default value to return on error.</param>
        /// <returns>The return value of the passed-in function.</returns>
        private TResult SafeChannelCall<TResult>(Func<TResult> action, TResult defaultReturn)
        {
            try
            {
                return action();
            }
            catch (TimeoutException ex)
            {
                this.applicationEventLogger.WriteExceptionToLog(
                    "MediaPlayerProxy Channel Timeout",
                    "The call to [" + action.Method.Name + "] timed-out.",
                    ex,
                    LoggingCategory.ApplicationLog);
            }
            catch (CommunicationException ex)
            {
                this.applicationEventLogger.WriteExceptionToLog(
                    "MediaPlayerProxy Communication Error",
                    "The call to [" + action.Method.Name + "] experienced an error.",
                    ex,
                    LoggingCategory.ApplicationLog);
            }
            catch (Exception ex)
            {
                this.applicationEventLogger.WriteExceptionToLog(
                    "MediaPlayerProxy Unexpected Error",
                    "The call to [" + action.Method.Name + "] experienced an unexpected error",
                    ex,
                    LoggingCategory.ApplicationLog);
            }

            this.OnStreamingStatusChanged(ConnectionStatus.Disconnected, string.Empty);
            return defaultReturn;
        }

        /// <summary>
        /// Handles Exited event of the ProcessHost. Raises OnStreamingStatusChanged event.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnHostedProcessExited(object sender, EventArgs e)
        {
            this.OnStreamingStatusChanged(ConnectionStatus.Disconnected, string.Empty);
        }

        /// <summary>
        /// Handles the <see cref="HostedContent"/> window being in a potentially unstable state.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">Event args.</param>
        private void HostedContentNativeCallFailed(object sender, EventArgs e)
        {
            this.OnStreamingStatusChanged(ConnectionStatus.Disconnected, string.Empty);
            this.Exit();
        }

        /// <summary>
        /// Handles the communication channel faulting. Attempts to reconnect once, signals Disconnect if reconnect attempt fails.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void MediaChannelFaulted(object sender, EventArgs e)
        {
            // Attempt to re-establish the channel once. Close the stream if we're unable to.
            // Skip if we're exiting. The channel can fault while exiting and can be ignored.
            if (!this.exiting && !this.EstablishMediaPlayerServiceChannel())
            {
                this.OnStreamingStatusChanged(ConnectionStatus.Disconnected, string.Empty);
            }
        }

        /// <summary>
        /// Handles the external process host communication channel faulting.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private void HostChannelFaulted(object sender, EventArgs e)
        {
            // Try once to reconnect.
            this.EstablishProcessHostChannel();
        }

        /// <summary>
        /// Setup the communication channel with the external host.
        /// </summary>
        /// <returns>Whether the channel was successfully established.</returns>
        private bool EstablishProcessHostChannel()
        {
            var uri = this.moduleConfiguration.GetConfigParameter("ProcessHostListeningAddress", string.Empty);
            if (string.IsNullOrWhiteSpace(uri))
            {
                return false;
            }

            var channelFactory = new ChannelFactory<IProcessHostServiceContract>(
                new NetNamedPipeBinding(),
                new EndpointAddress(string.Format(uri, this.ExternalProcessId)));

            this.processHost = channelFactory.CreateChannel();
            channelFactory.Faulted += this.HostChannelFaulted;

            return true;
        }

        /// <summary>
        /// Setup the communication channel with the MediaPlayer in the external host.
        /// </summary>
        /// <returns>Whether the channel was successfully established.</returns>
        private bool EstablishMediaPlayerServiceChannel()
        {
            try
            {
                IClientChannel channel;
                if (this.mediaPlayerServiceProxy != null)
                {
                    // ReSharper disable once SuspiciousTypeConversion.Global (mediaPlayerServiceProxy is only superficially the type, it is also an IClientChannel)
                    channel = this.mediaPlayerServiceProxy as IClientChannel;
                    if (channel != null)
                    {
                        channel.Faulted -= this.MediaChannelFaulted;
                    }
                }

                var binding = new NetNamedPipeBinding { ReceiveTimeout = new TimeSpan(0, 0, 1, 30) };
                var mediaPlayerChannelFactory = new DuplexChannelFactory<IMediaPlayerServiceContract>(
                    this,
                    binding,
                    new EndpointAddress(this.playerProxyUri));
                this.mediaPlayerServiceProxy = mediaPlayerChannelFactory.CreateChannel();

                this.mediaPlayerServiceProxy.SetupCallbackChannel();

                // ReSharper disable once SuspiciousTypeConversion.Global (mediaPlayerServiceProxy is only superficially the type, it is also an IClientChannel)
                channel = this.mediaPlayerServiceProxy as IClientChannel;
                if (channel != null)
                {
                    channel.Faulted += this.MediaChannelFaulted;
                    return true;
                }
            }
            catch (CommunicationException ex)
            {
                this.applicationEventLogger.WriteExceptionToLog(
                    "Communication Error Establishing Media Player Channel",
                    "Error", ex,
                    LoggingCategory.ApplicationLog);
            }
            catch (TimeoutException ex)
            {
                this.applicationEventLogger.WriteExceptionToLog(
                    "Timeout While Establishing Media Player Channel",
                    "A timeout was encountered while establishing callback channel.",
                    ex,
                    LoggingCategory.ApplicationLog);
            }
            catch (Exception ex)
            {
                this.applicationEventLogger.WriteExceptionToLog(
                    "Unexpected Error Establishing Media Player Channel",
                    "Error", ex,
                    LoggingCategory.ApplicationLog);
            }

            return false;
        }

        /// <summary>
        /// On application exit, signals to the remote Media Player to exit/shutdown.
        /// </summary>
        /// <param name="args">
        /// The application closing event args.
        /// </param>
        private void OnApplicationExit(ApplicationClosingEventArgs args)
        {
            if (this.processHost != null)
            {
                try
                {
                    this.SafeChannelCall(() => this.processHost.CloseMediaPlayer(this.playerProxyUri, false));
                }
                catch (CommunicationException)
                {
                }
                catch (TimeoutException)
                {
                }
            }
        }

        public double Width
        {
            get
            {
                return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.Width, 0);
            }
        }

        public double Height
        {
            get
            {
                return this.SafeChannelCall(() => this.mediaPlayerServiceProxy.Height, 0);
            }
        }
    }
}
