using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using SmartHub.UWP.Plugins.Things.Models;
using SmartHub.UWP.Plugins.UI.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;

namespace SmartHub.UWP.Plugins.Things
{
    [AppSectionItem("Things", AppSectionType.System, null, "Represents devices and their lines", "/api/things/ui/settings")]
    public class ThingsPlugin : PluginBase
    {
        #region Fields
        private Task taskValues;
        private CancellationTokenSource ctsValues;
        private bool isTaskValuesActive = false;
        #endregion

        #region Plugin overrides
        public override void InitDbModel()
        {
            var db = Context.StorageGet();

            db.CreateTable<Device>();
            db.CreateTable<Line>();
            db.CreateTable<LineValue>();

            //db.CreateTable<WemosLineMonitor>();
            ////db.CreateTable<WemosLineController>();
            //db.CreateTable<WemosController>();
            ////db.CreateTable<WemosZone>();
        }
        public override void StartPlugin()
        {
            StartValuesTask();
        }
        public override void StopPlugin()
        {
            StopValuesTask();
        }
        #endregion

        #region Public methods
        public void RegisterDevice(Device device)
        {
            if (device != null)
            {
                var item = GetDevice(device.ID);

                if (item != null)
                {
                    item.Type = device.Type;
                    item.IPAddress = device.IPAddress;

                    Context.StorageSaveOrUpdate(item);
                }
                else
                    Context.StorageSave(device);
            }
        }
        public List<Device> GetDevices()
        {
            return Context.StorageGet().Table<Device>().ToList();
        }
        public Device GetDevice(string id)
        {
            return Context.StorageGet().Table<Device>().SingleOrDefault(d => d.ID == id);
        }

        public void RegisterLine(Line line)
        {
            if (line != null)
            {
                var item = GetLine(line.ID);

                if (item != null)
                {
                    item.Type = line.Type;
                    //item.IPAddress = device.IPAddress;

                    Context.StorageSaveOrUpdate(item);
                }
                else
                    Context.StorageSave(line);
            }
        }
        public List<Line> GetLines()
        {
            return Context.StorageGet().Table<Line>()
                .Select(l => { l.TimeStamp = l.TimeStamp.ToLocalTime(); return l; }) // time in DB is in UTC; convert to local
                .ToList();
        }
        public Line GetLine(string id)
        {
            return Context.StorageGet().Table<Line>()
                .Select(l => { l.TimeStamp = l.TimeStamp.ToLocalTime(); return l; })
                .FirstOrDefault(l => l.ID == id);
        }



        public List<LineValue> GetLineValues(string lineID, int count)
        {
            return Context.StorageGet().Table<LineValue>()
                .Where(v => v.LineID == lineID)// && v.TimeStamp > DateTime.Now.AddDays(-1))
                .OrderByDescending(v => v.TimeStamp)
                .Take(count)
                .OrderBy(v => v.TimeStamp)
                .Select(v => { v.TimeStamp = v.TimeStamp.ToLocalTime(); return v; }) // time in DB is in UTC; convert to local time
                .ToList();
        }
        public LineValue GetLineLastValue(string lineID)
        {
            return Context.StorageGet().Table<LineValue>()
                .Where(v => v.LineID == lineID)
                .OrderByDescending(v => v.TimeStamp)
                //.Take(1)
                .Select(v => { v.TimeStamp = v.TimeStamp.ToLocalTime(); return v; }) // time in DB is in UTC; convert to local time
                .FirstOrDefault();
        }

        public static bool IsValueFromLine(LineValue val, string lineID)
        {
            return val != null && lineID == val.LineID;
        }
        #endregion

        #region Private methods
        private void StartValuesTask()
        {
            if (!isTaskValuesActive)
            {
                ctsValues = new CancellationTokenSource();

                taskValues = Task.Factory.StartNew(async () =>
                {
                    while (!ctsValues.IsCancellationRequested)
                        if (isTaskValuesActive)
                        {
                            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                            {
                                //for (int i = 0; i < Context.GetPlugin<LinesPlugin>().oldControllers.Count; i++)
                                //    Context.GetPlugin<LinesPlugin>().oldControllers[i].ProcessTimer(DateTime.Now);

                                //var controllers = Context.GetPlugin<WemosPlugin>().controllers;
                                //for (int i = 0; i < controllers.Count; i++)
                                //    await controllers[i].ProcessAsync();
                            });

                            await Task.Delay(50);
                        }

                }, ctsValues.Token);

                isTaskValuesActive = true;
            }
        }
        private void StopValuesTask()
        {
            if (isTaskValuesActive)
            {
                ctsValues?.Cancel();
                isTaskValuesActive = false;
            }
        }
        #endregion

        #region Remote API




        #region Web UI
        [ApiMethod("/api/things/ui/settings")]
        public ApiMethod apiGetUISettings => (args =>
        {
            var stream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream("SmartHub.UWP.Plugins.Things.UIWeb.Settings.html");
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        });
        #endregion

        #endregion
    }
}
