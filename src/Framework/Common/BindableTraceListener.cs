using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using Kinect.Common.Models;

namespace Kinect.Common
{
    /// <summary>
    /// Tracelistener bindable to view
    /// </summary>
    public class BindableTraceListener : TraceListener, INotifyCollectionChanged
    {
        private readonly ObservableCollection<Message> _messages = new ObservableCollection<Message>();

        /// <summary>
        /// Gets the messages.
        /// </summary>
        public ObservableCollection<Message> Messages
        {
            get { return _messages; }
        }

        #region INotifyCollectionChanged Members

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        /// <summary>
        /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void Write(string message)
        {
            WriteMessage(new Message {Value = message});
        }

        /// <summary>
        /// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void WriteLine(string message)
        {
            WriteMessage(new Message {Value = message});
        }

        /// <summary>
        /// Writes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void WriteMessage(Message message)
        {
            Messages.Insert(0, message);
            OnCollectionChanged(message);
        }

        /// <summary>
        /// Called when [collection changed].
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnCollectionChanged(Message message)
        {
            NotifyCollectionChangedEventHandler handler = CollectionChanged;
            if (handler != null)
            {
                handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, message));
            }
        }
    }
}