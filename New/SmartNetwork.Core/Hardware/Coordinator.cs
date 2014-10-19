using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SmartNetwork.Core.Hardware
{
    public class Coordinator : ObservableObject
    {
        #region Fields
        private ObservableCollection<Module> modules = new ObservableCollection<Module>();
        #endregion

        #region Properties
        public ObservableCollection<Module> Modules
        {
            get { return modules; }
        }
        public ObservableCollection<ControlLine> ControlLines // control lines of all modules
        {
            get { return new ObservableCollection<ControlLine>(modules.SelectMany(module => module.ControlLines).ToList()); }
        }

        public Module this[byte[] moduleAddress]
        {
            get
            {
                var res = modules.Where(module => module.Address == moduleAddress);
                return res.Any() ? res.First() : null;
            }
        }
        public ControlLine this[byte[] moduleAddress, byte lineAddress]
        {
            get
            {
                var res = ControlLines.Where(line => line.Module.Address == moduleAddress && line.Address == lineAddress);
                return res.Any() ? res.First() : null;
            }
        }
        #endregion

        #region Events
        public event NotifyCollectionChangedEventHandler ModulesCollectionChanged;
        #endregion

        #region Constructors
        public Coordinator()
        {
        }
        #endregion

        #region Public methods
        public async void Locate()
        {
            DatagramSocket socket = new DatagramSocket();
            //socket.Control.DontFragment = true;
            socket.MessageReceived += MessageReceived;

            try
            {
                //// Connect to the server (in our case the listener we created in previous step).
                await socket.ConnectAsync(new HostName("255.255.255.255"), "8888");
                //await socket.ConnectAsync(new HostName("192.168.1.177"), "8888");

                //rootPage.NotifyUser("Connected", NotifyType.StatusMessage);

                // Mark the socket as connected. Set the value to null, as we care only about the fact that the property is set.
                //CoreApplication.Properties.Add("connected", null);

                DataWriter udpWriter = new DataWriter(socket.OutputStream);
                udpWriter.WriteString("SNC");
                await udpWriter.StoreAsync();



                //byte[] msg = new byte[] { 35, 36, 37 };
                //IOutputStream stream = await socket.GetOutputStreamAsync(new HostName("255.255.255.255"), "8888");
                //await stream.WriteAsync(BytesToBuffer(msg)); 

            }
            catch (Exception exception)
            {
                // If this is an unknown status it means that the error is fatal and retry will likely fail.
                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                {
                    throw;
                }

                //rootPage.NotifyUser("Connect failed with error: " + exception.Message, NotifyType.ErrorMessage);
            }
        }
        private void MessageReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs eventArguments)
        {
            try
            {
                uint stringLength = eventArguments.GetDataReader().UnconsumedBufferLength;
                string a = eventArguments.GetDataReader().ReadString(stringLength);
                string b = a;


                //NotifyUserFromAsyncThread(
                //    "Receive data from remote peer: \"" +
                //    eventArguments.GetDataReader().ReadString(stringLength) + "\"",
                //    NotifyType.StatusMessage);
            }
            catch (Exception exception)
            {
                SocketErrorStatus socketError = SocketError.GetStatus(exception.HResult);
                if (socketError == SocketErrorStatus.ConnectionResetByPeer)
                {
                    // This error would indicate that a previous send operation resulted in an 
                    // ICMP "Port Unreachable" message.
                    //NotifyUserFromAsyncThread(
                    //    "Peer does not listen on the specific port. Please make sure that you run step 1 first " +
                    //    "or you have a server properly working on a remote server.",
                    //    NotifyType.ErrorMessage);
                }
                else if (socketError != SocketErrorStatus.Unknown)
                {
                    //NotifyUserFromAsyncThread(
                    //    "Error happened when receiving a datagram: " + socketError.ToString(),
                    //    NotifyType.ErrorMessage);
                }
                else
                {
                    throw;
                }
            }
        }
        public void UpdateNetwork()
        {
            var modulesOnline = GetOnlineModules();

            var modulesRemoved = Modules.Except(modulesOnline);
            foreach (var item in modulesRemoved)
                Modules.Remove(item);

            var modulesAdded = modulesOnline.Except(Modules);
            foreach (var module in modulesAdded)
                Modules.Add(module);

            if (ModulesCollectionChanged != null && (modulesAdded.Count() != 0 || modulesRemoved.Count() != 0))
                ModulesCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, modulesAdded, modulesRemoved));
        }
        #endregion

        #region Private methods
        internal bool Write(Module target, byte[] request)
        {
            return false;
        }
        internal bool WriteRead(Module target, byte[] request, byte[] response)
        {
            return false;
        }
        private IList<Module> GetOnlineModules()
        {
            //var newItems =
            //    from str in data
            //    let m = r.Match(str)
            //    let id = int.Parse(m.Groups["ID"].Value)
            //    let address = int.Parse(m.Groups["Address"].Value)
            //    let strName = m.Groups["Name"].Value
            //    let name = !string.IsNullOrEmpty(strName) ? strName.Replace("\"", "") : ""
            //    let protocol = m.Groups["Protocol"].Value
            //    select new Locomotive(id, address, name, protocol);


            return new List<Module>();
        }


        #endregion

        static byte[] BufferToBytes (IBuffer buf)

        {

        using (var dataReader = DataReader.FromBuffer (buf))

        {

        var bytes = new byte[buf.Capacity];

        dataReader.ReadBytes(bytes);

        return bytes;

        }

        }

 

        static IBuffer BytesToBuffer (byte[] bytes)

        {

        using (var dataWriter = new DataWriter ())

        {

        dataWriter.WriteBytes (bytes);

        return dataWriter.DetachBuffer();

        }

        }
    }
}
