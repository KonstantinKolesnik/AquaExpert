using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.Wemos.Core;
using SmartHub.UWP.Plugins.Wemos.Models;
using SmartHub.UWP.Plugins.Wemos.Transport;
using System;
using System.Threading.Tasks;

namespace SmartHub.UWP.Plugins.Wemos
{
    public class WemosPlugin : PluginBase
    {
        #region Fields
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private WemosTransport transport = new WemosTransport();
        #endregion

        public event WemosMessageEventHandler MessageReceived;

        #region Plugin ovverrides
        public override void InitDbModel()
        {
            using (var db = Context.OpenConnection())
            {
                db.CreateTable<WemosSetting>();
                db.CreateTable<WemosNode>();
                db.CreateTable<WemosLine>();
                db.CreateTable<WemosNodeBatteryValue>();
                db.CreateTable<WemosLineValue>();
            }
        }
        public override void InitPlugin()
        {
            if (GetSetting("UnitSystem") == null)
                Save(new WemosSetting() { Name = "UnitSystem", Value = "M" });

            transport.MessageReceived += OnMessageReceived;
        }
        public override async void StartPlugin()
        {
            await transport.Open();
            await RequestPresentation();
        }
        public override void StopPlugin()
        {
            transport.Close();
        }
        #endregion

        #region API
        public async Task Send(WemosMessage data, bool isBrodcast = false)
        {
            await transport.Send(data, isBrodcast);
        }
        public async Task RequestPresentation(int nodeID = -1, int lineID = -1)
        {
            await Send(new WemosMessage(nodeID, lineID, WemosMessageType.Presentation, 0), true);
        }
        public async Task RequestLineValue(WemosLine line)
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
        public async Task RebootNode(WemosNode node)
        {
            await Send(new WemosMessage(node.NodeID, -1, WemosMessageType.Internal, (int) WemosInternalMessageType.Reboot));
        }


        public static bool IsMessageFromLine(WemosMessage msg, WemosLine line)
        {
            return msg != null && line != null && line.NodeID == msg.NodeID && line.LineID == msg.LineID;
        }

        //public List<WemosNode> GetNodes()
        //{
        //    using (var db = Context.OpenConnection())
        //        //return db.Table<WemosNode>().ToList();
        //        return (from p in db.Table<WemosNode>() select p).ToList();
        //}
        public WemosNode GetNode(int nodeID)
        {
            using (var db = Context.OpenConnection())
            {
                //db.TraceListener = new DebugTraceListener(); // cctivate tracing
                return db.Table<WemosNode>().Where(n => n.NodeID == nodeID).FirstOrDefault();
            }
        }
        public WemosLine GetLine(int nodeID, int lineID)
        {
            using (var db = Context.OpenConnection())
                return db.Table<WemosLine>().Where(l => l.NodeID == nodeID && l.LineID == lineID).FirstOrDefault();
        }
        #endregion

        #region Event handlers
        private async void OnMessageReceived(object sender, WemosMessageEventArgs e)
        {
            if (e.Message != null)
            {
                //System.Diagnostics.Debug.WriteLine(e.Message.ToString());

                MessageReceived?.Invoke(this, e); // TODO: temporary!!!
                await ProcessMessage(e.Message);
            }
        }
        #endregion

        #region Private methods
        private async Task ProcessMessage(WemosMessage message)
        {
            WemosNode node = GetNode(message.NodeID);
            WemosLine line = GetLine(message.NodeID, message.LineID);

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

                        WemosLineValue sv = new WemosLineValue()
                        {
                            NodeID = message.NodeID,
                            LineID = message.LineID,
                            TimeStamp = DateTime.Now,
                            //Type = (SensorValueType) message.SubType,
                            Value = message.GetFloat()
                        };
                        Save(sv);

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
                                WemosNodeBatteryValue bl = new WemosNodeBatteryValue()
                                {
                                    NodeID = message.NodeID,
                                    TimeStamp = DateTime.Now,
                                    Value = message.GetInteger()
                                };

                                Save(bl);

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
                await RebootNode(node);
        }
        #endregion

        #region DB
        private WemosSetting GetSetting(string name)
        {
            using (var db = Context.OpenConnection())
                return db.Table<WemosSetting>().Where(setting => setting.Name == name).FirstOrDefault();
        }
        private void Save(object item)
        {
            using (var db = Context.OpenConnection())
                db.Insert(item);
        }
        private void SaveOrUpdate(object item)
        {
            using (var db = Context.OpenConnection())
                db.InsertOrReplace(item);
        }
        private void Delete(object item)
        {
            using (var db = Context.OpenConnection())
                db.Delete(item);
        }
        #endregion
    }
}
