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
        private ObservableCollection<Message> _messages = new ObservableCollection<Message>();

        /// <summary>
        /// Initializes a new instance of the <see cref="BindableTraceListener"/> class.
        /// </summary>
        public BindableTraceListener()
        {

        }

        /// <summary>
        /// Occurs when the collection changes.
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Gets the messages.
        /// </summary>
        public ObservableCollection<Message> Messages { get { return _messages; } }

        /// <summary>
        /// When overridden in a derived class, writes the specified message to the listener you create in the derived class.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void Write(string message)
        {
            this.WriteMessage(new Message { Value = message });
        }

        /// <summary>
        /// When overridden in a derived class, writes a message to the listener you create in the derived class, followed by a line terminator.
        /// </summary>
        /// <param name="message">A message to write.</param>
        public override void WriteLine(string message)
        {
            this.WriteMessage(new Message { Value = message });
        }

        /// <summary>
        /// Writes the message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void WriteMessage(Message message)
        {
            this.Messages.Insert(0, message);
            this.OnCollectionChanged(message);
        }

        /// <summary>
        /// Called when [collection changed].
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnCollectionChanged(Message message)
        {
            var handler = this.CollectionChanged;
            if (handler != null)
            {
                handler(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, message));
            }
        }
    }
}
