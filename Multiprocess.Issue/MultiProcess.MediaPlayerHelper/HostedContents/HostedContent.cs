using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Integration;

namespace MultiProcess.MediaPlayerHelper.HostedContents
{
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;

    public class HostedContent : WindowsFormsHost
    {
        /// <summary>
        /// Dependency property for this control's ExternalHandle.
        /// </summary>
        public static readonly DependencyProperty ExternalHandleProperty = DependencyProperty.Register("ExternalHandle", typeof(IntPtr), typeof(HostedContent));

        /// <summary>
        /// Fires if the <see cref="NativeMethods.SetParent"/> or <see cref="NativeMethods.SetWindowPos"/> calls failed.
        /// This indicates the external window may be in an unknown or unstable state.
        /// </summary>
        public event EventHandler NativeCallFailed;

        /// <summary>
        /// Single-thread Apartment task scheduler for our UI-related NativeMethod calls.
        /// </summary>
        private readonly StaTaskScheduler taskScheduler = new StaTaskScheduler(1);

        /// <summary>
        /// The internal container of the WindowsFormsHost that contains the externally hosted content.
        /// </summary>
        private readonly System.Windows.Forms.Panel internalContainer;

        /// <summary>
        /// Gets or sets the external handle for the hosted content.
        /// </summary>
        public IntPtr ExternalHandle
        {
            get
            {
                return (IntPtr)GetValue(ExternalHandleProperty);
            }

            set
            {
                this.SetValue(ExternalHandleProperty, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostedContent"/> class.
        /// </summary>
        public HostedContent()
        {
            this.SizeChanged += this.InternalHostSizeChanged;
            this.internalContainer = new System.Windows.Forms.Panel();
            this.Child = this.internalContainer;
        }

        /// <summary>
        /// Handles redrawing the hosted area when the parent content size has changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">The <see cref="SizeChangedEventArgs"/> containing the size changed data.</param>
        private void InternalHostSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.Invalidate(e.NewSize);
        }

        /// <summary>
        /// Invalidates the drawing area of the external process.
        /// </summary>
        /// <param name="size">The size of the window area to be drawn.</param>
        public void Invalidate(Size size)
        {
            this.ReDraw(size, this.ExternalHandle);
        }

        /// <summary>
        /// Forces the position of the window to move thus causing a redraw.
        /// </summary>
        /// <param name="size">
        /// The size of the window area to be drawn.
        /// </param>
        /// <param name="externalHandle">
        /// The handle for the external window.
        /// </param>
        private void ReDraw(Size size, IntPtr externalHandle)
        {
            if (this.Dispatcher.CheckAccess())
            {
                Task.Factory.StartNew(() => this.ReDraw(size, externalHandle), CancellationToken.None, TaskCreationOptions.AttachedToParent, this.taskScheduler);
            }
            else
            {
                if (externalHandle == IntPtr.Zero)
                {
                    return;
                }

                var success = NativeMethods.SetWindowPos(
                    externalHandle,
                    IntPtr.Zero,
                    0,
                    0,
                    (int)size.Width,
                    (int)size.Height,
                    NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);

                if (!success)
                {
                    this.OnNativeCallFailed();
                }
            }
        }

        /// <summary>
        /// Fires the <see cref="NativeCallFailed"/> event.
        /// </summary>
        private void OnNativeCallFailed()
        {
            if (this.NativeCallFailed != null)
            {
                this.NativeCallFailed.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
