// -----------------------------------------------------------------------
// <copyright file="IProcessHostServiceContract.cs" company="Motorola Solutions, Inc.">
//   Copyright (C) 2015 Motorola Solutions, Inc.
// </copyright>
// -----------------------------------------------------------------------

namespace Motorola.IVS.Client.Viewer.ProcessHost.Communication
{
    using System.ServiceModel;

    /// <summary>
    /// Interface to define the communication between RIC and Viewer.ProcessHost.exe.
    /// </summary>
    [ServiceContract]
    public interface IProcessHostServiceContract
    {
        /// <summary>
        /// Spawns a new MediaPlayerServer instance and returns a URI for it.
        /// </summary>
        /// <returns>The URI for communication with the new MediaPlayerServer instance.</returns>
        [OperationContract]
        string GetMediaPlayerProxyChannelUri();

        /// <summary>
        /// Closes the specified MediaPlayer.
        /// </summary>
        /// <param name="mediaPlayerUri">The URI identifying the player to close.</param>
        /// <param name="retainProcess">
        /// Whether to retain the process even if this is the last open player.
        /// </param>
        [OperationContract(IsOneWay = true)]
        void CloseMediaPlayer(string mediaPlayerUri, bool retainProcess);
    }
}