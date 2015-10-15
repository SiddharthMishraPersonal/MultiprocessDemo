using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiProcess.MediaPlayerHelper
{
    using System.Diagnostics;
    using System.ServiceModel;
    using System.Windows;
    using System.Windows.Threading;

    using Motorola.IVS.Client.Viewer.ProcessHost.Communication;

    using MultiProcess.Client;
    using MultiProcess.MediaPlayerHelper.HostedContents;

    using Telerik.Windows.Controls.Docking;

    public class MediaPlayerProxy : IMediaPlayer, IMediaPlayerServiceEventContract
    {
        private Uri videoUri;

        /// <summary>
        /// How many milliseconds to wait for the external process to finish starting.
        /// </summary>
        private const int ProcessTimeout = 20000;

        /// <summary>
        /// The external ProcessHost.
        /// </summary>
        private readonly Process externalProcess;

        /// <summary>
        /// Fires when the streaming status of the video stream has changed.
        /// </summary>
        public event StreamingStatusEventHanlder StreamingStatusChanged;

        /// <summary>
        /// The URI of the desired video stream.
        /// </summary>
        private string channelUri;

        /// <summary>
        /// The URI to the remote media player endpoint.
        /// </summary>
        private string playerProxyUri;

        /// <summary>
        /// Whether this instance of MediaPlayerProxy is currently exiting.
        /// </summary>
        private bool exiting;


        /// <summary>
        /// Lock to prevent threading issues.
        /// </summary>
        private readonly object setupLock = new object();


        /// <summary>
        /// The object for communication with the process host service.
        /// </summary>
        private IProcessHostServiceContract processHost;

        /// <summary>
        /// The object for communication with the remote media player.
        /// </summary>
        private IMediaPlayerServiceContract mediaPlayerServiceProxy;

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

        public MediaPlayerProxy(Process processHost, Uri videoUri)
        {
            this.videoUri = videoUri;
            this.externalProcess = processHost;
            processHost.Exited += this.OnHostedProcessExited;

        }

        /// <summary>
        /// Handles Exited event of the ProcessHost. Raises OnStreamingStatusChanged event.
        /// </summary>
        /// <param name="sender">The sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnHostedProcessExited(object sender, System.EventArgs e)
        {
            this.OnStreamingStatusChanged(ConnectionStatus.Disconnected, string.Empty);
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
            var uri = "net.pipe://localhost/viewer.processhost/{0}";
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

            }
            catch (TimeoutException ex)
            {

            }
            catch (Exception ex)
            {

            }

            return false;
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
        /// The setup player object.
        /// </summary>
        /// <param name="mediaType">
        /// The media type.
        /// </param>
        /// <returns>
        /// The <see cref="UIElement"/>.
        /// </returns>
        public UIElement SetupPlayerObject(MediaType  mediaType)
        {
            lock (this.setupLock)
            {
                HostedContent hostedContent = null;

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

                var hostHandle = this.SafeChannelCall(() => this.mediaPlayerServiceProxy.SetupPlayerObject(this.channelUri, mediaType), IntPtr.Zero);


                if (Application.Current != null)
                {
                    hostedContent = Application.Current.Dispatcher.Invoke(
                            () =>
                            {
                                try
                                {
                                    return new HostedContent() { ExternalHandle = hostHandle };
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
        /// <returns>The return value of the passed-in function.</returns>                                                                ,
        private TResult SafeChannelCall<TResult>(Func<TResult> action, TResult defaultReturn)
        {
            try
            {
                return action();
            }
            catch (TimeoutException ex)
            {
                throw;
            }
            catch (CommunicationException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }

            this.OnStreamingStatusChanged(ConnectionStatus.Disconnected, string.Empty);
            return defaultReturn;
        }

        private HostedContent CreateHostedContent()
        {
            try
            {
                var hostHandle = IntPtr.Zero;
                return new HostedContent() { ExternalHandle = hostHandle };
            }
            catch (OutOfMemoryException ex)
            {
                // TODO: Need to determine root cause for this. This is raised by Thread.Start in the STATaskScheduler 
                // TODO: constructor in HostedContent.


                return null;
            }
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

        public bool Play()
        {
            bool played = false;
            try
            {
                played = this.SafeChannelCall(() => this.mediaPlayerServiceProxy.Play(), false);
            }
            catch (Exception)
            {

                this.EstablishProcessHostChannel();
            }

            return played;
        }

        public void Pause()
        {
            this.SafeChannelCall(this.mediaPlayerServiceProxy.Pause);
        }

        public void Stop()
        {
            this.SafeChannelCall(this.mediaPlayerServiceProxy.Stop);
        }

        public void Dispose()
        {

        }


        public bool Play(DateTime startTime, DateTime endTime)
        {
            throw new NotImplementedException();
        }

        public bool FastForward(PlayerSpeed speed)
        {
            throw new NotImplementedException();
        }

        public bool FastRewind(PlayerSpeed speed)
        {
            throw new NotImplementedException();
        }

        public PlayDirection PlayDirection
        {
            get { throw new NotImplementedException(); }
        }

        public void Seek(int secondsBack)
        {
            throw new NotImplementedException();
        }

        public int CurrentPosition
        {
            get { throw new NotImplementedException(); }
        }

        public bool DigitalZoomCapable
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

        public void TakeSnapshot(string fileName)
        {
            throw new NotImplementedException();
        }

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

        public void SetFullscreen()
        {
            throw new NotImplementedException();
        }

        public bool Initialize(string uri)
        {
            this.channelUri = uri;
            return true;
        }

        public bool IsSDKInstalled()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<StreamingStaticsEventArgs> StreamingStatisticsUpdated;

        public event SelectEventHandler Selected;

        public event DragStartEventHandler DragStart;

        public event DragEventHandler Drop;

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


        public void OnMouseEnter()
        {

        }

        public void OnMouseLeave()
        {

        }
    }
}
