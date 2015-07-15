using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiProcess.Client.EventArg
{
    /// <summary>
    /// The player error event args.
    /// </summary>
    public class PlayerErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerErrorEventArgs"/> class.
        /// </summary>
        /// <param name="errorCode">
        /// The error code.
        /// </param>
        /// <param name="errorMessage">
        /// The error message.
        /// </param>
        public PlayerErrorEventArgs(string errorCode, string errorMessage)
        {
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets or sets ErrorCode.
        /// </summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets ErrorMessage.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
