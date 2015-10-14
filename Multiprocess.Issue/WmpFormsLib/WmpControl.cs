// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WmpControl.cs" company="Motorola Solutions">
//  For Demo Purpose 
// </copyright>
// <summary>
//   The play event handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WmpFormsLib
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;

    using AxWMPLib;

    /// <summary>
    /// The play event handler.
    /// </summary>
    public delegate void PlayEventHandler();

    /// <summary>
    /// The stop event hander.
    /// </summary>
    public delegate void StopEventHander();

    /// <summary>
    /// The error occurred event handler.
    /// </summary>
    /// <param name="exception">
    /// The exception.
    /// </param>
    public delegate void ErrorOccurredEventHandler(Exception exception);

    /// <summary>
    /// The player state changed handler.
    /// </summary>
    /// <param name="newState">
    /// The new state.
    /// </param>
    public delegate void PlayerStateChangedHandler(WMPLib.WMPPlayState newState);

    /// <summary>
    /// The Window Media Player control.
    /// </summary>
    public partial class WmpControl : UserControl
    {
        /// <summary>
        /// The error occurred.
        /// </summary>
        private bool errorOccurred;

        /// <summary>
        /// Initializes a new instance of the <see cref="WmpControl"/> class.
        /// </summary>
        public WmpControl()
        {
            this.InitializeComponent();
            if (this.axWindowsMediaPlayer == null)
            {
                this.axWindowsMediaPlayer = new AxWindowsMediaPlayer();
            }

            this.axWindowsMediaPlayer.uiMode = "none";
            this.axWindowsMediaPlayer.settings.setMode("loop", false);
            this.axWindowsMediaPlayer.stretchToFit = true;
            this.axWindowsMediaPlayer.enableContextMenu = false;
            this.axWindowsMediaPlayer.ErrorEvent += this.AxWindowsMediaPlayerOnErrorEvent;
            this.axWindowsMediaPlayer.PlayStateChange += this.AxWindowsMediaPlayerOnPlayStateChange;
        }

        /// <summary>
        /// The on playing.
        /// </summary>
        public event PlayEventHandler OnPlaying;

        /// <summary>
        /// The on stopped.
        /// </summary>
        public event StopEventHander OnStopped;

        /// <summary>
        /// The on error occurred.
        /// </summary>
        public event ErrorOccurredEventHandler OnErrorOccurred;

        /// <summary>
        /// The on player state changed.
        /// </summary>
        public event PlayerStateChangedHandler OnPlayerStateChanged;
        
        /// <summary>
        /// The play.
        /// </summary>
        /// <param name="uri">
        /// The video URL.
        /// </param>
        public void Play(Uri uri)
        {
            this.axWindowsMediaPlayer.URL = uri.AbsoluteUri;
            this.axWindowsMediaPlayer.Ctlcontrols.play();
            if (!this.errorOccurred)
            {
                this.OnPlaying();
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.errorOccurred = false;
            this.axWindowsMediaPlayer.Ctlcontrols.stop();
            if (!this.errorOccurred)
            {
                this.OnStopped();
            }
        }

        /// <summary>
        /// The pause.
        /// </summary>
        public void Pause()
        {
            this.errorOccurred = false;
            this.axWindowsMediaPlayer.Ctlcontrols.pause();
        }

        /// <summary>
        /// The resume.
        /// </summary>
        public void Resume()
        {
            this.errorOccurred = false;
            this.axWindowsMediaPlayer.Ctlcontrols.play();
        }

        /// <summary>
        /// The exit.
        /// </summary>
        public void Exit()
        {
            this.axWindowsMediaPlayer.Ctlcontrols.stop();
            this.axWindowsMediaPlayer.close();
            this.axWindowsMediaPlayer.Dispose();
        }

        /// <summary>
        /// The ax windows media player on play state change.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="wmpocxEventsPlayStateChangeEvent">
        /// The play state change event.
        /// </param>
        private void AxWindowsMediaPlayerOnPlayStateChange(object sender, _WMPOCXEvents_PlayStateChangeEvent wmpocxEventsPlayStateChangeEvent)
        {
            var state = (WMPLib.WMPPlayState)wmpocxEventsPlayStateChangeEvent.newState;
            if (this.OnPlayerStateChanged != null)
            {
                this.OnPlayerStateChanged(state);
            }
        }

        /// <summary>
        /// The ax windows media player on error event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="eventArgs">
        /// The event args.
        /// </param>
        private void AxWindowsMediaPlayerOnErrorEvent(object sender, EventArgs eventArgs)
        {
            this.errorOccurred = true;

            if (this.OnErrorOccurred != null)
            {
                this.OnErrorOccurred(new Exception(this.axWindowsMediaPlayer.Error.Item[0].errorDescription));
            }
        }
    }
}
