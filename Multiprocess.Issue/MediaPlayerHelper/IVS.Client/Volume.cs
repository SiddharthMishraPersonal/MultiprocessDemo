using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerHelper.IVS.Client
{
    /// <summary>
    /// The volume.
    /// </summary>
    public enum Volume
    {
        /// <summary>
        /// The mute.
        /// </summary>
        Mute = 0,

        /// <summary>
        /// The one third.
        /// </summary>
        OneThird = 33,

        /// <summary>
        /// The two thirds.
        /// </summary>
        TwoThirds = 66,

        /// <summary>
        /// The max.
        /// </summary>
        Max = 100,
    }

    /// <summary>
    /// The volume state.
    /// </summary>
    public static class VolumeState
    {
        /// <summary>
        /// The get next state.
        /// </summary>
        /// <param name="volume">
        /// The volume.
        /// </param>
        /// <returns>
        /// The <see cref="Volume"/>.
        /// </returns>
        public static Volume GetNextState(Volume volume)
        {
            if (volume == Volume.Mute)
            {
                return Volume.OneThird;
            }
            else if (volume == Volume.OneThird)
            {
                return Volume.TwoThirds;
            }
            else if (volume == Volume.TwoThirds)
            {
                return Volume.Max;
            }

            return Volume.Mute;
        }
    }
}
