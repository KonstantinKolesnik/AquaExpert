using SmartHub.UWP.Core.Communication.Stream;
using SmartHub.UWP.Core.Communication.Transporting;
using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SmartHub.UWP.Plugins.ApiListener
{
    [Plugin]
    public class ApiListenerPlugin : PluginBase
    {
        public const string ServiceName = "11111";

        #region Fields
        private StreamSocketListener listener;
        #endregion

        #region Imports
        [ImportMany]
        //public IEnumerable<Lazy<Func<object[], object>, ApiCommandAttribute>> ApiCommands { get; set; }
        public IEnumerable<Func<object[], object>> ApiCommands { get; set; }

        [OnImportsSatisfied]
        public void OnImportsSatisfied()
        {
            int a = 0;
            int b = a;
        }

        #endregion

        #region Plugin ovverrides
        public async override void StartPlugin()
        {
            await Start();
        }
        public override void StopPlugin()
        {
            Stop();
        }
        #endregion

        #region Private methods
        private async Task Start()
        {
            if (listener == null)
            {
                listener = new StreamSocketListener();
                listener.ConnectionReceived += Listener_ConnectionReceived;

                // If necessary, tweak the listener's control options before carrying out the bind operation.
                // These options will be automatically applied to the connected StreamSockets resulting from
                // incoming connections (i.e., those passed as arguments to the ConnectionReceived event handler).
                // Refer to the StreamSocketListenerControl class' MSDN documentation for the full list of control options.
                listener.Control.KeepAlive = false;

                await listener.BindServiceNameAsync(ServiceName);
            }
        }
        private void Stop()
        {
            if (listener != null)
            {
                listener.Dispose();
                listener = null;
            }
        }
        #endregion

        #region Event handlers
        private async void Listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            var reader = new DataReader(args.Socket.InputStream);

            try
            {
                while (true)
                {
                    // Read first 4 bytes (length of the subsequent string).
                    uint sizeFieldCount = await reader.LoadAsync(sizeof(uint)); // waits for connection
                    if (sizeFieldCount != sizeof(uint))
                    {
                        // The underlying socket was closed before we were able to read the whole data.
                        return;
                    }

                    uint dataLength = reader.ReadUInt32();
                    uint actualDataLength = await reader.LoadAsync(dataLength);
                    if (dataLength != actualDataLength)
                    {
                        // The underlying socket was closed before we were able to read the whole data.
                        return;
                    }

                    string str = reader.ReadString(actualDataLength);
                    var data = Transport.Deserialize<StreamData>(str);
                    var result = ExecuteApiCommand(data);

                    // The event is invoked on a non-UI thread, so we need to marshal the text back to the UI thread.
                    //await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    //{
                        
                    //});






                    //using (DataWriter writer = new DataWriter(args.Socket.OutputStream))
                    //{
                    //    string stringToSend = "Hello from listener!";
                    //    //writer.WriteUInt32(writer.MeasureString(stringToSend));
                    //    writer.WriteString(stringToSend);

                    //    await writer.StoreAsync();
                    //    writer.DetachStream();
                    //}
                }
            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    Stop();
                    await Start();
                }
            }
        }

        #endregion

        private object ExecuteApiCommand(StreamData data)
        {
            //var cmd = ApiCommands.FirstOrDefault(c => c.Metadata.CommandName == data.CommandName);
            var cmd = ApiCommands.FirstOrDefault(c => {
                //c.Metadata.CommandName == data.CommandName

                var attr = c.GetType().GetTypeInfo().GetCustomAttribute<ApiCommandAttribute>();

                return attr != null && attr.CommandName == data.CommandName;
            });

            if (cmd != null)
            {
                return cmd(data.Parameters);
            }





            return null;
        }
    }
}
