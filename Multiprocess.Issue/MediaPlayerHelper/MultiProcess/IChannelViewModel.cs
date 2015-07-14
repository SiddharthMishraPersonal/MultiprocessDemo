using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerHelper.MultiProcess
{
    /// <summary>
    /// The play state.
    /// </summary>
    public enum PlayState
    {
        /// <summary>
        ///   The playing.
        /// </summary>
        Playing,

        /// <summary>
        ///   The not playing.
        /// </summary>
        NotPlaying,

        /// <summary>
        ///   Play state is changing
        /// </summary>
        Changing,
    }
}
