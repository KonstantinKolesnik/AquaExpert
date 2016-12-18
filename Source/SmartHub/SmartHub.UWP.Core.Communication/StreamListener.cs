using System;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.UI.Core;

namespace SmartHub.UWP.Core.Communication
{
    public class StreamListener
    {
        private StreamSocketListener listener;

        public event EventHandler<StringEventArgs> DataReceived;

        public async void Start(string serviceName)
        {
            if (listener != null)
            {
                listener = new StreamSocketListener();
                listener.ConnectionReceived += OnConnection;

                // If necessary, tweak the listener's control options before carrying out the bind operation.
                // These options will be automatically applied to the connected StreamSockets resulting from
                // incoming connections (i.e., those passed as arguments to the ConnectionReceived event handler).
                // Refer to the StreamSocketListenerControl class' MSDN documentation for the full list of control options.
                listener.Control.KeepAlive = false;

                await listener.BindServiceNameAsync(serviceName);
            }
        }
        public void Stop()
        {
            if (listener != null)
            {
                listener.Dispose();
                listener = null;
            }
        }

        private async void OnConnection(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            DataReader reader = new DataReader(args.Socket.InputStream);
            try
            {
                while (true)
                {
                    // Read first 4 bytes (length of the subsequent string).
                    uint sizeFieldCount = await reader.LoadAsync(sizeof(uint));
                    if (sizeFieldCount != sizeof(uint))
                    {
                        // The underlying socket was closed before we were able to read the whole data.
                        return;
                    }

                    // Read the string.
                    uint stringLength = reader.ReadUInt32();
                    uint actualStringLength = await reader.LoadAsync(stringLength);
                    if (stringLength != actualStringLength)
                    {
                        // The underlying socket was closed before we were able to read the whole data.
                        return;
                    }

                    // Display the string on the screen. The event is invoked on a non-UI thread, so we need to marshal
                    // the text back to the UI thread.
                    //NotifyUserFromAsyncThread(String.Format("Received data: \"{0}\"", reader.ReadString(actualStringLength)), NotifyType.StatusMessage);

                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        DataReceived?.Invoke(this, new StringEventArgs(reader.ReadString(actualStringLength)));
                    });
                }
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                    throw;
            }
        }
    }
}
