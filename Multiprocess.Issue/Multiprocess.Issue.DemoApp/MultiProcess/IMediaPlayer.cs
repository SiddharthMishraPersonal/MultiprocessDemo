using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multiprocess.Issue.MultiProcess
{
    using System.Windows;

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
        /// The pause.
        /// </summary>
        void Pause();

        /// <summary>
        /// Stop operation. This will also send teardown in case of rtsp playback.
        /// </summary>
        void Stop();
    }

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
}
