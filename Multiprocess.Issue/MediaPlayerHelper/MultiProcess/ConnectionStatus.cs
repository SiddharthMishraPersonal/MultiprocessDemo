using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MediaPlayerHelper.MultiProcess
{
    /// <summary>
    /// The connection status.
    /// </summary>
    public enum ConnectionStatus
    {
        /// <summary>
        /// The streaming.
        /// </summary>
        Streaming,

        /// <summary>
        /// The disconnected.
        /// </summary>
        Disconnected,

        /// <summary>
        /// The interrupted.
        /// </summary>
        Interrupted,

        /// <summary>
        /// The failed to initialize.
        /// </summary>
        FailedToInitialize,

        /// <summary>
        /// The un authorized.
        /// </summary>
        UnAuthorized,

        /// <summary>
        /// The channel not found.
        /// </summary>
        ChannelNotFound,

        /// <summary>
        /// The admin terminated.
        /// </summary>
        AdminTerminated,

        /// <summary>
        /// The recording not found.
        /// </summary>
        RecordingNotFound,

        /// <summary>
        /// The media inactivity timeout.
        /// </summary>
        MediaInactivityTimeout,

        /// <summary>
        /// The session timeout.
        /// </summary>
        SessionTimeout,

        /// <summary>
        /// Interruption for 30 seconds or longer
        /// </summary>
        LongInterruption,

        /// <summary>
        /// The default, initial, cleared state.
        /// </summary>
        Idle,

        /// <summary>
        /// The invalid license state.
        /// </summary>
        InvalidLicense,
    }
}
