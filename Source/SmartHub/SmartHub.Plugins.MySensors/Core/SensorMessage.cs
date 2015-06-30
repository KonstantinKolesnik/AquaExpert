using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SmartHub.Plugins.MySensors.Core
{
    public class SensorMessage
    {
        #region Properties
        public byte NodeNo { get; set; }
        public byte SensorNo { get; set; }
        public SensorMessageType Type { get; set; }
        public bool IsAckNeeded { get; set; }
        public byte SubType { get; set; }
        public string Payload { get; private set; }
        
        public float PayloadFloat
        {
            get
            {
                if (!string.IsNullOrEmpty(Payload))
                {
                    string ds = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                    string p = Payload.Replace(",", ds).Replace(".", ds);

                    float v;
                    return float.TryParse(p, out v) ? v : float.NaN;
                }

                return float.NaN;
            }
            set
            {
                if (PayloadFloat != value)
                    Payload = value.ToString();
            }
        }
        public List<int> PayloadFirmware
        {
            get
            {
                var result = new List<int>();

                if (Type == SensorMessageType.Stream)
                    for (var i = 0; i < Payload.Length; i += 2)
                        result.Add(int.Parse(Payload.Substring(i, 2), NumberStyles.HexNumber));
                
                return result;
            }
            set
            {
                if (Type == SensorMessageType.Stream)
                {
                    string result = "";

                    for (var i = 0; i < value.Count; i++)
                    {
                        if (value[i] < 16) // ??????
                            result += "0";
                        result += value[i].ToString("x");
                    }

                    Payload = result;
                }
            }
        }
        #endregion

        #region Constructor
        public SensorMessage(byte nodeID, byte sensorID, SensorMessageType type, bool isAckNeeded, byte subType, string payload)
        {
            NodeNo = nodeID;
            SensorNo = sensorID;
            Type = type;
            IsAckNeeded = isAckNeeded;
            SubType = subType;
            Payload = payload;
        }
        public SensorMessage(byte nodeID, byte sensorID, SensorMessageType type, bool isAckNeeded, byte subType, float payload)
        {
            NodeNo = nodeID;
            SensorNo = sensorID;
            Type = type;
            IsAckNeeded = isAckNeeded;
            SubType = subType;
            PayloadFloat = payload;
        }
        #endregion

        #region Public methods
        public static SensorMessage FromRawMessage(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            string[] parts = str.Split(new char[] { ';' }, StringSplitOptions.None);
            if (parts.Length != 6)
                return null;

            SensorMessage msg = null;

            try
            {
                msg = new SensorMessage(
                    byte.Parse(parts[0]),
                    byte.Parse(parts[1]),
                    (SensorMessageType)byte.Parse(parts[2]),
                    byte.Parse(parts[3]) == 1,
                    byte.Parse(parts[4]),
                    parts[5].Trim());
            }
            catch (Exception) { }

            return msg;
        }
        public string ToRawMessage()
        {
            return string.Format("{0};{1};{2};{3};{4};{5}",
                NodeNo,
                SensorNo,
                (byte)Type,
                IsAckNeeded ? "1" : "0",
                (byte)SubType,
                Payload);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            //sb.AppendLine(string.Format("Node ID: \t\t{0:d3}", NodeID));
            //sb.AppendLine(string.Format("Sensor ID: \t\t{0:d3}", SensorID));
            //sb.AppendLine(string.Format("Type: \t\t\t{0}", Type));
            ////sb.AppendLine(string.Format("Is ack needed: \t\t{0}", IsAckNeeded));

            //string propertyName = "Sub-type";
            //object propertyValue = SubType;
            //switch (Type)
            //{
            //    case SensorMessageType.Presentation:
            //        propertyName = "Sensor type";
            //        propertyValue = (SensorType)SubType;
            //        break;
            //    case SensorMessageType.Set:
            //    case SensorMessageType.Request:
            //        propertyName = "Value type";
            //        propertyValue = (SensorValueType)SubType;
            //        break;
            //    case SensorMessageType.Internal:
            //        propertyName = "Data type";
            //        propertyValue = (InternalValueType)SubType;
            //        break;
            //    case SensorMessageType.Stream:
            //        propertyName = "Stream data type";
            //        propertyValue = (StreamValueType)SubType;
            //        break;
            //    default:
            //        propertyName = "Sub-type";
            //        propertyValue = SubType;
            //        break;
            //}
            //sb.AppendLine(string.Format("{0}: \t\t{1}", propertyName, propertyValue));

            //sb.AppendLine(string.Format("Value: \t\t\t{0}", Payload));





            sb.Append(string.Format("[{0:d3}] ", NodeNo));
            sb.Append(string.Format("[{0:d3}] ", SensorNo));
            sb.Append(string.Format("[{0}] ", Type));
            //sb.Append(string.Format("[Ack: {0}] ", IsAckNeeded));
            switch (Type)
            {
                case SensorMessageType.Presentation:
                    sb.Append(string.Format("[{0}] ", (SensorType)SubType));
                    break;
                case SensorMessageType.Set:
                case SensorMessageType.Request:
                    sb.Append(string.Format("[{0}] ", (SensorValueType)SubType));
                    break;
                case SensorMessageType.Internal:
                    sb.Append(string.Format("[{0}] ", (InternalValueType)SubType));
                    break;
                case SensorMessageType.Stream:
                    sb.Append(string.Format("[{0}] ", (StreamValueType)SubType));
                    break;
                default:
                    sb.Append(string.Format("[{0}] ", SubType));
                    break;
            }
            sb.Append(string.Format("[{0}]", Payload));

            return sb.ToString();
        }
        #endregion

        #region Private methods
        private int CRCUpdate(int old, int value)
        {
            var c = old ^ value;

            for (var i = 0; i < 8; ++i)
                if ((c & 1) > 0)
                    c = ((c >> 1) ^ 0xA001);
                else
                    c = (c >> 1);

            return c;
        }
        private int PullWord(List<int> array, int position)
        {
            return array[position] + 256 * array[position + 1];
        }
        private void PushWord(List<int> array, int value)
        {
            array.Add(value & 0x00FF);
            array.Add((value >> 8) & 0x00FF);
        }
        private void PushDword(List<int> array, int value)
        {
            array.Add(value & 0x000000FF);
            array.Add((value >> 8) & 0x000000FF);
            array.Add((value >> 16) & 0x000000FF);
            array.Add((value >> 24) & 0x000000FF);
        }



        //const P_STRING						= 0;
        //const P_BYTE						= 1;
        //const P_INT16						= 2;
        //const P_UINT16						= 3;
        //const P_LONG32						= 4;
        //const P_ULONG32						= 5;
        //const P_CUSTOM						= 6;

        //const fwSketches					= [ ];
        //const fwDefaultType 				= 0xFFFF; // index of hex file from array above (0xFFFF)
        //const FIRMWARE_BLOCK_SIZE			= 16;

        //var fs = require('fs');
        //var path = require('path');
        //var requestify = require('requestify');
        //var appendedString="";

        private void AppendData(string str /*, db, gw*/)
        {
            //int pos = 0;
            //while (str.charAt(pos) != '\n' && pos < str.length)
            //{
            //    appendedString = appendedString + str.charAt(pos);
            //    pos++;
            //}
            //if (str.charAt(pos) == '\n')
            //{
            //    rfReceived(appendedString.trim(), db, gw);
            //    appendedString = "";
            //}
            //if (pos < str.length)
            //{
            //    AppendData(str.substr(pos + 1, str.length - pos - 1), db, gw);
            //}
        }

        //function sendFirmwareResponse(destination, fwtype, fwversion, fwblock, db, gw) {
        //    db.collection('firmware', function(err, c) {
        //        c.findOne({
        //            'type': fwtype,
        //            'version': fwversion
        //        }, function(err, result) {
        //            if (err)
        //                console.log('Error finding firmware version ' + fwversion + ' for type ' + fwtype);
        //            var payload = [];
        //            pushWord(payload, result.type);
        //            pushWord(payload, result.version);
        //            pushWord(payload, fwblock);
        //            for (var i = 0; i < FIRMWARE_BLOCK_SIZE; i++)
        //                payload.push(result.data[fwblock * FIRMWARE_BLOCK_SIZE + i]);
        //            var sensor = NODE_SENSOR_ID;
        //            var command = C_STREAM;
        //            var acknowledge = 0; // no ack
        //            var type = ST_FIRMWARE_RESPONSE;
        //            var td = encode(destination, sensor, command, acknowledge, type, payload);
        //            console.log('-> ' + td.toString());
        //            gw.write(td);
        //        });
        //    });
        //}

        //function sendFirmwareConfigResponse(destination, fwtype, fwversion, db, gw) {
        //    // keep track of type/versin info for each node
        //    // at the same time update the last modified date
        //    // could be used to remove nodes not seen for a long time etc.
        //    db.collection('node', function(err, c) {
        //        c.update({
        //            'id': destination
        //        }, {
        //            $set: {
        //                'type': fwtype,
        //                'version': fwversion,
        //                'reboot': 0
        //            }
        //        }, {
        //            upsert: true
        //        }, function(err, result) {
        //            if (err)
        //                console.log("Error writing node type and version to database");
        //        });
        //    });
        //    if (fwtype == 0xFFFF) {
        //        // sensor does not know which type / blank EEPROM
        //        // take predefined type (ideally selected in UI prior to connection of new sensor)
        //        if (fwDefaultType == 0xFFFF)
        //            throw new Error('No default sensor type defined');
        //        fwtype = fwDefaultType;
        //    }
        //    db.collection('firmware', function(err, c) {
        //        c.findOne({
        //            $query: {
        //                'type': fwtype
        //            },
        //            $orderby: {
        //                'version': -1
        //            }
        //        }, function(err, result) {
        //            if (err)
        //                console.log('Error finding firmware for type ' + fwtype);
        //            else if (!result)
        //                console.log('No firmware found for type ' + fwtype);
        //            else {
        //                var payload = [];
        //                pushWord(payload, result.type);
        //                pushWord(payload, result.version);
        //                pushWord(payload, result.blocks);
        //                pushWord(payload, result.crc);
        //                var sensor = NODE_SENSOR_ID;
        //                var command = C_STREAM;
        //                var acknowledge = 0; // no ack
        //                var type = ST_FIRMWARE_CONFIG_RESPONSE;
        //                var td = encode(destination, sensor, command, acknowledge, type, payload);
        //                console.log('-> ' + td.toString());
        //                gw.write(td);
        //            }
        //        });
        //    });
        //}

        private void LoadFirmware(/*fwtype, fwversion, sketch, db*/)
        {
            //var filename = path.basename(sketch);
            //console.log("compiling firmware: " + filename);

            //var req = {
            //        files: [{
            //                filename: filename,
            //                content: fs.readFileSync(sketch).toString()
            //        }],
            //        format: "hex",
            //        version: "105",
            //        build: {
            //                mcu: "atmega328p",
            //                f_cpu: "16000000L",
            //                core: "arduino",
            //                variant: "standard"
            //        }
            //};

        //    requestify.post('https://codebender.cc/utilities/compile/', req).then(function(res) {
        //        var body = JSON.parse(res.getBody());
        //        if (body.success)
        //        {
        //            console.log("loading firmware: " + filename);
        //            fwdata = [];
        //            var start = 0;
        //            var end = 0;
        //            var pos = 0;
        //            var hex = body.output.split("\n");

        //            for (l in hex)
        //            {
        //                line = hex[l].trim();
        //                if (line.length > 0)
        //                {
        //                    while (line.substring(0, 1) != ":")
        //                        line = line.substring(1);

        //                    var reclen = parseInt(line.substring(1, 3), 16);
        //                    var offset = parseInt(line.substring(3, 7), 16);
        //                    var rectype = parseInt(line.substring(7, 9), 16);
        //                    var data = line.substring(9, 9 + 2 * reclen);
        //                    var chksum = parseInt(line.substring(9 + (2 * reclen), 9 + (2 * reclen) + 2), 16);
					
        //                    if (rectype == 0)
        //                    {
        //                        if ((start == 0) && (end == 0))
        //                        {
        //                            if (offset % 128 > 0)
        //                                throw new Error("error loading hex file - offset can't be devided by 128");
        //                            start = offset;
        //                            end = offset;
        //                        }
        //                        if (offset < end)
        //                            throw new Error("error loading hex file - offset lower than end");

        //                        while (offset > end)
        //                        {
        //                            fwdata.push(255);
        //                            pos++;
        //                            end++;
        //                        }
        //                        for (var i = 0; i < reclen; i++)
        //                        {
        //                            fwdata.push(parseInt(data.substring(i * 2, (i * 2) + 2), 16));
        //                            pos++;
        //                        }
        //                        end += reclen;
        //                    }
        //                }
        //            }

        //            var pad = end % 128; // ATMega328 has 64 words per page / 128 bytes per page
        //            for (var i = 0; i < 128 - pad; i++)
        //            {
        //                fwdata.push(255);
        //                pos++;
        //                end++;
        //            }

        //            var blocks = (end - start) / FIRMWARE_BLOCK_SIZE;
        //            var crc = 0xFFFF;
        //            for (var i = 0; i < blocks * FIRMWARE_BLOCK_SIZE; ++i)
        //            {
        //                var v = crc;
        //                crc = CRCUpdate(crc, fwdata[i]);
        //            }

        //            db.collection('firmware', function(err, c) {
        //                c.update({
        //                    'type': fwtype,
        //                    'version': fwversion
        //                },
        //                {
        //                    $set: {
        //                        'filename': filename,
        //                        'blocks': blocks,
        //                        'crc': crc,
        //                        'data': fwdata
        //                    }
        //                },
        //                {
        //                    upsert: true
        //                },
        //                function(err, result) {
        //                    if (err)
        //                        console.log('Error writing firmware to database');
        //                });
        //            });

        //            console.log("loading firmware done. blocks: " + blocks + " / crc: " + crc);
        //        }
        //        else
        //            console.log("error: %j", res.body);

        //});
}

        //function sendFirmwareResponse(destination, fwtype, fwversion, fwblock, db, gw) {
        //    db.collection('firmware', function(err, c) {
        //        c.findOne({
        //            'type': fwtype,
        //            'version': fwversion
        //        }, function(err, result) {
        //            if (err)
        //                console.log('Error finding firmware version ' + fwversion + ' for type ' + fwtype);
        //            var payload = [];
        //            pushWord(payload, result.type);
        //            pushWord(payload, result.version);
        //            pushWord(payload, fwblock);
        //            for (var i = 0; i < FIRMWARE_BLOCK_SIZE; i++)
        //                payload.push(result.data[fwblock * FIRMWARE_BLOCK_SIZE + i]);
        //            var sensor = NODE_SENSOR_ID;
        //            var command = C_STREAM;
        //            var acknowledge = 0; // no ack
        //            var type = ST_FIRMWARE_RESPONSE;
        //            var td = encode(destination, sensor, command, acknowledge, type, payload);
        //            console.log('-> ' + td.toString());
        //            gw.write(td);
        //        });
        //    });
        //}
        #endregion
    }
}
