using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Viewer.ProcessHost
{
    using System.Diagnostics;
    using System.Runtime.ExceptionServices;
    using System.ServiceModel;

    using Motorola.IVS.Client.Viewer.ProcessHost;
    using Motorola.IVS.Client.Viewer.ProcessHost.Communication;
    using Motorola.IVS.Client.Viewer.ProcessHost.Extensions;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Name of the event log source to write to.
        /// </summary>
        private static readonly string EventLogSource = "Test Application";

        /// <summary>
        /// The entry point to the application. Starts a new instance of the application if one isn't already started.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            var parentProcess = Process.GetCurrentProcess().GetParent();
            if (parentProcess != null)
            {
                parentProcess.EnableRaisingEvents = true;
                parentProcess.Exited += ParentProcessExited;
            }

            var localServer = new ProcessHostServer();
            var serviceHost = CreateServiceHost(localServer);

            if (serviceHost != null)
            {
                serviceHost.Open();
                var app = new App();
                app.InitializeComponent();
                app.DispatcherUnhandledException += AppDispatcherUnhandledException;
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;
                app.Run();
                serviceHost.Close();
            }
            else
            {
                Log("Error creating ServiceHost.", EventLogEntryType.Warning);
            }
        }

        /// <summary>
        /// Allows us to log/handle unhandled exceptions.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">Exception event arguments.</param>
        [HandleProcessCorruptedStateExceptions]
        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log(e.ExceptionObject.ToString(), EventLogEntryType.Error);
            Current.Dispatcher.Invoke(Current.Shutdown);
        }

        /// <summary>
        /// Allows us to log/handle unhandled exceptions from the UI thread.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">Exception event arguments.</param>
        [HandleProcessCorruptedStateExceptions]
        private static void AppDispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            Log(e.ToString(), EventLogEntryType.Error);
            Current.Dispatcher.Invoke(Current.Shutdown);
        }

        /// <summary>
        /// Fires when the parent process (if any) has exited.
        /// </summary>
        /// <param name="sender">The sending object.</param>
        /// <param name="e">The event arguments.</param>
        private static void ParentProcessExited(object sender, System.EventArgs e)
        {
            Current.Dispatcher.Invoke(Current.Shutdown);
        }

        /// <summary>
        /// Creates a ServiceHost and service endpoint.
        /// </summary>
        /// <param name="processHostServer">The local process host server implementation.</param>
        /// <returns>The <see cref="ServiceHost"/> hosting the <see cref="ProcessHostServer"/>.</returns>
        private static ServiceHost CreateServiceHost(IProcessHostServiceContract processHostServer)
        {
            var uri = @"net.pipe://localhost/viewer.processhost/{0}";

            if (!string.IsNullOrEmpty(uri))
            {
                var serviceHost = new ServiceHost(processHostServer, new Uri(string.Format(uri, Process.GetCurrentProcess().Id)));

                serviceHost.AddServiceEndpoint(typeof(IProcessHostServiceContract), new NetNamedPipeBinding(), string.Empty);

                return serviceHost;
            }

            return null;
        }

        /// <summary>
        /// Writes an entry to the event log.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="type">What severity to make the message.</param>
        private static void Log(string message, EventLogEntryType type)
        {
            if (EventLog.SourceExists(EventLogSource))
            {
                EventLog.WriteEntry(EventLogSource, message, type);
            }
        }
    }
}
