using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms.Integration;

namespace MultiProcess.MediaPlayerHelper.HostedContents
{
    using System.Diagnostics;
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
        public void Invalidate()
        {
            if (this.internalContainer == null)
            {
                return;
            }

            this.Invalidate(this.internalContainer.ClientSize.Width, this.internalContainer.ClientSize.Height, this.ExternalHandle);
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
        /// Invalidates the drawing area of the external process.
        /// </summary>
        /// <param name="width">
        /// The width of the window.
        /// </param>
        /// <param name="height">
        /// The height of the window.
        /// </param>
        /// <param name="externalHandle">
        /// The handle for the external window.
        /// </param>
        public void Invalidate(int width, int height, IntPtr externalHandle)
        {
            if (this.internalContainer == null)
            {
                return;
            }

            this.ReDraw(new Size(width, height), externalHandle);
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.SizeChanged -= this.InternalHostSizeChanged;
                this.taskScheduler.Dispose();
                this.internalContainer.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Handles the Initialized event of the WindowsFormsHost. Performs set-up and window styling.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.AdoptWindow(this.ExternalHandle, this.internalContainer.Handle, this.internalContainer.ClientSize.Width, this.internalContainer.ClientSize.Height);
        }

        /// <summary>
        /// Sets the external window to be a child of this application window, and strips it of all window styles.
        /// </summary>
        /// <param name="externalHandle">Handle for the external window.</param>
        /// <param name="containerHandle">Handle for the internal control that will host the external window.</param>
        /// <param name="width">Width of the window.</param>
        /// <param name="height">Height of the window.</param>
        private void AdoptWindow(IntPtr externalHandle, IntPtr containerHandle, int width, int height)
        {
            if (this.Dispatcher.CheckAccess())
            {
                Task.Factory.StartNew(() => this.AdoptWindow(externalHandle, containerHandle, width, height), CancellationToken.None, TaskCreationOptions.AttachedToParent, this.taskScheduler);
            }
            else
            {
                if (this.SetHostedProcessParentHandle(externalHandle, containerHandle))
                {
                    this.CleanExternalProcessWindowStyle(externalHandle);
                    this.Invalidate(width, height, externalHandle);
                }
                else
                {
                    this.OnNativeCallFailed();
                }
            }
        }

        /// <summary>
        /// Removes window styling from the main window of the process that is getting hosted.
        /// </summary>
        /// <param name="externalProcessWindowHandle">The window handle to remove the styling from.</param>
        private void CleanExternalProcessWindowStyle(IntPtr externalProcessWindowHandle)
        {
            if (this.Dispatcher.CheckAccess())
            {
                Task.Factory.StartNew(() => this.CleanExternalProcessWindowStyle(externalProcessWindowHandle), CancellationToken.None, TaskCreationOptions.AttachedToParent, this.taskScheduler);
            }
            else
            {
                // Gets the style of the window.
                // See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms632600(v=vs.85).aspx
                var style = NativeMethods.GetWindowLong(externalProcessWindowHandle, NativeMethods.GWL_STYLE);

                // Remove the title bar and border from the window.
                style += ~NativeMethods.WS_CAPTION;

                // Remove the sizing border/ability from the window.
                style += ~NativeMethods.WS_THICKFRAME;

                // Remove the child flag from the window.  We will put this back when calling SetWindowLong.
                style += ~NativeMethods.WS_CHILD;

                // Set the window style and define the window as a Popup Child (needed to prevent taskbar appearance)
                var setWindowStatus = NativeMethods.SetWindowLong(externalProcessWindowHandle, NativeMethods.GWL_STYLE, style | NativeMethods.WS_POPUP | NativeMethods.WS_CHILD);

                if (setWindowStatus == 0)
                {
                    var errorCode = NativeMethods.GetLastError();
                    Trace.WriteLine(errorCode);
                }
            }
        }


        /// <summary>
        /// Takes an external process window handle and puts the content of that handle into the handle of an internal control.
        /// </summary>
        /// <param name="externalHandle">
        /// The external Handle.
        /// </param>
        /// <param name="containerHandle">
        /// The container Handle.
        /// </param>
        /// <returns>
        /// Whether the operation succeeded.
        /// </returns>
        private bool SetHostedProcessParentHandle(IntPtr externalHandle, IntPtr containerHandle)
        {
            if (this.Dispatcher.CheckAccess())
            {
                return Task.Factory.StartNew(
                    () => this.SetHostedProcessParentHandle(externalHandle, containerHandle), CancellationToken.None, TaskCreationOptions.AttachedToParent, this.taskScheduler).Result;
            }

            IntPtr parentWindow;
            var retryCount = 0;
            const int MaxRetries = 2000; // Genetec 4.7 can block this for long time . . .

            var externalProcessThreadId = NativeMethods.GetWindowThreadProcessId(externalHandle, IntPtr.Zero);
            var localProcessThreadId = NativeMethods.GetWindowThreadProcessId(containerHandle, IntPtr.Zero);

            // Detach the message queue for the external window from the RIC UI thread.
            NativeMethods.AttachThreadInput(externalProcessThreadId, localProcessThreadId, false);

            do
            {
                parentWindow = NativeMethods.SetParent(externalHandle, containerHandle);

                Thread.Sleep(10);
                retryCount++;
            }
            while (parentWindow == IntPtr.Zero && retryCount <= MaxRetries);

            return retryCount <= MaxRetries;
        }

    }
}
