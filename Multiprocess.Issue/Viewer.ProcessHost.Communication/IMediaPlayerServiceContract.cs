// -----------------------------------------------------------------------
// <copyright file="IMediaPlayerServiceContract.cs" company="Motorola Solutions, Inc.">
//   Copyright (C) 2015 Motorola Solutions, Inc.
// </copyright>
// -----------------------------------------------------------------------

namespace Motorola.IVS.Client.Viewer.ProcessHost.Communication
{
    using System;
    using System.ServiceModel;

    using MultiProcess.Client;

    /// <summary>
    /// Interface to define the communication between a video plugin's media player and the hosting application.
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IMediaPlayerServiceEventContract))]
    public interface IMediaPlayerServiceContract
    {
        /// <summary>
        /// Gets the current position.
        /// </summary>
        /// <returns>The current position.</returns>
        int CurrentPosition
        {
            [OperationContract]
            get;
        }

        /// <summary>
        /// Gets the current timestamp from the VMS (if live).
        /// </summary>
        /// <returns>Returns the current live timestamp for the VMS.</returns>
        /// <remarks>
        /// The purpose of this method is to deal with clock differences
        /// between the client machine and the VMS server. This essentially returns
        /// the current time on the server, allowing us to make necessary adjustments.
        /// </remarks>
        DateTimeOffset CurrentLiveTimestamp
        {
            [OperationContract]
            get;
        }

        /// <summary>
        /// Gets the maximum buffer size.
        /// </summary>
        /// <returns>The maximum buffer size in seconds.</returns>
        int MaxBufferSize
        {
            [OperationContract]
            get;
        }

        /// <summary>
        /// Gets the current buffer size.
        /// </summary>
        /// <returns>The current buffer size in seconds.</returns>
        int CurrentBufferSize
        {
            [OperationContract]
            get;
        }

        /// <summary>
        /// Gets a value indicating whether player is digital zoom capable.
        /// </summary>
        bool DigitalZoomCapable
        {
            [OperationContract] 
            get;
        }

        /// <summary>
        /// Play live stream
        /// </summary>
        /// <returns>
        /// Play succeeded
        /// </returns>
        [OperationContract]
        bool Play();

        /// <summary>
        /// Play recorded stream.
        /// </summary>
        /// <param name="startTime">The <see cref="DateTime"/> for the starting-point of the stream.</param>
        /// <param name="endTime">The <see cref="DateTime"/> for the ending-point of the stream.</param>
        /// <returns>A value indicating whether the operation succeeded.</returns>
        [OperationContract(Name = "PlayRecorded")]
        bool Play(DateTime startTime, DateTime endTime);

        /// <summary>
        /// Stop the video stream.
        /// </summary>
        [OperationContract]
        void Stop();

        /// <summary>
        /// Pause the video stream.
        /// </summary>
        [OperationContract]
        void Pause();

        /// <summary>
        /// Fast-forwards the video stream at the specified speed.
        /// </summary>
        /// <param name="speed">
        /// The <see cref="PlayerSpeed"/> at which to fast-forward.
        /// </param>
        /// <returns>
        /// Whether the operation succeeded.
        /// </returns>
        [OperationContract]
        bool FastForward(PlayerSpeed speed);

        /// <summary>
        /// Rewinds the video stream at the specified speed.
        /// </summary>
        /// <param name="speed">
        /// The <see cref="PlayerSpeed"/> at which to rewind.
        /// </param>
        /// <returns>
        /// Whether the operation succeeded.
        /// </returns>
        [OperationContract]
        bool FastRewind(PlayerSpeed speed);

        /// <summary>
        /// Seeks to the specified number of seconds before the current position.
        /// </summary>
        /// <param name="secondsBack">How many seconds to go back.</param>
        [OperationContract]
        void Seek(int secondsBack);

        /// <summary>
        /// Creates a snapshot of the video stream with the specified path/filename.
        /// </summary>
        /// <param name="filename">The full path/name of the snapshot file.</param>
        [OperationContract]
        void TakeSnapshot(string filename);

        /// <summary>
        /// Calls SetupPlayerObject on the IMediaPlayer, but returns a handle rather than the actual UIElement.
        /// </summary>
        /// <param name="uri">
        /// The uri of the video stream.
        /// </param>
        /// <param name="mediaType">
        /// The media Type.
        /// </param>
        /// <returns>
        /// The <see cref="IntPtr"/> handle to the player window.
        /// </returns>
        [OperationContract]
        IntPtr SetupPlayerObject(string uri, MediaType mediaType);

        /// <summary>
        /// Sets-up and persists the callback channel on the "server".
        /// </summary>
        [OperationContract]
        void SetupCallbackChannel();

        /// <summary>
        /// Moves the camera to the indicated preset.
        /// </summary>
        /// <param name="channelId">The channel of the camera.</param>
        /// <param name="presetId">The preset to move to.</param>
        /// <returns>Whether the operation succeeded.</returns>
        [OperationContract]
        bool GoToPreset(string channelId, int presetId);

        /// <summary>
        /// Starts a pan/tilt operation.
        /// </summary>
        /// <param name="channelId">The channel of the camera.</param>
        /// <param name="pan">The pan value.</param>
        /// <param name="tilt">The tilt value.</param>
        /// <returns>Whether the operation succeeded.</returns>
        [OperationContract]
        bool StartPanTilt(string channelId, int pan, int tilt);

        /// <summary>
        /// Starts a zoom operation.
        /// </summary>
        /// <param name="channelId">The channel of the camera.</param>
        /// <param name="zoomDirection">The direction to zoom in.</param>
        /// <param name="zoomStrength">The speed to zoom at.</param>
        /// <returns>Whether the operation succeeded.</returns>
        [OperationContract]
        bool StartZoom(string channelId, bool zoomDirection, int zoomStrength);

        /// <summary>
        /// Stops a pan/tilt operation.
        /// </summary>
        /// <param name="channelId">The channel of the camera.</param>
        /// <returns>Whether the operation succeeded.</returns>
        [OperationContract]
        bool StopPanTilt(string channelId);

        /// <summary>
        /// Stops a zoom operation.
        /// </summary>
        /// <param name="channelId">The channel of the camera.</param>
        /// <returns>Whether the operation succeeded.</returns>
        [OperationContract]
        bool StopZoom(string channelId);

        /// <summary>
        /// Sets the digital zoom window.
        /// </summary>
        /// <param name="left">The left coordinate of digitalZoom rectangle</param>
        /// <param name="top">The top coordinate of digitalZoom rectangle</param>
        /// <param name="width">The width of digitalZoom rectangle</param>
        /// <param name="height">The height of digitalZoom rectangle</param>
        [OperationContract]
        void SetDigitalZoomWindow(int left, int top, int width, int height);

        /// <summary>
        /// Gets the width.
        /// </summary>
        double Width
        {
            [OperationContract]
            get;
        }

        /// <summary>
        /// Gets the height.
        /// </summary>
        double Height
        {
            [OperationContract]
            get;
        }
    }
}