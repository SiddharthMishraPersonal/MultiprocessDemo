using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiProcess.MediaPlayerHelper
{
    using System.Diagnostics;
    using System.IO;

    using MultiProcess.Client;
    using MultiProcess.MediaPlayerHelper.HostedContents;

    using Telerik.Windows.Controls;

    public class MediaPlayerProxyFactory : IMediaPlayerProxyFactory, IDisposable
    {

        /// <summary>
        /// The collection of hosted Processes that are currently running, keyed by plugin-type for single-instance plugins and GUID for multi-instance.
        /// </summary>
        private readonly Dictionary<string, Process> hostedProcessCollection = new Dictionary<string, Process>();

        /// <summary>
        /// Lock object for accessing the <see cref="hostedProcessCollection"/> dictionary.
        /// </summary>
        private readonly object processCollectionLock = new object();

        /// <summary>
        /// Returns an externally-hosted instance of MediaPlayer.
        /// </summary>
        /// <param name="channelUri">The URI for the desired stream.</param>
        /// <param name="externalProcessId">The external process to re-use (if any).</param>
        /// <returns>The <see cref="IMediaPlayer"/> containing the desired stream.</returns>
        public IMediaPlayer GetPlayerInstance(Uri channelUri, int externalProcessId = 0)
        {
            if (null == channelUri)
            {
                return null;
            }

            var playerProcess = this.GetPlayerProcess(channelUri, externalProcessId);

            return new MediaPlayerProxy(playerProcess, channelUri);
        }

        /// <summary>
        /// Gets an instance of an external process that hosts the channel stream that is being requested.
        /// </summary>
        /// <param name="channelUri">The channel uri for the stream being requested.</param>
        /// /// <param name="externalProcessId">The external process to re-use (use 0 if none).</param>
        /// <returns>The external process that is hosting the window that can play the stream.</returns>
        private Process GetPlayerProcess(Uri channelUri, int externalProcessId)
        {
            Process hostedProcess = null;

            if (externalProcessId != 0)
            {
                hostedProcess =
                    this.hostedProcessCollection.SingleOrDefault(p => p.Value.Id == externalProcessId).Value;
            }

            if (hostedProcess == null)
            {
                var processKey = Guid.NewGuid().ToString();
                hostedProcess = this.CreateProcess(processKey);
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

        private Process StartExternalProcess()
        {
            Process process;
            try
            {

                // This is not in the config by default currently, but this will allow us to change the path later if needed and
                // inject a dummy .exe during unit tests.
                var executablePath = @"Viewer.ProcessHost.exe";

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

                process = Process.Start(processStartInfo);
                if (process != null)
                {
                    process.EnableRaisingEvents = true;
                }
            }
            catch (Exception exception)
            {

                throw;
            }
            return process;
        }      

        public void Dispose()
        {
            foreach (KeyValuePair<string, Process> keyValuePair in hostedProcessCollection)
            {
                if (keyValuePair.Value != null)
                {
                    keyValuePair.Value.Kill();
                }
            }
        }
    }
}
