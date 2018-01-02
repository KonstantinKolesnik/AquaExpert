using SmartHub.UWP.Core.Plugins;
using SmartHub.UWP.Plugins.ApiListener;
using SmartHub.UWP.Plugins.ApiListener.Attributes;
using SmartHub.UWP.Plugins.Lines.Models;
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

namespace SmartHub.UWP.Plugins.Lines
{
    [AppSectionItem("Lines", AppSectionType.System, null, "Lines represent things/entities", "/api/lines/ui/settings")]
    public class LinesPlugin : PluginBase
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

            //db.CreateTable<WemosNode>();
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
                .Take(1)
                .Select(v => { v.TimeStamp = v.TimeStamp.ToLocalTime(); return v; }) // time in DB is in UTC; convert to local time
                .FirstOrDefault();
        }

        public static bool IsValueFromLine(LineValue val, string lineID)
        {
            return val != null && lineID == val.LineID;
        }


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
        [ApiMethod("/api/lines/ui/settings")]
        public ApiMethod apiGetUISettings => (args =>
        {
            var stream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream("SmartHub.UWP.Plugins.Lines.UIWeb.Settings.html");
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        });
        #endregion

        #endregion
    }
}
