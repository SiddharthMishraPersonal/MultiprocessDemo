using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaPlayerHelper.MultiProcess
{
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Threading;

    using MediaPlayerHelper.HostedContents;

    using Telerik.Windows.Controls.Docking;

    public class MediaPlayerProxy : IMediaPlayer
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
                                                    
        public System.Windows.UIElement SetupPlayerObject()
        {
            HostedContent hostedContent = null;

            //// Wait for process to finish initialization.
            //try
            //{
            //    if (!this.externalProcess.WaitForInputIdle(ProcessTimeout))
            //    {
            //        this.externalProcess.Kill();
            //        return null;
            //    }
            //}
            //catch (InvalidOperationException)
            //{
            //    /* This condition can happen if we're waiting for InputIdle when the process is closed due to the user closing the video panes.
            //     * This is an expected result of using WaitForInputIdle. An alternative method for determining when the process is ready for
            //     * communication may be more desirable. */
            //    return null;
            //}


            var hostHandle = IntPtr.Zero;




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
            //this.Exit(false);
        }

        ///// <summary>
        ///// Stops the video stream and cleans up used resources.
        ///// </summary>
        ///// <param name="retainProcessHost">
        ///// Whether to retain ProcessHost even if this is the last MediaPlayer in use. (RIC only)
        ///// </param>
        //public void Exit(bool retainProcessHost)
        //{
        //    this.exiting = true;
        //    lock (this.setupLock)
        //    {
        //        this.SafeChannelCall(() => this.processHost.CloseMediaPlayer(this.playerProxyUri, retainProcessHost));

        //        // ReSharper disable SuspiciousTypeConversion.Global (These interfaces are WCF channels)
        //        var mediaChannel = this.mediaPlayerServiceProxy as System.ServiceModel.Channels.IChannel;
        //        if (mediaChannel != null)
        //        {
        //            mediaChannel.Close();
        //        }

        //        var hostChannel = this.processHost as System.ServiceModel.Channels.IChannel;
        //        if (hostChannel != null)
        //        {
        //            hostChannel.Close();
        //        }

        //        // ReSharper restore SuspiciousTypeConversion.Global
        //        this.mediaPlayerServiceProxy = null;
        //        this.processHost = null;
        //    }
        //}


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

            return true;
        }

        public void Pause()
        {

        }

        public void Stop()
        {

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

        public IVS.Client.Volume Volume
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

        public void Exit(bool retainProcessHost = false)
        {
            throw new NotImplementedException();
        }

        public bool Initialize(string uri)
        {
            throw new NotImplementedException();
        }

        public bool IsSDKInstalled()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<IVS.Client.StreamingStaticsEventArgs> StreamingStatisticsUpdated;

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
    }
}
