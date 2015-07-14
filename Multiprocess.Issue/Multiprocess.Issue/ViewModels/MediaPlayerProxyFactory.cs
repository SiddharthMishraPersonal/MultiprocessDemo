using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Multiprocess.Issue.ViewModels
{
    using System.Diagnostics;
    using System.IO;

    using Motorola.APS.MMC.Common.Logging.Prism;
    using Motorola.APS.MMC.Common.Utilities;
    using Motorola.IVS.Client;

    internal class MediaPlayerProxyFactory : IMediaPlayerProxyFactory
    {
        /// <summary>
        /// The file name of the process host .exe.
        /// </summary>
        private const string ProcessPath = @".\Modules\RTVIModule\Viewer.ProcessHost.exe";

        /// <summary>
        /// The folder path of the RTVI generic plugins.
        /// </summary>
        private const string GenericPluginSupportingFilesPath = @"c:\Program Files (x86)\Motorola Solutions\VideoStreaming\Generic Plugins\SupportingFiles\";

        /// <summary>
        /// The file extension for the plugin metadata files.
        /// </summary>
        private const string PluginMetadataExtension = "metadata";

        /// <summary>
        /// The collection of hosted Processes that are currently running, keyed by plugin-type for single-instance plugins and GUID for multi-instance.
        /// </summary>
        private readonly Dictionary<string, Process> hostedProcessCollection = new Dictionary<string, Process>();

        /// <summary>
        /// Lock object for process creation.
        /// </summary>
        private readonly object processLock = new object();

        /// <summary>
        /// Lock object for accessing the <see cref="hostedProcessCollection"/> dictionary.
        /// </summary>
        private readonly object processCollectionLock = new object();

        /// <summary>
        /// The folder path for the legacy plugins (e.g. Genetec 4.7).
        /// </summary>
      
        private readonly string oldPluginPath = Path.Combine(Environment.CurrentDirectory, @"Modules\RTVIModule\GenericMediaPlayer");

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaPlayerProxyFactory"/> class.
        /// </summary>
        /// <param name="eventLogger">
        /// Event logger for writing to application event log. 
        /// </param>
        /// <param name="moduleConfiguration">
        /// Provides methods for accessing module config file.
        /// </param>
        /// <param name="eventAggregator">
        /// Event aggregator to provide access to and a means to publish PRISM events.
        /// </param>
      
        public MediaPlayerProxyFactory()
        {
           
        }

        /// <summary>
        /// Returns an externally-hosted instance of MediaPlayer.
        /// </summary>
        /// <param name="channelUri">The URI for the desired stream.</param>
        /// <param name="externalProcessId">The external process to re-use (if any).</param>
        /// <returns>The <see cref="IMediaPlayer"/> containing the desired stream.</returns>
        public IMediaPlayer GetPlayerInstance(string channelUri, int externalProcessId = 0)
        {
            if (string.IsNullOrWhiteSpace(channelUri))
            {
                return null;
            }

            var playerProcess = this.GetPlayerProcess(channelUri, externalProcessId);

            return new MediaPlayerProxy();
        }

        /// <summary>
        /// Gets an instance of an external process that hosts the channel stream that is being requested.
        /// </summary>
        /// <param name="channelUri">The channel uri for the stream being requested.</param>
        /// /// <param name="externalProcessId">The external process to re-use (use 0 if none).</param>
        /// <returns>The external process that is hosting the window that can play the stream.</returns>
        private Process GetPlayerProcess(string channelUri, int externalProcessId)
        {
            Process hostedProcess = null;

            // Get the plugin type.
            var pluginType = this.GetPluginType(channelUri);
            var clientPluginConfiguration = this.GetPluginConfiguration(pluginType);

            lock (this.processCollectionLock)
            {
                if (clientPluginConfiguration.IsSingleInstance && this.hostedProcessCollection.ContainsKey(pluginType) && externalProcessId == 0)
                {
                    // Re-use the plugin-specific single-instance process.
                    hostedProcess = this.hostedProcessCollection[pluginType];
                }
                else if (externalProcessId != 0)
                {
                    // Re-use the process with the specified ID.
                    hostedProcess = this.hostedProcessCollection.SingleOrDefault(p => p.Value.Id == externalProcessId).Value;
                }
            }

            if (hostedProcess == null)
            {
                var processKey = clientPluginConfiguration.IsSingleInstance ? pluginType : Guid.NewGuid().ToString();

                // Start a new instance of process host.
                if (clientPluginConfiguration.IsSingleInstance)
                {
                    // Need to lock the actual process-creation step if this is single-instance in case
                    // we're opening several streams at once, in order to prevent multiple processes from being started.
                    lock (this.processLock)
                    {
                        // While waiting for the lock, another thread may have created the single-instance process already, do another check and
                        // return the process if so.
                        lock (this.processCollectionLock)
                        {
                            if (this.hostedProcessCollection.ContainsKey(pluginType))
                            {
                                return this.hostedProcessCollection[pluginType];
                            }
                        }

                        hostedProcess = this.CreateProcess(processKey);
                    }
                }
                else
                {
                    // Don't want to lock if we're not single-instance as it unnecessarily slows things down.
                    hostedProcess = this.CreateProcess(processKey);
                }
            }

            return hostedProcess;
        }

        /// <summary>
        /// Creates a new process and adds it to the <see cref="hostedProcessCollection"/> with the specified key.
        /// </summary>
        /// <param name="processKey">
        /// The key to use for this process's entry in the <see cref="hostedProcessCollection"/>.
        /// </param>
        /// <returns>
        /// The created <see cref="Process"/>.
        /// </returns>
        private Process CreateProcess(string processKey)
        {
            var hostedProcess = this.StartExternalProcess();
            hostedProcess.Exited += this.OnProcessExited;

            lock (this.processCollectionLock)
            {
                this.hostedProcessCollection.Add(processKey, hostedProcess);
            }

            return hostedProcess;
        }

        /// <summary>
        /// Removes process from the collection if the process exits.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnProcessExited(object sender, EventArgs e)
        {
            lock (this.processCollectionLock)
            {
                if (this.hostedProcessCollection.ContainsValue((Process)sender))
                {
                    ((Process)sender).Exited -= this.OnProcessExited;

                    var hostedProcess = this.hostedProcessCollection.FirstOrDefault(x => x.Value == (Process)sender);
                    this.hostedProcessCollection.Remove(hostedProcess.Key);
                }
            }
        }

        /// <summary>
        /// Gets the plugin type for a specific channel uri.
        /// </summary>
        /// <param name="channelUri">The channel uri to obtain the plugin type from.</param>
        /// <returns>The plugin type.</returns>
        private string GetPluginType(string channelUri)
        {
            var pluginType = channelUri.Substring(0, channelUri.IndexOf("://", StringComparison.InvariantCulture));

            return pluginType;
        }

        /// <summary>
        /// Gets the plugin configuration settings for a specific plugin type.
        /// </summary>
        /// <param name="pluginType">The plugin type to obtain configuration for.</param>
        /// <returns>The configuration for the plugin type.</returns>
        private ClientPluginConfiguration GetPluginConfiguration(string pluginType)
        {
            //var isSingleInstance = false;

            //// This is not currently a value in the .config file, but this will allow us to inject a dummy metadata during unit tests.
            //var genericPath = this.moduleConfiguration.GetConfigParameter("GenericPluginSupportingFilesPath", GenericPluginSupportingFilesPath);

            //var pluginPath = string.Equals(pluginType, "Genetec") ? this.oldPluginPath : genericPath;
            //var filePath = Path.Combine(pluginPath, string.Format("{0}.{1}", pluginType, PluginMetadataExtension));

            //if (File.Exists(filePath))
            //{
            //    var configMap = new ExeConfigurationFileMap { ExeConfigFilename = filePath };
            //    var config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

            //    bool.TryParse(
            //            (config.AppSettings.Settings["IsSingleInstance"] ?? new KeyValueConfigurationElement("IsSingleInstance", "false")).Value,
            //            out isSingleInstance);
            //}

            return new ClientPluginConfiguration { IsSingleInstance = false };
        }

        /// <summary>
        /// Starts an external process in a fashion that it can be injected into the hosting process.
        /// </summary>
        /// <returns>The external process that was started.</returns>
        private Process StartExternalProcess()
        {
            // This is not in the config by default currently, but this will allow us to change the path later if needed and
            // inject a dummy .exe during unit tests.
            var executablePath = this.moduleConfiguration.GetConfigParameter("ExternalProcessPath", ProcessPath);

            executablePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, executablePath);

            if (!File.Exists(executablePath))
            {
                throw new InvalidOperationException("Executable path was not found: " + executablePath + ".");
            }

            var processStartInfo = new ProcessStartInfo(executablePath)
            {
                WindowStyle = ProcessWindowStyle.Minimized,
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            var process = Process.Start(processStartInfo);
            if (process != null)
            {
                process.EnableRaisingEvents = true;
            }

            return process;
        }
    }
}
