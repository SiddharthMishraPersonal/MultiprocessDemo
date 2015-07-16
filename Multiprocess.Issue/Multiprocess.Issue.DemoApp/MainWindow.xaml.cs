using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Multiprocess.Issue.DemoApp
{
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows.Threading;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                //this.VlcControl.MediaPlayer.VlcLibDirectoryNeeded += MediaPlayer_VlcLibDirectoryNeeded;
            }
            catch (Exception exception)
            {
                MessageBox.Show(this, exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        //void MediaPlayer_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        //{
        //    var currentAssembly = Assembly.GetEntryAssembly();
        //    var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;

        //    if (currentDirectory == null)
        //        return;

        //    e.VlcLibDirectory = AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture
        //                        == ProcessorArchitecture.X86 ? new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\..\lib\x86")) : new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\..\lib\x64"));

        //}

        //private void StartButton_Click(object sender, RoutedEventArgs e)
        //{
        //    this.PlayVideo();
        //}

        //private void PlayVideo()
        //{
        //    var fileInfo = new FileInfo(@"..\..\Video\videoplayback.mp4");
        //    this.VlcControl.MediaPlayer.Play(fileInfo);
        //}

        //private void StopButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this.VlcControl.MediaPlayer.IsPlaying)
        //    {
        //        this.VlcControl.MediaPlayer.Stop();
        //    }
        //}

        //private void PauseButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this.VlcControl.MediaPlayer.IsPlaying)
        //    {
        //        this.VlcControl.MediaPlayer.Pause();
        //    }
        //}
    }
}
