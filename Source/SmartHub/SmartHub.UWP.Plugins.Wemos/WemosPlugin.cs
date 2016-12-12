using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Speech;
using SmartHub.UWP.Plugins.Wemos.Core;
using SmartHub.UWP.Plugins.Wemos.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace SmartHub.UWP.Plugins.Wemos
{
    public class WemosPlugin : PluginBase
    {
        #region Fields
        private const string localService = "11111";
        private const string remoteService = "22222";
        private const string remoteMulticastAddress = "224.3.0.5";
        private const string remoteBroadcastAddress = "255.255.255.255";
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private DatagramSocket listenerSocket = null;
        private const string socketId = "ListenerSocket";
        private IBackgroundTaskRegistration task = null;
        #endregion

        public event WemosMessageEventHandler DataReceived;

        #region Plugin ovverrides
        //public override void InitDbModel(ModelMapper mapper)
        //{
        //    mapper.Class<MySensorsSetting>(cfg => cfg.Table("MySensors_Settings"));
        //    mapper.Class<Node>(cfg => cfg.Table("MySensors_Nodes"));
        //    mapper.Class<Sensor>(cfg => cfg.Table("MySensors_Sensors"));
        //    mapper.Class<BatteryLevel>(cfg => cfg.Table("MySensors_BatteryLevels"));
        //    mapper.Class<SensorValue>(cfg => cfg.Table("MySensors_SensorValues"));
        //}
        public override void InitPlugin()
        {
            if (GetSetting("UnitSystem") == null)
                Save(new WemosSetting() { Name = "UnitSystem", Value = "M" });
        }
        public override async void StartPlugin()
        {
            await OpenSocket();
        }
        public override void StopPlugin()
        {
            CloseSocket();
        }
        #endregion

        #region Public methods
        public async Task Send(WemosMessage data)
        {
            if (data != null)
            {
                var str = data.ToDto();
                if (!string.IsNullOrEmpty(str))
                {
                    try
                    {
                        // GetOutputStreamAsync can be called multiple times on a single DatagramSocket instance to obtain
                        // IOutputStreams pointing to various different remote endpoints. The remote hostname given to
                        // GetOutputStreamAsync can be a unicast, multicast or broadcast address.
                        IOutputStream outputStream = await listenerSocket.GetOutputStreamAsync(new HostName(remoteMulticastAddress), remoteService);

                        DataWriter writer = new DataWriter(outputStream);
                        writer.WriteString(str);
                        await writer.StoreAsync();
                    }
                    catch (Exception exception)
                    {
                        // If this is an unknown status it means that the error is fatal and retry will likely fail.
                        if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                            throw;

                        //rootPage.NotifyUser("Send failed with error: " + exception.Message, NotifyType.ErrorMessage);
                    }
                }
            }
        }
        #endregion

        #region API
        public async void RequestLineValue(WemosLine line)
        {
            await Send(new WemosMessage(line.NodeID, line.LineID, WemosMessageType.Get, (int) line.Type));
        }

        //public async void SetLineValue(WemosLine sensor, float value)
        //{
        //    //if (sensor != null)
        //    //{
        //    //    var lastSV = GetLastSensorValue(sensor);
        //    //    if (lastSV == null || (lastSV.Value != value))
        //    //        await Send(new WemosMessage(sensor.NodeID, sensor.LineID, WemosMessageType.Set, (int) sensor.Type, value));
        //    //}
        //}
        //public async void SetLineValue(WemosLine sensor, string value)
        //{
        //    await Send(new WemosMessage(sensor.NodeID, sensor.LineID, WemosMessageType.Set, (int) sensor.Type, value));
        //}






        public static bool IsMessageFromLine(WemosMessage msg, WemosLine line)
        {
            return msg != null && line != null && line.NodeID == msg.NodeID && line.LineID == msg.LineID;
        }
        #endregion

        #region Event handlers
        private void MessageReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs eventArguments)
        {
            try
            {
                // Interpret the incoming datagram's entire contents as a string.
                uint length = eventArguments.GetDataReader().UnconsumedBufferLength;
                string str = eventArguments.GetDataReader().ReadString(length);

                //NotifyUserFromAsyncThread("Received data from remote peer (Remote Address: " + eventArguments.RemoteAddress.CanonicalName + ", Remote Port: " + eventArguments.RemotePort + "): \"" + str + "\"", NotifyType.StatusMessage);

                foreach (var msg in WemosMessage.FromDto(str))
                    ProcessWemosMessage(msg);
            }
            catch (Exception exception)
            {
                SocketErrorStatus socketError = SocketError.GetStatus(exception.HResult);

                if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                    throw;

                //rootPage.NotifyUser("Error happened when receiving a datagram:" + exception.Message, NotifyType.ErrorMessage);
            }
        }
        #endregion

        #region Private methods
        private async Task OpenSocket()
        {
            if (listenerSocket == null)
            {
                listenerSocket = new DatagramSocket();
                listenerSocket.MessageReceived += MessageReceived;

                listenerSocket.Control.DontFragment = true;

                // DatagramSockets conduct exclusive (SO_EXCLUSIVEADDRUSE) binds by default, effectively blocking
                // any other UDP socket on the system from binding to the same local port. This is done to prevent
                // other applications from eavesdropping or hijacking a DatagramSocket's unicast traffic.
                //
                // Setting the MulticastOnly control option to 'true' enables a DatagramSocket instance to share its
                // local port with any Win32 sockets that are bound using SO_REUSEADDR/SO_REUSE_MULTICASTPORT and
                // with any other DatagramSocket instances that have MulticastOnly set to true. However, note that any
                // attempt to use a multicast-only DatagramSocket instance to send or receive unicast data will result
                // in an exception being thrown.
                //
                // This control option is particularly useful when implementing a well-known multicast-based protocol,
                // such as mDNS and UPnP, since it enables a DatagramSocket instance to coexist with other applications
                // running on the system that also implement that protocol.
                listenerSocket.Control.MulticastOnly = true;

                //listenerSocket.TransferOwnership()

                // Start listen operation.
                try
                {
                    await listenerSocket.BindServiceNameAsync(localService);

                    //if (isMulticastSocket)
                    //{
                    // Join the multicast group to start receiving datagrams being sent to that group.
                    listenerSocket.JoinMulticastGroup(new HostName(remoteMulticastAddress));


                    //rootPage.NotifyUser("Listening on port " + listenerSocket.Information.LocalPort + " and joined to multicast group", NotifyType.StatusMessage);
                    //}
                    //else
                    //    rootPage.NotifyUser("Listening on port " + listenerSocket.Information.LocalPort, NotifyType.StatusMessage);

                    Context.GetPlugin<SpeechPlugin>()?.Say("WEMOS UDP клиент запущен!");
                }
                catch (Exception exception)
                {
                    CloseSocket();

                    // If this is an unknown status it means that the error is fatal and retry will likely fail.
                    if (SocketError.GetStatus(exception.HResult) == SocketErrorStatus.Unknown)
                        throw;

                    //rootPage.NotifyUser("Start listening failed with error: " + exception.Message, NotifyType.ErrorMessage);
                }
            }
        }
        private void CloseSocket()
        {
            if (listenerSocket != null)
            {
                // DatagramSocket.Close() is exposed through the Dispose() method in C#.
                // The call below explicitly closes the socket, freeing the UDP port that it is currently bound to.
                listenerSocket.Dispose();
                listenerSocket = null;

                Context.GetPlugin<SpeechPlugin>()?.Say("WEMOS UDP клиент остановлен");
            }
        }
        //private void NotifyUserFromAsyncThread(string strMessage, NotifyType type)
        //{
        //    var ignore = Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => rootPage.NotifyUser(strMessage, type));
        //}

        private async void ProcessWemosMessage(WemosMessage message)
        {
            if (message != null)
            {
                Debug.WriteLine(message.ToString());
                DataReceived?.Invoke(this, new WemosMessageEventArgs(message)); // TODO: temporary!!!

                WemosNode node = null;// GetNode(message.NodeID);
                WemosLine line = null;// GetLine(message.NodeID, message.LineID); // if message.SensorID == 255 it returns null

                switch (message.Type)
                {
                    #region Presentation
                    case WemosMessageType.Presentation: // sent by a nodes when they present attached sensors.
                        if (message.LineID == -1) // node
                        {
                            if (node == null)
                            {
                                node = new WemosNode
                                {
                                    NodeID = message.NodeID,
                                    Type = (WemosLineType) message.SubType,
                                    ProtocolVersion = message.GetFloat()
                                };
                                Save(node);
                            }
                            else
                            {
                                node.Type = (WemosLineType) message.SubType;
                                node.ProtocolVersion = message.GetFloat();
                                SaveOrUpdate(node);
                            }

                            //NotifyMessageReceivedForPlugins(message);
                            //NotifyMessageReceivedForScripts(message);
                            //NotifyForSignalR(new { MsgId = "NodePresentation", Data = BuildNodeRichWebModel(node) });
                        }
                        else // line
                        {
                            if (node != null)
                            {
                                if (line == null)
                                {
                                    line = new WemosLine()
                                    {
                                        NodeID = node.NodeID,
                                        LineID = message.LineID,
                                        Type = (WemosLineType) message.SubType,
                                        ProtocolVersion = message.GetFloat()
                                    };
                                    Save(line);
                                }
                                else
                                {
                                    line.Type = (WemosLineType) message.SubType;
                                    line.ProtocolVersion = message.GetFloat();
                                    SaveOrUpdate(line);
                                }

                                //NotifyMessageReceivedForPlugins(message);
                                //NotifyMessageReceivedForScripts(message);
                                //NotifyForSignalR(new { MsgId = "SensorPresentation", Data = BuildSensorRichWebModel(line) });
                            }
                        }
                        break;
                    #endregion

                    #region Report
                    case WemosMessageType.Report:
                        if (line != null)
                        {
                            //NotifyMessageCalibrationForPlugins(message); // before saving to DB plugins may adjust the sensor value due to their calibration params
                            var sv = SaveLineValueToDB(message);

                            //NotifyForSignalR(new { MsgId = "MySensorsTileContent", Data = BuildTileContent() }); // update MySensors tile
                            //NotifyMessageReceivedForPlugins(message);
                            //NotifyMessageReceivedForScripts(message);
                            //NotifyForSignalR(new { MsgId = "SensorValue", Data = sv }); // notify Web UI
                        }
                        break;
                    #endregion

                    #region Set
                    case WemosMessageType.Set: // sent to a sensor when a sensor value should be updated
                        break;
                    #endregion

                    #region Request
                    case WemosMessageType.Get: // requests a variable value (usually from an actuator destined for controller)
                        break;
                    #endregion

                    #region Internal
                    case WemosMessageType.Internal:
                        var imt = (WemosInternalMessageType) message.SubType;
                        switch (imt)
                        {
                            case WemosInternalMessageType.BatteryLevel: // int, in %
                                if (node != null)
                                {
                                    var dtDB = DateTime.UtcNow;
                                    var dt = DateTime.Now;

                                    WemosNodeBatteryValue bl = new WemosNodeBatteryValue()
                                    {
                                        NodeID = message.NodeID,
                                        TimeStamp = dtDB,
                                        Value = message.GetInteger()
                                    };

                                    Save(bl);

                                    bl.TimeStamp = dt;

                                    //NotifyMessageReceivedForPlugins(message);
                                    //NotifyMessageReceivedForScripts(message);
                                    //NotifyForSignalR(new { MsgId = "BatteryValue", Data = bl });
                                }
                                break;
                            case WemosInternalMessageType.Time: // seconds since 1970
                                var sec = Convert.ToInt64(DateTime.Now.Subtract(unixEpoch).TotalSeconds);
                                await Send(new WemosMessage(message.NodeID, message.LineID, WemosMessageType.Internal, (int) WemosInternalMessageType.Time).Set(sec));
                                break;
                            case WemosInternalMessageType.Version: // float!
                                break;
                            case WemosInternalMessageType.Config:
                                await Send(new WemosMessage(message.NodeID, -1, WemosMessageType.Internal, (int) WemosInternalMessageType.Config).Set(GetSetting("UnitSystem").Value));
                                break;
                            case WemosInternalMessageType.FirmwareName:
                            case WemosInternalMessageType.FirmwareVersion:
                                if (node != null)
                                {
                                    if (imt == WemosInternalMessageType.FirmwareName)
                                        node.FirmwareName = message.GetString();
                                    else
                                        node.FirmwareVersion = message.GetFloat();

                                    SaveOrUpdate(node);

                                    //NotifyMessageReceivedForPlugins(message);
                                    //NotifyMessageReceivedForScripts(message);
                                    //NotifyForSignalR(new { MsgId = "NodePresentation", Data = BuildNodeRichWebModel(node) });
                                }
                                break;
                        }
                        break;
                    #endregion

                    #region Stream
                    //case WemosMessageType.Stream: //used for OTA firmware updates
                    //    switch ((StreamValueType) message.SubType)
                    //    {
                    //        case StreamValueType.FirmwareConfigRequest:
                    //            var fwtype = pullWord(payload, 0);
                    //            var fwversion = pullWord(payload, 2);
                    //            sendFirmwareConfigResponse(sender, fwtype, fwversion, db, gw);
                    //            break;
                    //        case StreamValueType.FirmwareConfigResponse:
                    //            break;
                    //        case StreamValueType.FirmwareRequest:
                    //            break;
                    //        case StreamValueType.FirmwareResponse:
                    //            var fwtype = pullWord(payload, 0);
                    //            var fwversion = pullWord(payload, 2);
                    //            var fwblock = pullWord(payload, 4);
                    //            sendFirmwareResponse(sender, fwtype, fwversion, fwblock, db, gw);
                    //            break;
                    //        case StreamValueType.Sound:
                    //            break;
                    //        case StreamValueType.Image:
                    //            break;
                    //    }
                    //    break;
                    #endregion
                }

                if (node != null && node.NeedsReboot)
                    await Send(new WemosMessage(node.NodeID, -1, WemosMessageType.Internal, (int) WemosInternalMessageType.Reboot));
            }
        }
        #endregion

        #region DB
        private WemosLineValue SaveLineValueToDB(WemosMessage message)
        {
            //WemosLineValue lastSV = GetLastSensorValue(message.NodeNo, message.SensorNo);
            //if (lastSV != null && lastSV.Type == (SensorValueType)message.SubType && lastSV.Value == message.PayloadFloat)
            //    return lastSV;

            var dtDB = DateTime.UtcNow;
            var dt = DateTime.Now;

            WemosLineValue sv = new WemosLineValue()
            {
                NodeID = message.NodeID,
                LineID = message.LineID,
                TimeStamp = dtDB,
                //Type = (SensorValueType) message.SubType,
                Value = message.GetFloat()
            };

            Save(sv);

            sv.TimeStamp = dt;

            return sv;
        }

        private WemosSetting GetSetting(string name)
        {
            //using (var session = Context.OpenSession())
            //    return session.Query<WemosSetting>().FirstOrDefault(setting => setting.Name == name);
            return null;
        }
        private void Save(object item)
        {
            //using (var session = Context.OpenSession())
            //{
            //    try
            //    {
            //        session.Save(item);
            //        session.Flush();
            //    }
            //    catch (Exception) { }
            //}
        }
        private void SaveOrUpdate(object item)
        {
            //using (var session = Context.OpenSession())
            //{
            //    session.SaveOrUpdate(item);
            //    session.Flush();
            //}
        }
        private void Delete(object item)
        {
            //using (var session = Context.OpenSession())
            //{
            //    session.Delete(item);
            //    session.Flush();
            //}
        }
        #endregion
    }
}
