using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Multiprocess.Issue.DemoApp.UserControls
{
    using System.IO;
    using System.Reflection;

    using Vlc.DotNet.Core;
    using Vlc.DotNet.Wpf;

    using Path = System.Windows.Shapes.Path;

    /// <summary>
    /// Interaction logic for Test.xaml
    /// </summary>
    public partial class Test : Window, IDisposable
    {

        public Test()
        {
            InitializeComponent();
        }

        List<VlcControl> vlcControls = new List<VlcControl>(16);

        private int count = 0;
        private void AddVideoRadButton_Click(object sender, RoutedEventArgs e)
        {
            var vlcControl = this.GetVlcControl();
            vlcControl.Margin = new Thickness(5);
            MainGrid.Children.Add(vlcControl);
            Grid.SetRow(vlcControl, this.count / 4);
            Grid.SetColumn(vlcControl, this.count % 4);
            var videoUrl = string.Format(@"C:\Video\4KVideo0{0}.mp4", count++);
            vlcControl.MediaPlayer.Play(new Uri(videoUrl));
        }

        private VlcControl GetVlcControl()
        {

            var control = new VlcControl();
            control.MediaPlayer.VlcLibDirectoryNeeded += MediaPlayer_VlcLibDirectoryNeeded;
            control.MediaPlayer.Playing += MediaPlayer_Playing;
            control.MediaPlayer.EncounteredError += MediaPlayer_EncounteredError;
            control.MediaPlayer.Opening += MediaPlayer_Opening;
            control.Unloaded += this.ucVLCMediaPlayerControl_Unloaded;
            vlcControls.Add(control);
            return control;
        }

        private void ucVLCMediaPlayerControl_Unloaded(object sender, RoutedEventArgs e)
        {

        }

        private void MediaPlayer_Opening(object sender, Vlc.DotNet.Core.VlcMediaPlayerOpeningEventArgs e)
        {

        }

        private void MediaPlayer_EncounteredError(object sender, Vlc.DotNet.Core.VlcMediaPlayerEncounteredErrorEventArgs e)
        {
            MessageBox.Show(this, e.ToString(), "error");
        }

        private void MediaPlayer_Playing(object sender, Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs e)
        {

        }

        private void MediaPlayer_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;

            if (currentDirectory == null)
                return;

            e.VlcLibDirectory = AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture
                                == ProcessorArchitecture.X86 ? new DirectoryInfo(System.IO.Path.Combine(currentDirectory, @"..\..\lib\WPF\x86")) : new DirectoryInfo(System.IO.Path.Combine(currentDirectory, @"..\..\lib\WPF\x64"));
        }

        public void Dispose()
        {
            foreach (var vlcControl in vlcControls)
            {
                vlcControl.MediaPlayer.Position = 1f;
            }
            foreach (var vlcControl in vlcControls)
            {
                vlcControl.MediaPlayer.Stop();
            }
        }
    }
}
