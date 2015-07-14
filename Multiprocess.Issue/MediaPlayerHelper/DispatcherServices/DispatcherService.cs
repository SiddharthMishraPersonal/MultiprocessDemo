using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerHelper.DispatcherServices
{
    using System.Windows;
    using System.Windows.Threading;

    public class DispatcherService : IDispatcherService
    {
        /// <summary>
        /// Executes the specified Action synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="action">A delegate to a method that takes no arguments, which is pushed onto the Dispatcher event queue.</param>
        public void Invoke(Action action)
        {
            if ((Application.Current == null) || (action == null))
            {
                return;
            }

            var dispatcher = Application.Current.Dispatcher;

            if (dispatcher == null)
            {
                return;
            }

            if (this.CheckAccess() == false)
            {
                dispatcher.Invoke(action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Executes the specified delegate synchronously at the specified priority on the thread on which the Dispatcher is associated with.
        /// </summary>
        /// <param name="priority"> The priority, relative to the other pending operations in the Dispatcher event queue, the specified method is invoked.</param>
        /// <param name="action">A delegate to a method that takes no arguments, which is pushed onto the Dispatcher event queue.</param>
        public void Invoke(DispatcherPriority priority, Action action)
        {
            if ((Application.Current == null) || (action == null))
            {
                return;
            }

            var dispatcher = Application.Current.Dispatcher;

            if (dispatcher == null)
            {
                return;
            }

            if (this.CheckAccess() == false)
            {
                dispatcher.Invoke(priority, action);
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Executes the specified delegate asynchronously at the specified priority on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="priority"> The priority, relative to the other pending operations in the Dispatcher event queue, the specified method is invoked.</param>
        /// <param name="action"> The delegate to a method that takes no arguments, which is pushed onto the Dispatcher event queue.</param>
        public void BeginInvoke(DispatcherPriority priority, Action action)
        {
            if ((Application.Current == null) || (action == null))
            {
                return;
            }

            var dispatcher = Application.Current.Dispatcher;

            if (dispatcher != null)
            {
                dispatcher.BeginInvoke(priority, action);
            }
        }

        /// <summary>
        /// Checks if the current thread has access to the UI thread.
        /// </summary>
        /// <returns>A value indicating whether or not access is allow.</returns>
        public bool CheckAccess()
        {
            var dispatcher = Application.Current.Dispatcher;

            if (dispatcher == null)
            {
                return false;
            }

            return dispatcher.CheckAccess();
        }
    }
}
