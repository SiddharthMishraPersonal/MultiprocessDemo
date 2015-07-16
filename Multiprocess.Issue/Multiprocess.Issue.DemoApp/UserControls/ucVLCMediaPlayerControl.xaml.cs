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

namespace Multiprocess.Issue.DemoApp.UserControls
{
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Interaction logic for ucVLCMediaPlayerControl.xaml
    /// </summary>
    public partial class ucVLCMediaPlayerControl : UserControl
    {
        public ucVLCMediaPlayerControl()
        {
            InitializeComponent();

            this.VlcControl.MediaPlayer.VlcLibDirectoryNeeded += MediaPlayer_VlcLibDirectoryNeeded;
            this.Loaded += ucVLCMediaPlayerControl_Loaded;
            //this.Unloaded += ucVLCMediaPlayerControl_Unloaded;
        }

        void MediaPlayer_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;

            if (currentDirectory == null)
                return;

            e.VlcLibDirectory = AssemblyName.GetAssemblyName(currentAssembly.Location).ProcessorArchitecture
                                == ProcessorArchitecture.X86 ? new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\..\lib\x86")) : new DirectoryInfo(Path.Combine(currentDirectory, @"..\..\..\lib\x64"));

        }

        void ucVLCMediaPlayerControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (this.VlcControl.MediaPlayer.IsPlaying)
            {
                this.VlcControl.MediaPlayer.Stop();
            }
        }

        void ucVLCMediaPlayerControl_Loaded(object sender, RoutedEventArgs e)
        {
            //var fileInfo = new FileInfo(@"..\..\Video\videoplayback.mp4");
            //this.VlcControl.MediaPlayer.Play(fileInfo);
        }
    }
}
