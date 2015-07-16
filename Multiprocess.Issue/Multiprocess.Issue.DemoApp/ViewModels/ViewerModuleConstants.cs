using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multiprocess.Issue.ViewModels
{
    using System.Diagnostics.CodeAnalysis;

    public class ViewerModuleConstants
    {
        /// <summary>
        /// The name of the send link attachment as displayed to the client.
        /// </summary>
        public const string SendLinkAttachmentFilename = "SendLink.html";

        /// <summary>
        /// String for sending HTML video links.
        /// </summary>
        public const string SendLinkHtmlFormat = "<html><head><meta http-equiv=\"Refresh\" content=\"1; url={0}\" /></head></html>";

        /// <summary>
        /// Format string to contact the arbitration service.
        /// </summary>
        public const string PtzArbitrationServiceUrl = "http://{0}:9077/PtzArbitrationServiceEx";

        /// <summary>
        /// String that indicates the end of the player type in a media URL.
        /// </summary>
        public const string MediaUrlPlayerTypeSeparator = ":";

        /// <summary>
        /// String the RTVI directory uses to refer to OnSSI channels.
        /// </summary>
        public const string OnSsiDirectoryString = "NetDVMS";

        /// <summary>
        /// String the Generic player infrastructure is looking for for OnSSI.
        /// </summary>
        public const string OnSsiPlayerString = "onssi";

        /// <summary>
        /// The player type string of ocularis
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        public const string OcularisPlayerString = "Ocularis";
    }
}
