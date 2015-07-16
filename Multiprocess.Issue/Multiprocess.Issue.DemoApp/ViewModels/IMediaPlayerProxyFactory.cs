using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multiprocess.Issue.ViewModels
{
    using Motorola.IVS.Client;

    public interface IMediaPlayerProxyFactory
    {
        /// <summary>
        /// Returns an externally-hosted instance of MediaPlayer.
        /// </summary>
        /// <param name="channelUri">The URI for the desired stream.</param>
        /// <param name="externalProcessId">
        /// The process ID of the ProcessHost to force re-use of. Only supply this parameter if you specifically
        /// want to re-use a Viewer.ProcessHost instance (such as going Back 10 from live, or "Back to live").
        /// </param>
        /// <returns>The <see cref="IMediaPlayer"/> containing the desired stream.</returns>
        IMediaPlayer GetPlayerInstance(string channelUri, int externalProcessId = 0);
    }
}
