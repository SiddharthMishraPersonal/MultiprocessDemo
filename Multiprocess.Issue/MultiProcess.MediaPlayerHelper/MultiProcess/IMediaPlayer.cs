using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiProcess.MediaPlayerHelper
{
    using System.Windows;

    using global::MultiProcess.Client.EventArg;

    using MultiProcess.Client;

    /// <summary>
    /// The player speed.
    /// </summary>
    public enum PlayerSpeed
    {
        /// <summary>
        /// The two x spped.
        /// </summary>
        TwoX,

        /// <summary>
        /// The three x speed.
        /// </summary>
        ThreeX,

        /// <summary>
        /// The four x speed.
        /// </summary>
        FourX,
    }

    /// <summary>
    /// The play direction.
    /// </summary>
    public enum PlayDirection
    {
        /// <summary>
        /// The forward direction.
        /// </summary>
        Forward,

        /// <summary>
        /// The reverse direction.
        /// </summary>
        Reverse,
    }

    /// <summary>
    /// This interface defines the player operations to be called from client.
    /// </summary>
    public interface IMediaPlayer : IDisposable
    {
        /// <summary>
        /// Setup the player object
        /// </summary>
        /// <returns>
        /// Player UI element
        /// </returns>
        UIElement SetupPlayerObject();

        /// <summary>
        /// Play live stream (MSTG only player)
        /// </summary>
        /// <returns>
        /// Play succeeded
        /// </returns>
        bool Play();

        /// <summary>
        /// Play recorded stream
        /// </summary>
        /// <param name="startTime">
        /// Start time for recorded
        /// </param>
        /// <param name="endTime">
        /// End time for recorded
        /// </param>
        /// <returns>
        /// Play succeeded
        /// </returns>
        bool Play(DateTime startTime, DateTime endTime);

        

        

        /// <summary>
        /// The pause.
        /// </summary>
        void Pause();

        /// <summary>
        /// Stop operation. This will also send teardown in case of rtsp playback.
        /// </summary>
        void Stop();

       
        /// <summary>
        /// The take snapshot.
        /// </summary>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        void TakeSnapshot(string fileName);

        
        /// <summary>
        /// To clean up any used resources.
        /// </summary>
        /// <param name="retainProcessHost">
        /// Whether to retain the ProcessHost instance after this exit (RIC ONLY).
        /// </param>
        void Exit(bool retainProcessHost = false);

        /// <summary>
        /// Initialize with URI or other paramters
        /// </summary>
        /// <param name="uri">
        /// The uri.
        /// </param>
        /// <returns>
        /// The initialize.
        /// </returns>
        bool Initialize(string uri);

        /// <summary>
        /// Checks whether SDK is installed or not
        /// </summary>
        /// <returns></returns>

        

        /// <summary>
        /// The streaming statistics updated.
        /// </summary>
        event EventHandler<StreamingStaticsEventArgs> StreamingStatisticsUpdated;

        /// <summary>
        ///   Indicates the status of a streaming channel for this media player
        /// </summary>
        event StreamingStatusEventHanlder StreamingStatusChanged;

        /// <summary>
        ///   This event is to handle the select event on the player object.
        /// </summary>
        event SelectEventHandler Selected;

        /// <summary>
        ///   This event is to handle the drag event on the player object.
        /// </summary>
        event DragStartEventHandler DragStart;

        /// <summary>
        ///   This event is to handle the drop event on the player object.
        /// </summary>
        event DragEventHandler Drop;

        /// <summary>
        /// The player error.
        /// </summary>
        event PlayerErrorEventHandler PlayerError;

        /// <summary>
        /// The play state changed.
        /// </summary>
        event PlayStateChangedEventHandler PlayStateChanged;

        /// <summary>
        /// The play position changed.
        /// </summary>
        event PlayPositionChangedEventHandler PlayPositionChanged;

        /// <summary>
        /// Gets the width.
        /// </summary>
        double Width { get; }

        /// <summary>
        /// Gets the height.
        /// </summary>
        double Height { get; }


    }

    /// <summary>
    /// The play position changed event handler.
    /// </summary>
    /// <param name="secondsBackFromEnd">
    /// The seconds back from end.
    /// </param>
    public delegate void PlayPositionChangedEventHandler(int secondsBackFromEnd);

    /// <summary>
    /// The play state changed event handler.
    /// </summary>
    /// <param name="playState">
    /// The play state.
    /// </param>
    public delegate void PlayStateChangedEventHandler(PlayState playState);

    /// <summary>
    /// The streaming status event handler.
    /// </summary>
    /// <param name="status">
    /// The status.
    /// </param>
    /// <param name="errorCode">
    /// The errorCode.
    /// </param>
    public delegate void StreamingStatusEventHanlder(ConnectionStatus status, string errorCode);

    /// <summary>
    /// The select event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public delegate void SelectEventHandler(object sender, EventArgs e);

    /// <summary>
    /// The drag start event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public delegate void DragStartEventHandler(object sender, EventArgs e);

    /// <summary>
    /// The player error event handler.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    public delegate void PlayerErrorEventHandler(object sender, PlayerErrorEventArgs e);
}
