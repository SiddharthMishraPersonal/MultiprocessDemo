using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Multiprocess.Issue.DemoApp
{
    using Multiprocess.Issue.DemoAppViewModels;

    using Telerik.Windows.Controls;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// The app view model.
        /// </summary>
        private ViewModelBase appViewModel;

        /// <summary>
        /// The main view.
        /// </summary>
        private Window mainView;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            this.Startup += this.AppStartup;
            this.Exit += this.AppExit;
        }

        /// <summary>
        /// The app exit.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AppExit(object sender, ExitEventArgs e)
        {
            this.appViewModel.Dispose();
        }

        /// <summary>
        /// The app startup.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void AppStartup(object sender, StartupEventArgs e)
        {
            try
            {
                this.mainView = new MainWindow();
                this.appViewModel = new ApplicationViewModel(this.mainView);
                this.mainView.Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(this.mainView, exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
