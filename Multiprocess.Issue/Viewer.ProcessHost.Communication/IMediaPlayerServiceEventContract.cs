// -----------------------------------------------------------------------
// <copyright file="IMediaPlayerServiceEventContract.cs" company="Motorola Solutions, Inc.">
//   Copyright (C) 2015 Motorola Solutions, Inc.
// </copyright>
// -----------------------------------------------------------------------

namespace Motorola.IVS.Client.Viewer.ProcessHost.Communication
{
    using System.ServiceModel;

    using Motorola.IVS.Client;
    using MediaPlayerHelper.MultiProcess;

    /// <summary>
    /// This communication contract consists of events fired by the IMediaPlayer that need to be published back to the hosting application.
    /// This is the <see cref="ServiceContractAttribute.CallbackContract"/> for <see cref="IMediaPlayerServiceContract"/>./>
    /// </summary>
    public interface IMediaPlayerServiceEventContract
    {
        /// <summary>
        /// Indicates the status of a streaming channel for the media player
        /// </summary>
        /// <param name="status">The new status of the player.</param>
        /// <param name="errorCode">Error code associated with the status (if any ).</param>
        [OperationContract(IsOneWay = true)]
        void OnStreamingStatusChanged(ConnectionStatus status, string errorCode);

        /// <summary>
        /// Fired when the mouse enters the video area.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void OnMouseEnter();

        /// <summary>
        /// Fired when the mouse exits the video area.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void OnMouseLeave();
    }
}