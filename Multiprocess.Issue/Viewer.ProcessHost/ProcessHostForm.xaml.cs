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
using System.Windows.Shapes;

namespace Viewer.ProcessHost
{
    using System.Diagnostics;
    using System.Windows.Interop;

    using Motorola.IVS.Client.Viewer.ProcessHost;

    /// <summary>
    /// Interaction logic for ProcessHostForm.xaml
    /// </summary>
    public partial class ProcessHostForm : Window
    {
        public ProcessHostForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Shows the window after applying styling.
        /// </summary>
        public new void Show()
        {
            Trace.WriteLine("Show is called.");
            var windowHelper = new WindowInteropHelper(this);

            if (windowHelper.Handle == IntPtr.Zero)
            {
                this.ShowInTaskbar = false;
                this.WindowState = WindowState.Minimized;
                base.Show();
            }

            NativeMethods.ShowWindow(windowHelper.Handle, 1);
        }
    }
}
