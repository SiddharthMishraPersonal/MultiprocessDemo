using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerHelper.IVS.Client
{
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// The streaming statistics.
    /// </summary>
    [DataContract(Name = "StreamingStatistics")]
    public class StreamingStatistics
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamingStatistics"/> class.
        /// </summary>
        public StreamingStatistics()
        {
            Resolution = new Resolution();
            LostPackets = new LostPackets();
            LatePackets = new LatePackets();
        }

        /// <summary>
        /// Gets or sets the average bit rate.
        /// </summary>
        [DataMember(Name = "AverageBitRate", EmitDefaultValue = false, Order = 1)]
        public int AverageBitRate { get; set; }

        /// <summary>
        /// Gets or sets the maximum bit rate.
        /// </summary>
        public int MaximumBitRate { get; set; }

        /// <summary>
        /// Gets or sets the average frame rate.
        /// </summary>
        [DataMember(Name = "AverageFrameRate", EmitDefaultValue = false, Order = 2)]
        public int AverageFrameRate { get; set; }

        /// <summary>
        /// Gets or sets the maximum frame rate.
        /// </summary>
        public int MaximumFrameRate { get; set; }

        /// <summary>
        /// Gets or sets the frames skipped.
        /// </summary>
        public int FramesSkipped { get; set; }

        /// <summary>
        /// Gets or sets the resolution.
        /// </summary>
        public Resolution Resolution { get; set; }

        /// <summary>
        /// Gets or sets the lost packets.
        /// </summary>
        [DataMember(Name = "LostPackets", EmitDefaultValue = false, Order = 3)]
        public LostPackets LostPackets { get; set; }

        /// <summary>
        /// Gets or sets the late packets.
        /// </summary>
        [DataMember(Name = "LatePackets", EmitDefaultValue = false, Order = 3)]
        public LatePackets LatePackets { get; set; }
    }

    /// <summary>
    /// The resolution.
    /// </summary>
    public class Resolution
    {
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int Height { get; set; }
    }

    /// <summary>
    /// The lost packets.
    /// </summary>
    [DataContract(Name = "LostPackets")]
    public class LostPackets
    {
        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        [XmlAttribute(AttributeName = "count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the percent.
        /// </summary>
        [XmlAttribute(AttributeName = "percent")]
        public double Percent { get; set; }
    }

    /// <summary>
    /// The late packets.
    /// </summary>
    [DataContract(Name = "LatePackets")]
    public class LatePackets
    {
        /// <summary>
        /// Gets or sets the accepted.
        /// </summary>
        [XmlAttribute(AttributeName = "accepted")]
        public int Accepted { get; set; }

        /// <summary>
        /// Gets or sets the dropped.
        /// </summary>
        [XmlAttribute(AttributeName = "dropped")]
        public int Dropped { get; set; }
    }

    /// <summary>
    /// The streaming statics event args.
    /// </summary>
    public class StreamingStaticsEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamingStaticsEventArgs"/> class.
        /// </summary>
        /// <param name="streamingStatistics">
        /// The streaming statistics.
        /// </param>
        public StreamingStaticsEventArgs(StreamingStatistics streamingStatistics)
        {
            StreamingStatistics = streamingStatistics;
        }

        /// <summary>
        /// Gets the streaming statistics.
        /// </summary>
        public StreamingStatistics StreamingStatistics { get; private set; }
    }
}
