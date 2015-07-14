using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiprocess.Issue.ViewModels
{
    /// <summary>
    /// Represents any plugin-specific configuration items.
    /// </summary>
    internal class ClientPluginConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether this plugin requires single-instance hosting. 
        /// </summary>
        internal bool IsSingleInstance { get; set; }
    }
}
