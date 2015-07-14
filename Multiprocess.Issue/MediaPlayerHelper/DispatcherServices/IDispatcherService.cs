using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerHelper.DispatcherServices
{
    using System.Windows.Threading;

    /// <summary>
    /// Provides abstraction of the UI dispatcher facility
    /// </summary>
    public interface IDispatcherService
    {
        /// <summary>
        /// Executes the specified Action synchronously on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="action">A delegate to a method that takes no arguments, which is pushed onto the Dispatcher event queue.</param>
        void Invoke(Action action);

        /// <summary>
        /// Executes the specified delegate synchronously at the specified priority on the thread on which the Dispatcher is associated with.
        /// </summary>
        /// <param name="priority"> The priority, relative to the other pending operations in the Dispatcher event queue, the specified method is invoked.</param>
        /// <param name="method">A delegate to a method that takes no arguments, which is pushed onto the Dispatcher event queue.</param>
        void Invoke(DispatcherPriority priority, Action method);

        /// <summary>
        /// Executes the specified delegate asynchronously at the specified priority on the thread the Dispatcher is associated with.
        /// </summary>
        /// <param name="priority"> The priority, relative to the other pending operations in the Dispatcher event queue, the specified method is invoked.</param>
        /// <param name="method"> The delegate to a method that takes no arguments, which is pushed onto the Dispatcher event queue.</param>
        void BeginInvoke(DispatcherPriority priority, Action method);

        /// <summary>
        /// Checks if the current thread has access to the UI thread.
        /// </summary>
        /// <returns>A value indicating whether or not access is allow.</returns>
        bool CheckAccess();
    }
}
