//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace MultiProcess.Client
//{
//    /// <summary>
//    /// The player channel state.
//    /// </summary>
//    public enum PlayerChannelState
//    {
//        /// <summary>
//        /// The closed.
//        /// </summary>
//        Closed,

//        /// <summary>
//        /// The opening.
//        /// </summary>
//        Opening,

//        /// <summary>
//        /// The opened.
//        /// </summary>
//        Opened,
//    }

//    /// <summary>
//    /// The send link delegate.
//    /// </summary>
//    /// <param name="channelDatas">
//    /// The channel datas.
//    /// </param>
//    /// <param name="users">
//    /// The users.
//    /// </param>
//    /// <param name="message">
//    /// The message.
//    /// </param>
//    public delegate void SendLinkDelegate(List<IChannelData> channelDatas, List<string> users, string message);

//    /// <summary>
//    /// The player channel state changed event handler.
//    /// </summary>
//    /// <param name="playerChannel">
//    /// The player channel.
//    /// </param>
//    public delegate void PlayerChannelStateChangedEventHandler(IPlayerChannel playerChannel);

//    /// <summary>
//    /// The PlayerChannel interface.
//    /// </summary>
//    public interface IPlayerChannel : IPlayerHostChannel, IDisposable
//    {
//        /// <summary>
//        ///  The PTZ arbitration state (Control/NoControl/NoOwner)
//        /// </summary>
//        PtzControlArbitrationState PtzArbitrationState { get; set; }

//        /// <summary>
//        /// Gets the arbitration agent.
//        /// </summary>
//        PtzArbitrationAgent ArbitrationAgent { get; }

//        /// <summary>
//        /// Gets or sets the send link.
//        /// </summary>
//        SendLinkDelegate SendLink { get; set; }

//        /// <summary>
//        /// The add clip to inbox.
//        /// </summary>
//        /// <param name="channelData">
//        /// The channel data.
//        /// </param>
//        void AddClipToInbox(IChannelData channelData);

//        /// <summary>
//        ///   The current status of the media player
//        ///   Setting this property will raise the player status changed event
//        /// </summary>
//        PlayerChannelState CurrentPlayerStatus { get; }

//        /// <summary>
//        ///   Gets or sets ChannelContainer.
//        /// </summary>
//        IChannelViewModel ChannelContainer { get; set; }

//        /// <summary>
//        /// Gets the player window.
//        /// </summary>
//        IPlayerWindow PlayerWindow { get; }

//        /// <summary>
//        /// Gets the channel name.
//        /// </summary>
//        string ChannelName { get; }

//        /// <summary>
//        ///   Gets or sets a value indicating whether HasError.
//        /// </summary>
//        bool HasError { get; set; }

//        /// <summary>
//        ///   Gets or sets a value indicating whether IsPlaying.
//        /// </summary>
//        bool IsPlaying { get; set; }

//        /// <summary>
//        ///  Gets the StreamingChannel to play
//        /// </summary>
//        IChannelData Channel { get; set; }

//        /// <summary>
//        ///   Gets or sets a value indicating whether IsSelected.
//        /// </summary>
//        bool IsSelected { get; set; }

//        /// <summary>
//        ///  Gets or sets Current WindowPosition associated with the PlayerChannel
//        /// </summary>
//        WindowPosition WindowPosition { get; set; }

//        /// <summary>
//        /// Gets or sets the preview position.
//        /// </summary>
//        PreviewPosition PreviewPosition { get; set; }

//        /// <summary>
//        /// The set selected channel.
//        /// </summary>
//        void SetSelectedChannel();

//        /// <summary>
//        /// The set player window.
//        /// </summary>
//        /// <param name="playerWindow">
//        /// The player window.
//        /// </param>
//        void SetPlayerWindow(IPlayerWindow playerWindow);

//        /// <summary>
//        /// The reconnect.
//        /// </summary>
//        void Reconnect();

       

//        /// <summary>
//        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
//        /// </summary>
//        /// <filterpriority>2</filterpriority>
//        void Dispose();

        

//        /// <summary>
//        ///   The player status changed.
//        /// </summary>
//        event PlayerChannelStateChangedEventHandler PlayerStatusChanged;

//        /// <summary>
//        /// The player channel updated.
//        /// </summary>
//        event EventHandler PlayerChannelUpdated;

//        /// <summary>
//        ///   The is selected changed.
//        /// </summary>
//        event EventHandler IsSelectedChanged;

//        /// <summary>
//        /// The un select.
//        /// </summary>
//        void UnSelect();

//        /// <summary>
//        /// The play.
//        /// </summary>
//        void Play();

//        /// <summary>
//        /// The stop.
//        /// </summary>
//        void Stop();

//        /// <summary>
//        /// This event fires up when a stream is been open for a long time in order to warn the user
//        /// </summary>
//        event EventHandler LongStreamingSessionAlert;

//        /// <summary>
//        /// In case of globally mute is enabled the property must be true
//        /// </summary>
//        bool IsGloballyMuteEnabled { get; set; }
//        /// <summary>
//        /// Once the instance of the PlayerChannel is initialized this flag is 
//        /// set to true to avoid re-subscribing to delegates
//        /// </summary>
//        bool IsInitialized { get; set; }
//    }
//}
